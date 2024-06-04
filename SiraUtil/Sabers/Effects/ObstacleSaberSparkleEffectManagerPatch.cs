﻿using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace SiraUtil.Sabers.Effects
{
    [HarmonyPatch(typeof(ObstacleSaberSparkleEffectManager))]
    internal class ObstacleSaberSparkleEffectManagerPatch
    {
        private static readonly FieldInfo _sabersField = typeof(ObstacleSaberSparkleEffectManager).GetField("_sabers", BindingFlags.NonPublic | BindingFlags.Instance);

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
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_I4_2)
                {
                    codes.RemoveAt(i); // Remove the '2' being pushed onto the stack
                    codes.InsertRange(i, new CodeInstruction[] // And use the Length property of the array
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, _sabersField),
                        new CodeInstruction(OpCodes.Ldlen),
                        new CodeInstruction(OpCodes.Conv_I4)
                    });
                }
            }
        }
    }
}