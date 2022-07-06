using HarmonyLib;

namespace SiraUtil.Submissions
{
    internal class SubmissionPatches
    {
        [HarmonyPatch(typeof(SoloFreePlayFlowCoordinator), "ProcessLevelCompletionResultsAfterLevelDidFinish")]
        internal class SoloFreePlay
        {
            [HarmonyPrefix]
            internal static void DynamicFinish(LevelCompletionResults levelCompletionResults, ref bool practice)
            {
                ForcePracticeIfScoresDisabled(levelCompletionResults, ref practice);
            }
        }

        [HarmonyPatch(typeof(PartyFreePlayFlowCoordinator), "ProcessLevelCompletionResultsAfterLevelDidFinish")]
        internal class Party
        {
            [HarmonyPrefix]
            internal static void DynamicFinish(LevelCompletionResults levelCompletionResults, ref bool practice)
            {
                ForcePracticeIfScoresDisabled(levelCompletionResults, ref practice);
            }
        }


        [HarmonyPatch(typeof(MultiplayerLevelCompletionResults), nameof(MultiplayerLevelCompletionResults.hasAnyResults), MethodType.Getter)]
        internal class Multi
        {
            [HarmonyPrefix]
            internal static bool DynamicFinish(MultiplayerLevelCompletionResults __instance, ref bool __result)
            {
                if (__instance.levelCompletionResults is SiraLevelCompletionResults siraLevelCompletionResults && !siraLevelCompletionResults.ShouldSubmitScores)
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }


        internal static void ForcePracticeIfScoresDisabled(LevelCompletionResults levelCompletionResults, ref bool practice)
        {
            if (levelCompletionResults is SiraLevelCompletionResults siraLevelCompletionResults && !siraLevelCompletionResults.ShouldSubmitScores)
                practice = true;
        }
    }
}