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
        private const string NewContextPrefabMethodName = "ByNewContextPrefab";
        private const string GetContainerMethodName = "get_Container";
        private const string ContainerFieldName = "_container";
        private static readonly MethodInfo _getType = typeof(object).GetMethod(nameof(object.GetType));
        private static readonly MethodInfo _prefabInitializingField = SymbolExtensions.GetMethodInfo(() => PrefabInitializing(null!, null!, null!, null!));
        private static readonly MethodInfo _newPrefabMethod = typeof(FactoryFromBinderBase).GetMethod(nameof(FactoryFromBinderBase.FromComponentInNewPrefab));

        [HarmonyPatch(typeof(BeatmapObjectsInstaller), nameof(BeatmapObjectsInstaller.InstallBindings))]
        internal class BeatmapObjects
        {
            [HarmonyTranspiler]
            protected static IEnumerable<CodeInstruction> Redecorate(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> codes = [.. instructions];
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
                List<CodeInstruction> codes = [.. instructions];
                InternalRedecorator.Redecorate(ref codes);
                return codes;
            }
        }

        [HarmonyPatch(typeof(NoteDebrisPoolInstaller), nameof(NoteDebrisPoolInstaller.InstallBindings))]
        internal class NoteDebrisPool
        {
            [HarmonyTranspiler]
            protected static IEnumerable<CodeInstruction> Redecorate(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> codes = [.. instructions];
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
                List<CodeInstruction> codes = [.. instructions];
                InternalRedecorator.Redecorate(ref codes);
                return codes;
            }
        }

        [HarmonyPatch(typeof(MultiplayerLobbyInstaller), nameof(MultiplayerLobbyInstaller.InstallBindings))]
        internal class MultiplayerLobby
        {
            [HarmonyTranspiler]
            protected static IEnumerable<CodeInstruction> Redecorate(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> codes = [.. instructions];
                InternalRedecorator.Redecorate(ref codes);
                return codes;
            }
        }

        [HarmonyPatch(typeof(MultiplayerPlayersManager), nameof(MultiplayerPlayersManager.BindPlayerFactories))]
        internal class MultiplayerPlayerFactories
        {
            [HarmonyTranspiler]
            protected static IEnumerable<CodeInstruction> Redecorate(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> codes = [.. instructions];
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
                if (codes[i].opcode == OpCodes.Ldfld && (codes[i + 1].Calls(_newPrefabMethod) || (codes[i + 1].opcode == OpCodes.Callvirt && ((MethodInfo)codes[i + 1].operand).Name == NewContextPrefabMethodName) || (codes.Count > i + 4 && codes[i + 4].Calls(_newPrefabMethod)))) // uhhh for teranary operators :PogOh:
                {
                    if (containerOpcode is null && containerOperand is null)
                    {
                        for (int c = i; c >= 0; c--)
                        {
                            // We are in a MonoInstallerBase/ScriptableObjectInstallerBase
                            if (codes[c].opcode == OpCodes.Call && ((MethodInfo)codes[c].operand).Name == GetContainerMethodName)
                            {
                                containerOpcode = OpCodes.Callvirt;
                                containerOperand = codes[c].operand;
                                break;
                            }

                            if (codes[c].opcode == OpCodes.Ldfld && ((FieldInfo)codes[c].operand).Name == ContainerFieldName)
                            {
                                containerOpcode = OpCodes.Ldfld;
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

                    List<CodeInstruction> toInsert = [];
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