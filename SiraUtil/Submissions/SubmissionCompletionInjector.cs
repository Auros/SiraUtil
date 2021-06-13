using IPA.Loader;
using IPA.Utilities;
using Mono.Cecil;
using SiraUtil.Affinity;
using System;
using System.Linq;
using System.Reflection;
using Zenject;

namespace SiraUtil.Submissions
{
    internal class SubmissionCompletionInjector : IAffinity
    {
        private readonly bool _inMission;
        private readonly bool _inStandard;
        private readonly Submission _submission;
        private readonly SubmissionDataContainer _submissionDataContainer;

        public SubmissionCompletionInjector(Submission submission, SubmissionDataContainer submissionDataContainer, [InjectOptional] MissionGameplaySceneSetupData missionGameplaySceneSetupData, [InjectOptional] StandardGameplaySceneSetupData standardGameplaySceneSetupData)
        {
            _submission = submission;
            _submissionDataContainer = submissionDataContainer;
            _inMission = missionGameplaySceneSetupData != null;
            _inStandard = standardGameplaySceneSetupData != null;
            _submissionDataContainer.SSS(false);
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
            {
                if (_inStandard)
                {
                    _submissionDataContainer.SSS(!_submission.Activated);
                }

                __result = new SiraLevelCompletionResults(__result, !_submission.Activated);
            }
        }
    }
}