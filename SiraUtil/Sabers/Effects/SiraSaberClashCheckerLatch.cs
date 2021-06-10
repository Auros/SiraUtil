using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Zenject;

namespace SiraUtil.Sabers.Effects
{
    [HarmonyPatch(typeof(GameplayCoreInstaller), nameof(GameplayCoreInstaller.InstallBindings))]
    internal class SiraSaberClashCheckerLatch
    {
        private static readonly MethodInfo _rootMethod = typeof(DiContainer).GetMethod(nameof(DiContainer.Bind), Array.Empty<Type>());
        private static readonly MethodInfo _clashAttacher = SymbolExtensions.GetMethodInfo(() => ClashAttacher(null!));
        private static readonly MethodInfo _originalMethod = _rootMethod.MakeGenericMethod(new Type[] { typeof(SaberClashChecker) });

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].Calls(_originalMethod))
                {
                    codes.Insert(i + 1, new CodeInstruction(OpCodes.Callvirt, _clashAttacher));
                    break;
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