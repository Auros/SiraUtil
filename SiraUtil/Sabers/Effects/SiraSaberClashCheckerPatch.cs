using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Zenject;

namespace SiraUtil.Sabers.Effects
{
    internal class SiraSaberClashCheckerPatch
    {
        internal static readonly MethodInfo _rootMethod = typeof(DiContainer).GetMethod(nameof(DiContainer.Bind), Array.Empty<Type>());
        internal static readonly MethodInfo _clashAttacher = SymbolExtensions.GetMethodInfo(() => ClashAttacher(null!));
        internal static readonly MethodInfo _originalMethod = _rootMethod.MakeGenericMethod(new Type[] { typeof(SaberClashChecker) });

        [HarmonyPatch(typeof(GameplayCoreInstaller), nameof(GameplayCoreInstaller.InstallBindings))]
        private class GameplayCore
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Upgrade(IEnumerable<CodeInstruction> instructions)
            {
                var codes = instructions.ToList();
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
                var codes = instructions.ToList();
                SiraSaberClashCheckerPatch.Upgrade(ref codes);
                return codes.AsEnumerable();
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