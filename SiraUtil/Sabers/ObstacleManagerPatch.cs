using HarmonyLib;
using System.Linq;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace SiraUtil.Sabers
{
    [HarmonyPatch(typeof(ObstacleSaberSparkleEffectManager), nameof(ObstacleSaberSparkleEffectManager.OnDisable))]
    internal class ObstacleManagerPatch_OnDisable : ObstacleSaberSparkleEffectManager
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            var ldf = codes.Last(u => u.opcode == OpCodes.Ldfld);
            for (int i = 0; i < codes.Count(); i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_I4_2)
                {
                    codes.RemoveAt(i);
                    codes.InsertRange(i, new CodeInstruction[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, ldf.operand),
                        new CodeInstruction(OpCodes.Ldlen),
                        new CodeInstruction(OpCodes.Conv_I4)
                    });
                    break;
                }
            }
            return codes;
        }
    }

    [HarmonyPatch(typeof(ObstacleSaberSparkleEffectManager), nameof(ObstacleSaberSparkleEffectManager.Update))]
    internal class ObstacleManagerPatch_Update
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            var ldf = codes.Last(u => u.opcode == OpCodes.Ldfld);
            for (int i = 0; i < codes.Count(); i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_I4_2)
                {
                    codes.RemoveAt(i);
                    codes.InsertRange(i, new CodeInstruction[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, ldf.operand),
                        new CodeInstruction(OpCodes.Ldlen),
                        new CodeInstruction(OpCodes.Conv_I4)
                    });
                }
            }
            return codes;
        }
    }
}