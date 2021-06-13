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

        [HarmonyPatch(typeof(ArcadeFlowCoordinator), "ProcessLevelCompletionResultsAfterLevelDidFinish")]
        internal class Arcade
        {
            [HarmonyPrefix]
            internal static void DynamicFinish(LevelCompletionResults levelCompletionResults, ref bool practice)
            {
                ForcePracticeIfScoresDisabled(levelCompletionResults, ref practice);
            }
        }

        internal static void ForcePracticeIfScoresDisabled(LevelCompletionResults levelCompletionResults, ref bool practice)
        {
            if (levelCompletionResults is SiraLevelCompletionResults siraLevelCompletionResults && !siraLevelCompletionResults.ShouldSubmitScores)
                practice = true;
        }
    }
}