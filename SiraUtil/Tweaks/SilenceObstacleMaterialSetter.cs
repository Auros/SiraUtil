using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace SiraUtil.Tweaks
{
    [HarmonyPatch(typeof(ObstacleMaterialSetter), nameof(ObstacleMaterialSetter.SetCoreMaterial))]
    internal class SilenceObstacleMaterialSetter
    {
        private static readonly MethodInfo DebugLogWarningMethod = AccessTools.DeclaredMethod(typeof(Debug), nameof(Debug.LogWarning), [typeof(object)]);

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                .MatchForward(
                    false,
                    new CodeMatch(OpCodes.Ldstr),
                    new CodeMatch(i => i.Calls(DebugLogWarningMethod)))
                .SetAndAdvance(OpCodes.Nop, null) // keep the label
                .RemoveInstruction()
                .InstructionEnumeration();
        }
    }
}
