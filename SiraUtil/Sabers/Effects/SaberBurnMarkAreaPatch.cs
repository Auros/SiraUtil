using HarmonyLib;
using System.Collections.Generic;
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
        private static readonly FieldInfo _lineRendererInfo = typeof(SaberBurnMarkArea).GetField(nameof(SaberBurnMarkArea._lineRenderers), BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo _sabersInfo = typeof(SaberBurnMarkArea).GetField(nameof(SaberBurnMarkArea._sabers), BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly MethodInfo _rendererGetEnabled = typeof(Renderer).GetProperty(nameof(Renderer.enabled), BindingFlags.Public | BindingFlags.Instance).GetMethod;

        [HarmonyPostfix]
        [HarmonyPatch(nameof(SaberBurnMarkArea.OnEnable))]
        internal static void DynamicEnable(ref LineRenderer[] ____lineRenderers)
        {
            if (____lineRenderers is not null && ____lineRenderers.Length > 2)
            {
                for (int i = 2; i < ____lineRenderers.Length; i++)
                {
                    ____lineRenderers[i].gameObject.SetActive(true);
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(SaberBurnMarkArea.OnDisable))]
        internal static void DynamicDisable(ref LineRenderer[] ____lineRenderers)
        {
            if (____lineRenderers is not null && ____lineRenderers.Length > 2)
            {
                for (int i = 2; i < ____lineRenderers.Length; i++)
                {
                    ____lineRenderers[i].gameObject.SetActive(false);
                }
            }
        }

        [HarmonyTranspiler]
        [HarmonyPatch(nameof(SaberBurnMarkArea.LateUpdate))]
        internal static IEnumerable<CodeInstruction> DynamicUpdate(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                // replace hardcoded 2 in `for (int i = 0; i < 2; i++)` with length of _sabers array
                .MatchForward(
                    false,
                    new CodeMatch(OpCodes.Ldc_I4_2),
                    new CodeMatch(OpCodes.Blt))
                .ThrowIfInvalid("Ldc_I4_2 not found")
                .RemoveInstruction()
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, _sabersInfo),
                    new CodeInstruction(OpCodes.Ldlen),
                    new CodeInstruction(OpCodes.Conv_I4))
                // remove hardcoded check of _lineRenderers at index 0 and 1 with check on everything in the array
                // `if (_lineRenderers[0].enabled || _lineRenderers[1].enabled)`
                .MatchForward(
                    false,
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(i => i.LoadsField(_lineRendererInfo)),
                    new CodeMatch(OpCodes.Ldc_I4_0),
                    new CodeMatch(OpCodes.Ldelem_Ref),
                    new CodeMatch(i => i.Calls(_rendererGetEnabled)),
                    new CodeMatch(OpCodes.Brtrue),
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(i => i.LoadsField(_lineRendererInfo)),
                    new CodeMatch(OpCodes.Ldc_I4_1),
                    new CodeMatch(OpCodes.Ldelem_Ref),
                    new CodeMatch(i => i.Calls(_rendererGetEnabled)),
                    new CodeMatch(OpCodes.Brfalse))
                .ThrowIfInvalid("_lineRenderers comparison not found")
                .Advance(2) // keep _lineRenderers field load
                .RemoveInstructions(9) // remove everything until the final brfalse
                .InsertAndAdvance(new CodeInstruction(OpCodes.Call, _evaluateAllRenderers))
                .InstructionEnumeration();
        }

        // This destroys the other line renderers in between where the first ones were destroyed and when the fade out material is destroyed.
        [HarmonyTranspiler]
        [HarmonyPatch(nameof(SaberBurnMarkArea.OnDestroy))]
        internal static IEnumerable<CodeInstruction> DynamicDestroy(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = [.. instructions];
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
                    codes.InsertRange(insertIndex,
                    [
                        new(OpCodes.Ldarg_0),
                        new(OpCodes.Ldfld, lineRendererOperand),
                        new(OpCodes.Callvirt, _destroyExtraLines)
                    ]);
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
                {
                    return true;
                }
            }
            return false;
        }

        private static void DestroyExtraLines(LineRenderer[] lineRenderers)
        {
            if (lineRenderers is not null && lineRenderers.Length > 2)
            {
                for (int i = 2; i < lineRenderers.Length; i++)
                {
                    if (lineRenderers[i] != null)
                    {
                        Object.Destroy(lineRenderers[i]);
                    }
                }
            }
        }
    }
}