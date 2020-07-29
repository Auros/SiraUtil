using HarmonyLib;

namespace SiraUtil.Events
{
    internal class DetailViewControllerPatch
    {
        [HarmonyPatch(typeof(StandardLevelDetailView), "RefreshContent")]
        internal class PatchRefreshContent
        {
            internal static void Postfix(ref StandardLevelDetailView __instance, ref BeatmapCharacteristicSegmentedControlController ____beatmapDifficultySegmentedControlController)
            {
                SiraEvents.InvokeLevelSelectionChange(__instance.selectedDifficultyBeatmap, ____beatmapDifficultySegmentedControlController.selectedBeatmapCharacteristic);
            }
        }

        [HarmonyPatch(typeof(StandardLevelDetailViewController), "DidDeactivate")]
        internal class PatchClose
        {
            internal static void Postfix()
            {
                SiraEvents.InvokeLevelSelectionChange(null, null);
            }
        }
    }
}