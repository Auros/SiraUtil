using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace SiraUtil.Sabers.Effects
{
    [HarmonyPatch(typeof(SaberBurnMarkSparkles))]
    internal class SaberBurnMarkSparklesPatch
    {
        private static readonly MethodInfo _evaluateState = SymbolExtensions.GetMethodInfo(() => CountOrDefault(null!));
        
        [HarmonyTranspiler]
        [HarmonyPatch(nameof(SaberBurnMarkSparkles.OnEnable))]
        internal static IEnumerable<CodeInstruction> DynamicEnable(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();
            ProcessEnableDisable(ref codes);
            return codes;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(nameof(SaberBurnMarkSparkles.OnDisable))]
        internal static IEnumerable<CodeInstruction> DynamicDisable(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();
            ProcessEnableDisable(ref codes);
            return codes;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(nameof(SaberBurnMarkSparkles.LateUpdate))]
        internal static IEnumerable<CodeInstruction> DynamicLateUpdate(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();
            ProcessEnableDisable(ref codes);
            return codes;
        }

        private static void ProcessEnableDisable(ref List<CodeInstruction> codes)
        {
            object? burnMarks = null;
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldfld && burnMarks is null) // We collect the operand that we are going to be using (_burnMarksPS or _sabers)
                    burnMarks = codes[i].operand;

                if (codes[i].opcode == OpCodes.Ldc_I4_2)
                {
                    codes.RemoveAt(i); // Remove the '2' being pushed onto the stack
                    codes.InsertRange(i, new CodeInstruction[] // And use the Length property of the array
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, burnMarks), // this needs operand of _burnMarksPs or _sabers
                        new CodeInstruction(OpCodes.Callvirt, _evaluateState)
                    });
                    break;
                }
            }
        }

        // Since OnEnable can be called before the burn sparkles Start, we need to check if the array are null or not. 
        private static int CountOrDefault(object[] ps)
        {
            return ps is not null ? ps.Length : 2;
        }
    }
}