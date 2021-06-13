using IPA.Utilities;
using SiraUtil.Affinity;
using Zenject;

namespace SiraUtil.Submissions
{
    internal class SubmissionCompletionInjector : IAffinity
    {
        private readonly bool _inMission;
        private readonly bool _inStandard;
        private readonly Submission _submission;

        public SubmissionCompletionInjector(Submission submission, [InjectOptional] MissionGameplaySceneSetupData missionGameplaySceneSetupData, [InjectOptional] StandardGameplaySceneSetupData standardGameplaySceneSetupData)
        {
            _submission = submission;
            _inMission = missionGameplaySceneSetupData != null;
            _inStandard = standardGameplaySceneSetupData != null;
        }

        [AffinityPatch(typeof(PrepareLevelCompletionResults), nameof(PrepareLevelCompletionResults.FillLevelCompletionResults))]
        private void StandardResultsPrepared(ref LevelCompletionResults __result)
        {
            if (!(_inStandard || _inMission))
                return;

            if (_inMission)
            {
                __result.SetField("levelEndStateType", LevelCompletionResults.LevelEndStateType.Failed);
                __result.SetField("levelEndAction", LevelCompletionResults.LevelEndAction.None);
            }

            if (_submission.Activated)
                __result = new SiraLevelCompletionResults(__result, !_submission.Activated);
        }
    }
}