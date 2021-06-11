using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace SiraUtil.Sabers.Effects
{
    [HarmonyPatch(typeof(SaberBurnMarkArea))]
    internal class SaberBurnMarkAreaPatch
    {
        private static readonly MethodInfo _destroyExtraLines = SymbolExtensions.GetMethodInfo(() => DestroyExtraLines(null!));
        private static readonly MethodInfo _evaluateAllRenderers = SymbolExtensions.GetMethodInfo(() => CompareAllRenderers(null!));
        private static readonly MethodInfo _safeDestroyMethodInfo = SymbolExtensions.GetMethodInfo(() => EssentialHelpers.SafeDestroy(null!));
        private static readonly FieldInfo _lineRendererInfo = typeof(SaberBurnMarkArea).GetField("_lineRenderers", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly List<OpCode> _lineCheck = new()
        {
            OpCodes.Ldarg_0,
            OpCodes.Ldfld,
            OpCodes.Ldc_I4_0,
            OpCodes.Ldelem_Ref,
            OpCodes.Callvirt,
            OpCodes.Brtrue_S
        };

        [HarmonyPostfix]
        [HarmonyPatch(nameof(SaberBurnMarkArea.OnEnable))]
        internal static void DynamicEnable(ref LineRenderer[] ____lineRenderers)
        {
            if (____lineRenderers is not null && ____lineRenderers.Length > 2)
                for (int i = 2; i < ____lineRenderers.Length; i++)
                    ____lineRenderers[i].gameObject.SetActive(true);
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(SaberBurnMarkArea.OnDisable))]
        internal static void DynamicDisable(ref LineRenderer[] ____lineRenderers)
        {
            if (____lineRenderers is not null && ____lineRenderers.Length > 2)
                for (int i = 2; i < ____lineRenderers.Length; i++)
                    ____lineRenderers[i].gameObject.SetActive(false);
        }

        [HarmonyTranspiler]
        [HarmonyPatch(nameof(SaberBurnMarkArea.LateUpdate))]
        internal static IEnumerable<CodeInstruction> DynamicUpdate(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();
            TwoToLength(ref codes);

            bool removedNativeSwap = false;
            for (int i = codes.Count - 1; i >= 0; i--)
            {
                if (removedNativeSwap && codes[i].Is(OpCodes.Ldfld, _lineRendererInfo))
                {
                    codes.RemoveAt(i + 1);
                    codes.RemoveAt(i + 1);
                    codes[i + 1] = new CodeInstruction(OpCodes.Callvirt, _evaluateAllRenderers);
                    break;
                }
                if (!removedNativeSwap && codes[i].opcode == OpCodes.Call)
                {
                    codes.RemoveRange(i + 1, codes.Count - i - 1);
                    codes.Add(new(OpCodes.Ret));
                    removedNativeSwap = true;
                }
            }
            return codes;
        }

        // This destroys the other line renderers in between where the first ones were destroyed and when the fade out material is destroyed.
        [HarmonyTranspiler]
        [HarmonyPatch(nameof(SaberBurnMarkArea.OnDestroy))]
        internal static IEnumerable<CodeInstruction> DynamicDestroy(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].Calls(_safeDestroyMethodInfo))
                {
                    while (i > 0 && codes[i].opcode != OpCodes.Ldarg_0)
                    {
                        i--; // Move backwards until we get to the start of the SafeDestroy sequence.
                    }
                    int insertIndex = i;
                    while (i > 0 && codes[i].opcode != OpCodes.Ldfld)
                    {
                        i--; // Move backwards until we get the operand of the line renderers.
                    }
                    object lineRendererOperand = codes[i].operand;
                    codes.InsertRange(insertIndex, new CodeInstruction[]
                    {
                        new(OpCodes.Ldarg_0),
                        new(OpCodes.Ldfld, lineRendererOperand),
                        new(OpCodes.Callvirt, _destroyExtraLines)
                    });
                    break;
                }
            }
            return codes;
        }

        private static bool CompareAllRenderers(LineRenderer[] lineRenderers)
        {
            for (int i = 0; i < lineRenderers.Length; i++)
            {
                if (lineRenderers[i].enabled)
                    return true;
            }
            return false;
        }

        private static void DestroyExtraLines(LineRenderer[] lineRenderers)
        {
            if (lineRenderers is not null && lineRenderers.Length > 2)
                for (int i = 2; i < lineRenderers.Length; i++)
                    if (lineRenderers[i] != null)
                        Object.Destroy(lineRenderers[i]);
        }

        private static void TwoToLength(ref List<CodeInstruction> codes)
        {
            object? array = null;

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldfld && array is null) // We collect the operand that we are going to be using _sabers
                    array = codes[i].operand;

                if (codes[i].opcode == OpCodes.Ldc_I4_2)
                {
                    codes.RemoveAt(i); // Remove the '2' being pushed onto the stack
                    codes.InsertRange(i, new CodeInstruction[] // And use the Length property of the array
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, array), // this needs operand of _sabers
                        new CodeInstruction(OpCodes.Ldlen),
                        new CodeInstruction(OpCodes.Conv_I4)
                    });
                    break;
                }
            }
        }
    }
}