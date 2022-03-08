using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Zenject;

namespace SiraUtil.Objects
{
    internal class InternalRedecorator
    {
        private static readonly MethodInfo _getType = typeof(object).GetMethod(nameof(object.GetType));
        private static readonly MethodInfo _prefabInitializingField = SymbolExtensions.GetMethodInfo(() => PrefabInitializing(null!, null!, null!, null!));
        private static readonly MethodInfo _newPrefabMethod = typeof(FactoryFromBinderBase).GetMethod(nameof(FactoryFromBinderBase.FromComponentInNewPrefab));
        private static readonly MethodInfo _getContainerMethod = typeof(MonoInstallerBase).GetProperty("Container", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod();

        [HarmonyPatch(typeof(BeatmapObjectsInstaller), nameof(BeatmapObjectsInstaller.InstallBindings))]
        internal class BeatmapObjects
        {
            [HarmonyTranspiler]
            protected static IEnumerable<CodeInstruction> Redecorate(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> codes = instructions.ToList();
                InternalRedecorator.Redecorate(ref codes);
                return codes;
            }
        }

        [HarmonyPatch(typeof(EffectPoolsManualInstaller), nameof(EffectPoolsManualInstaller.ManualInstallBindings))]
        internal class EffectPools
        {
            [HarmonyTranspiler]
            protected static IEnumerable<CodeInstruction> Redecorate(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> codes = instructions.ToList();
                InternalRedecorator.Redecorate(ref codes);
                return codes;
            }
        }

        [HarmonyPatch(typeof(MultiplayerConnectedPlayerInstaller), nameof(MultiplayerConnectedPlayerInstaller.InstallBindings))]
        internal class MultiplayerConnectedPlayer
        {
            [HarmonyTranspiler]
            protected static IEnumerable<CodeInstruction> Redecorate(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> codes = instructions.ToList();
                InternalRedecorator.Redecorate(ref codes);
                return codes;
            }
        }

        private static UnityEngine.Object PrefabInitializing(UnityEngine.Object originalPrefab, DiContainer container, string fieldName, Type mainType)
        {
            IEnumerable<RedecoratorRegistration> registrations = container.AncestorContainers[0].Resolve<List<RedecoratorRegistration>>().Where(rr => rr.ContainerType == mainType && rr.Contract == fieldName).OrderByDescending(rr => rr.Priority);

            if (!registrations.Any())
                return originalPrefab;

            GameObject irgo = new(nameof(InternalRedecorator));
            irgo.SetActive(false);
            UnityEngine.Object.Instantiate(originalPrefab, irgo.transform);
            UnityEngine.Object clone = irgo.GetComponentInChildren(registrations.First().PrefabType);

            foreach (var registration in registrations)
            {
                registration.Redecorate(clone);
                if (!registration.Chain)
                    break;
            }

            container.LazyInject(new ObjectDiffuser(irgo));

            return clone;
        }

        internal static void Redecorate(ref List<CodeInstruction> codes)
        {
            OpCode? containerOpcode = null!;
            object? containerOperand = null!;

            for (int i = 0; i < codes.Count - 1; i++)
            {
                if (codes[i].opcode == OpCodes.Ldfld && (codes[i + 1].Calls(_newPrefabMethod) || (codes.Count > i + 4 && codes[i + 4].Calls(_newPrefabMethod)))) // uhhh for teranary operators :PogOh:
                {
                    if (containerOpcode is null && containerOperand is null)
                    {
                        for (int c = i; c >= 0; c--)
                        {
                            // We are in a MonoInstallerBase
                            if (codes[c].opcode == OpCodes.Call)
                            {
                                containerOpcode = OpCodes.Callvirt;
                                containerOperand = codes[c].operand;
                                break;
                            }
                        }

                        // We are doing a manual install.
                        if (containerOperand is null)
                            containerOpcode = OpCodes.Ldarg_1;
                    }
                    MemberInfo activeField = (codes[i].operand as MemberInfo)!;
                    string fieldName = activeField.Name;

                    List<CodeInstruction> toInsert = new();
                    if (containerOperand is not null)
                        toInsert.Add(new(OpCodes.Ldarg_0));
                    toInsert.Add(new(containerOpcode ?? OpCodes.Ldarg_1, containerOperand));
                    toInsert.Add(new(OpCodes.Ldstr, fieldName));
                    toInsert.Add(new(OpCodes.Ldarg_0));
                    toInsert.Add(new(OpCodes.Callvirt, _getType));
                    toInsert.Add(new(OpCodes.Callvirt, _prefabInitializingField));
                    codes.InsertRange(i + 1, toInsert);
                }
            }
        }
    }
}