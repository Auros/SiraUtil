using IPA.Utilities;
using SiraUtil.Affinity;
using Zenject;

namespace SiraUtil.Submissions
{
    internal class SubmissionCompletionInjector : IAffinity
    {
        private readonly bool _inMission;
        private readonly bool _inStandard;
        private readonly bool _inMultiplayer;
        private readonly Submission _submission;
        private readonly SubmissionDataContainer _submissionDataContainer;

        public SubmissionCompletionInjector(Submission submission, SubmissionDataContainer submissionDataContainer, [InjectOptional] MissionGameplaySceneSetupData missionGameplaySceneSetupData, [InjectOptional] StandardGameplaySceneSetupData standardGameplaySceneSetupData, [InjectOptional] MultiplayerLevelSceneSetupData multiplayerLevelSceneSetupData)
        {
            _submission = submission;
            _submissionDataContainer = submissionDataContainer;
            _inMission = missionGameplaySceneSetupData != null;
            _inStandard = standardGameplaySceneSetupData != null;
            _inMultiplayer = multiplayerLevelSceneSetupData != null;
            _submissionDataContainer.SSS(true);
        }

        [AffinityPatch(typeof(PrepareLevelCompletionResults), nameof(PrepareLevelCompletionResults.FillLevelCompletionResults))]
        private void StandardResultsPrepared(ref LevelCompletionResults __result)
        {
            if (!(_inStandard || _inMission || _inMultiplayer))
            {
                return;
            }

            if (_submission.Activated)
            {
                if (_inStandard || _inMultiplayer)
                {
                    _submissionDataContainer.SSS(false);
                }
                else if (_inMission)
                {
                    // TODO: These are both readonly fields. Figure out if there's a way to get around that.
                    __result.SetField(nameof(LevelCompletionResults.levelEndStateType), LevelCompletionResults.LevelEndStateType.Failed);
                    __result.SetField(nameof(LevelCompletionResults.levelEndAction), LevelCompletionResults.LevelEndAction.None);
                }
                __result = new SiraLevelCompletionResults(__result, !_submission.Activated);
            }
        }
    }
}