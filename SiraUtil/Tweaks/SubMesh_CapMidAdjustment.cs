using HMUI;
using TMPro;
using HarmonyLib;

namespace SiraUtil.Tweaks
{
    [HarmonyPatch(typeof(CurvedTextMeshPro), "OnEnable")]
    internal class SubMesh_CapMidAdjustment
    {
        internal static void Postfix(ref CurvedTextMeshPro __instance)
        {
            if (__instance.alignment == TextAlignmentOptions.Capline)
            {
                __instance.alignment = TextAlignmentOptions.Midline;
            }
        }
    }
}