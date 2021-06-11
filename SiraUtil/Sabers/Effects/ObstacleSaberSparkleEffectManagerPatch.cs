using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace SiraUtil.Sabers.Effects
{
    [HarmonyPatch(typeof(ObstacleSaberSparkleEffectManager))]
    internal class ObstacleSaberSparkleEffectManagerPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(ObstacleSaberSparkleEffectManager.OnDisable))]
        internal static void DynamicDisable(ref bool[] ____isSystemActive)
        {
            if (____isSystemActive.Length > 2)
                for (int i = 2; i < ____isSystemActive.Length; i++)
                    ____isSystemActive[i] = false;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(nameof(ObstacleSaberSparkleEffectManager.Update))]
        internal static IEnumerable<CodeInstruction> DynamicUpdate(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();
            TwoToLength(ref codes);
            return codes;
        }

        private static void TwoToLength(ref List<CodeInstruction> codes)
        {
            object? array = null;

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldfld && array is null) // We collect the operand that we are going to be using (_isSystemActive or _wasSystemActive)
                    array = codes[i].operand;

                if (codes[i].opcode == OpCodes.Ldc_I4_2)
                {
                    codes.RemoveAt(i); // Remove the '2' being pushed onto the stack
                    codes.InsertRange(i, new CodeInstruction[] // And use the Length property of the array
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, array), // this needs operand of _isSystemActive or _wasSystemActive
                        new CodeInstruction(OpCodes.Ldlen),
                        new CodeInstruction(OpCodes.Conv_I4)
                    });
                }
            }
        }
    }
}