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
        public event Action<StandardLevelScenesTransitionSetupData, LevelCompletionResults>? StandardLevelDidFinish;
        public event Action<MissionLevelScenesTransitionSetupData, MissionCompletionResults>? MissionLevelDidFinish;
        public event Action<MultiplayerLevelScenesTransitionSetupData, DisconnectedReason>? LocalPlayerDidDisconnectFromMultiplayer;
        public event Action<MultiplayerLevelScenesTransitionSetupData, MultiplayerResultsData>? MultiplayerLevelDidFinish;

        [AffinityPrefix]
        [AffinityPatch(typeof(MenuTransitionsHelper), nameof(MenuTransitionsHelper.HandleMultiplayerLevelDidDisconnect))]
        private void SiraEvents_MultiplayerDisconnected(MultiplayerLevelScenesTransitionSetupData multiplayerLevelScenesTransitionSetupData, DisconnectedReason disconnectedReason)
        {
            LocalPlayerDidDisconnectFromMultiplayer?.Invoke(multiplayerLevelScenesTransitionSetupData, disconnectedReason);
            MultiplayerLevelDisconnected?.Invoke(disconnectedReason);
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(MenuTransitionsHelper), nameof(MenuTransitionsHelper.HandleMissionLevelSceneDidFinish))]
        private void SiraEvents_MissionFinish(MissionLevelScenesTransitionSetupData missionLevelScenesTransitionSetupData, MissionCompletionResults missionCompletionResults)
        {
            MissionLevelDidFinish?.Invoke(missionLevelScenesTransitionSetupData, missionCompletionResults);
            MissionLevelFinished?.Invoke(missionCompletionResults);
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(MenuTransitionsHelper), nameof(MenuTransitionsHelper.HandleMainGameSceneDidFinish))]
        private void SiraEvents_StandardFinished(StandardLevelScenesTransitionSetupData standardLevelScenesTransitionSetupData, LevelCompletionResults levelCompletionResults)
        {
            StandardLevelDidFinish?.Invoke(standardLevelScenesTransitionSetupData, levelCompletionResults);
            StandardLevelFinished?.Invoke(levelCompletionResults);
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(MenuTransitionsHelper), nameof(MenuTransitionsHelper.HandleMultiplayerLevelDidFinish))]
        private void SiraEvents_MultiplayerFinished(MultiplayerLevelScenesTransitionSetupData multiplayerLevelScenesTransitionSetupData, MultiplayerResultsData multiplayerResultsData)
        {
            MultiplayerLevelDidFinish?.Invoke(multiplayerLevelScenesTransitionSetupData, multiplayerResultsData);
            MultiplayerLevelFinished?.Invoke(multiplayerResultsData);
        }
    }
}