using HarmonyLib;
using ModestTree;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SiraUtil.Tweaks
{
    [HarmonyPatch(typeof(TypeStringFormatter), nameof(TypeStringFormatter.PrettyNameInternal))]
    internal static class ResolveExceptionDetailer
    {
        private static readonly MethodInfo MemberInfoNameGetter = AccessTools.DeclaredPropertyGetter(typeof(MemberInfo), nameof(MemberInfo.Name));
        private static readonly MethodInfo FullNameAndAssemblyMethod = AccessTools.DeclaredMethod(typeof(ResolveExceptionDetailer), nameof(FullNameAndAssembly));

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                .MatchForward(false, new CodeMatch(i => i.Calls(MemberInfoNameGetter)))
                .SetAndAdvance(OpCodes.Call, FullNameAndAssemblyMethod)
                .InstructionEnumeration();
        }

        private static string FullNameAndAssembly(Type type) => $"{type.FullName}, {type.Assembly.GetName().Name}";
    }
}
