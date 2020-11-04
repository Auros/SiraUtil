using Zenject;
using HarmonyLib;
using ModestTree;
using System.Linq;
using UnityEngine;
using System.Reflection;
using SiraUtil.Services;
using SiraUtil.Interfaces;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace SiraUtil.Sabers
{
    [HarmonyPatch(typeof(SaberModelContainer), nameof(SaberModelContainer.Start))]
    internal class SaberBindPatch
    {
        internal static bool Prefix(ref Saber ____saber, ref DiContainer ____container, ref SaberModelContainer __instance, SaberModelController ____saberModelControllerPrefab)
        {
            var providers = ____container.Resolve<List<IModelProvider>>().Where(x => x.Type.DerivesFrom(typeof(SaberModelController)) && x.Priority >= 0);
            var provider = GetProvider(____container);
            if (providers.Count() == 0)
            {
                if (!provider.IsSafe())
                {
                    provider.ModelPrefab = ____saberModelControllerPrefab;
                }
                return true;
            }
            var baseProvider = providers.OrderByDescending(x => x.Priority).First();
            var originalPrefab = ____saberModelControllerPrefab;
            var instanceTransform = __instance.transform;
            var container = ____container;
            var saber = ____saber;

            if (!provider.IsSafe(false))
            {
                var smc = new GameObject(baseProvider.GetType().FullName).AddComponent(baseProvider.Type) as SaberModelController;
                SetupSaber(____container, ____saberModelControllerPrefab, smc);
                provider.ModelPrefab = smc;
            }
            provider.GetModel((smc) =>
            {
                container.QueueForInject(smc);
                SetupSaber(container, originalPrefab, smc);
                smc.gameObject.transform.SetParent(instanceTransform, false);
                smc.Init(instanceTransform, saber);
            });
            return false;
        }

        private static void SetupSaber(DiContainer container, SaberModelController prefab, SaberModelController destination)
        {
            Accessors.SaberTrail(ref destination) = Accessors.SaberTrail(ref prefab);
            Accessors.SaberGlowColor(ref destination) = Accessors.SaberGlowColor(ref prefab);
            var glowColors = Accessors.SaberGlowColor(ref destination);
            for (int i = 0; i < glowColors.Length; i++)
            {
                container.Inject(glowColors[i]);
            }
            Accessors.FakeSaberGlowColor(ref destination) = Accessors.FakeSaberGlowColor(ref prefab);
            var fakeGlowColors = Accessors.FakeSaberGlowColor(ref destination);
            for (int i = 0; i < fakeGlowColors.Length; i++)
            {
                container.Inject(fakeGlowColors[i]);
            }
            Accessors.SaberLight(ref destination) = Accessors.SaberLight(ref prefab);
        }

        private static SaberProvider GetProvider(DiContainer container)
        {
            var provider = container.Resolve<SaberProvider>();
            return provider;
        }
    }

    [HarmonyPatch(typeof(GameplayCoreInstaller), "InstallBindings")]
    public class GameplayCoreClashCheckerSwap
    {
        private static readonly MethodInfo _clashAttacher = SymbolExtensions.GetMethodInfo(() => ClashAttacher(null));

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            int runIns = 0;
            for (int i = codes.Count - 1; i > 15; i--)
            {
                if (codes[i].opcode == OpCodes.Pop && codes[i - 1].opcode == OpCodes.Callvirt &&
                    codes[i - 2].opcode == OpCodes.Callvirt && codes[i - 3].opcode == OpCodes.Call &&
                    codes[i - 4].opcode == OpCodes.Ldarg_0)
                {
                    runIns++;
                    if (runIns == 2)
                    {
                        codes.Insert(i - 1, new CodeInstruction(OpCodes.Callvirt, _clashAttacher));
                        break;
                    }
                }
            }
            return codes.AsEnumerable();
        }

        private static FromBinderGeneric<SiraSaberClashChecker> ClashAttacher(ConcreteIdBinderGeneric<SaberClashChecker> contract)
        {
            return contract.To<SiraSaberClashChecker>();
        }
    }
}