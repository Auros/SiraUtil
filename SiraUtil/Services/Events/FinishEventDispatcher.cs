using SiraUtil.Affinity;
using System;

namespace SiraUtil.Services.Events
{
    /// <summary>
    /// A collection of events for when a level finishes. Avoid using this in the game scene.
    /// </summary>
    internal class FinishEventDispatcher : ILevelFinisher, IAffinity
    {
        public event Action<LevelCompletionResults>? StandardLevelFinished;
        public event Action<MissionCompletionResults>? MissionLevelFinished;
        public event Action<DisconnectedReason>? MultiplayerLevelDisconnected;
        public event Action<MultiplayerResultsData>? MultiplayerLevelFinished;
        public event Action<StandardLevelScenesTransitionSetupDataSO, LevelCompletionResults>? StandardLevelDidFinish;
        public event Action<MissionLevelScenesTransitionSetupDataSO, MissionCompletionResults>? MissionLevelDidFinish;
        public event Action<MultiplayerLevelScenesTransitionSetupDataSO, DisconnectedReason>? LocalPlayerDidDisconnectFromMultiplayer;
        public event Action<MultiplayerLevelScenesTransitionSetupDataSO, MultiplayerResultsData>? MultiplayerLevelDidFinish;

        [AffinityPrefix]
        [AffinityPatch(typeof(MenuTransitionsHelper), nameof(MenuTransitionsHelper.HandleMultiplayerLevelDidDisconnect))]
        private void SiraEvents_MultiplayerDisconnected(MultiplayerLevelScenesTransitionSetupDataSO multiplayerLevelScenesTransitionSetupData, DisconnectedReason disconnectedReason)
        {
            LocalPlayerDidDisconnectFromMultiplayer?.Invoke(multiplayerLevelScenesTransitionSetupData, disconnectedReason);
            MultiplayerLevelDisconnected?.Invoke(disconnectedReason);
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(MenuTransitionsHelper), nameof(MenuTransitionsHelper.HandleMissionLevelSceneDidFinish))]
        private void SiraEvents_MissionFinish(MissionLevelScenesTransitionSetupDataSO missionLevelScenesTransitionSetupData, MissionCompletionResults missionCompletionResults)
        {
            MissionLevelDidFinish?.Invoke(missionLevelScenesTransitionSetupData, missionCompletionResults);
            MissionLevelFinished?.Invoke(missionCompletionResults);
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(MenuTransitionsHelper), nameof(MenuTransitionsHelper.HandleMainGameSceneDidFinish))]
        private void SiraEvents_StandardFinished(StandardLevelScenesTransitionSetupDataSO standardLevelScenesTransitionSetupData, LevelCompletionResults levelCompletionResults)
        {
            StandardLevelDidFinish?.Invoke(standardLevelScenesTransitionSetupData, levelCompletionResults);
            StandardLevelFinished?.Invoke(levelCompletionResults);
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(MenuTransitionsHelper), nameof(MenuTransitionsHelper.HandleMultiplayerLevelDidFinish))]
        private void SiraEvents_MultiplayerFinished(MultiplayerLevelScenesTransitionSetupDataSO multiplayerLevelScenesTransitionSetupData, MultiplayerResultsData multiplayerResultsData)
        {
            MultiplayerLevelDidFinish?.Invoke(multiplayerLevelScenesTransitionSetupData, multiplayerResultsData);
            MultiplayerLevelFinished?.Invoke(multiplayerResultsData);
        }
    }
}