using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Zenject;

namespace SiraUtil.Sabers.Effects
{
    internal class SiraSaberClashCheckerPatch
    {
        internal static readonly MethodInfo _rootMethod = typeof(DiContainer).GetMethod(nameof(DiContainer.Bind), []);
        internal static readonly MethodInfo _clashAttacher = SymbolExtensions.GetMethodInfo(() => ClashAttacher(null!));
        internal static readonly MethodInfo _originalMethod = _rootMethod.MakeGenericMethod([typeof(SaberClashChecker)]);

        [HarmonyPatch(typeof(GameplayCoreInstaller), nameof(GameplayCoreInstaller.InstallBindings))]
        private class GameplayCore
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Upgrade(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> codes = [.. instructions];
                SiraSaberClashCheckerPatch.Upgrade(ref codes);
                return codes.AsEnumerable();
            }
        }

        [HarmonyPatch(typeof(TutorialInstaller), nameof(TutorialInstaller.InstallBindings))]
        private class Tutorial
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Upgrade(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> codes = [.. instructions];
                SiraSaberClashCheckerPatch.Upgrade(ref codes);
                return codes.AsEnumerable();
            }

        }

        [HarmonyPatch(typeof(SaberClashChecker), nameof(SaberClashChecker.AreSabersClashing))]
        private class DefaultOverride
        {
            [HarmonyPrefix]
            public static bool OverrideDefault(ref SaberClashChecker __instance, ref bool ____sabersAreClashing, ref Vector3 ____clashingPoint, ref int ____prevGetFrameNum, ref bool __result, out Vector3 clashingPoint)
            {
                if (__instance is not ISiraClashChecker customClashChecker || !customClashChecker.ExtraSabersDetected)
                {
                    clashingPoint = Vector3.zero;
                    return true;
                }
                __result = customClashChecker.AreSabersClashing(ref ____sabersAreClashing, ref ____clashingPoint, ref ____prevGetFrameNum, out clashingPoint);
                return false;
            }
        }

        internal static void Upgrade(ref List<CodeInstruction> codes)
        {
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].Calls(_originalMethod))
                {
                    codes.Insert(i + 1, new CodeInstruction(OpCodes.Callvirt, _clashAttacher));
                    break;
                }
            }
        }

        private static FromBinderGeneric<SiraSaberClashChecker> ClashAttacher(ConcreteIdBinderGeneric<SaberClashChecker> contract)
        {
            return contract.To<SiraSaberClashChecker>();
        }
    }
}