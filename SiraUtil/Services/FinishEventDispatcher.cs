using System;
using Zenject;
using SiraUtil.Events;
using SiraUtil.Interfaces;
using System.Collections.Generic;

namespace SiraUtil.Services
{
    /// <summary>
    /// A collection of events for when a level finishes. Avoid using this in the game scene.
    /// </summary>
    internal class FinishEventDispatcher : IInitializable, IDisposable, ILevelFinisher
    {
        public event Action<LevelCompletionResults> StandardLevelFinished;
        public event Action<MissionCompletionResults> MissionLevelFinished;
        public event Action<DisconnectedReason> MultiplayerLevelDisconnected;
        public event Action<StandardLevelScenesTransitionSetupDataSO, LevelCompletionResults> StandardLevelDidFinish;
        public event Action<MissionLevelScenesTransitionSetupDataSO, MissionCompletionResults> MissionLevelDidFinish;
        public event Action<LevelCompletionResults, Dictionary<string, LevelCompletionResults>> MultiplayerLevelFinished;
        public event Action<MultiplayerLevelScenesTransitionSetupDataSO, DisconnectedReason> LocalPlayerDidDisconnectFromMultiplayer;
        public event Action<MultiplayerLevelScenesTransitionSetupDataSO, LevelCompletionResults, Dictionary<string, LevelCompletionResults>> MultiplayerLevelDidFinish;

        public void Initialize()
        {
            SiraEvents.MissionFinish += SiraEvents_MissionFinish;
            SiraEvents.StandardFinished += SiraEvents_StandardFinished;
            SiraEvents.MultiplayerFinished += SiraEvents_MultiplayerFinished;
            SiraEvents.MultiplayerDisconnected += SiraEvents_MultiplayerDisconnected;
        }
        private void SiraEvents_MultiplayerDisconnected(MultiplayerLevelScenesTransitionSetupDataSO setupData, DisconnectedReason reason)
        {
            LocalPlayerDidDisconnectFromMultiplayer?.Invoke(setupData, reason);
            MultiplayerLevelDisconnected?.Invoke(reason);
        }

        private void SiraEvents_MissionFinish(MissionLevelScenesTransitionSetupDataSO setupData, MissionCompletionResults results)
        {
            MissionLevelDidFinish?.Invoke(setupData, results);
            MissionLevelFinished?.Invoke(results);
        }

        private void SiraEvents_StandardFinished(StandardLevelScenesTransitionSetupDataSO setupData, LevelCompletionResults results)
        {
            StandardLevelDidFinish?.Invoke(setupData, results);
            StandardLevelFinished?.Invoke(results);
        }

        private void SiraEvents_MultiplayerFinished(MultiplayerLevelScenesTransitionSetupDataSO setupData, LevelCompletionResults results, Dictionary<string, LevelCompletionResults> remotePlayerResults)
        {
            MultiplayerLevelDidFinish?.Invoke(setupData, results, remotePlayerResults);
            MultiplayerLevelFinished?.Invoke(results, remotePlayerResults);
        }

        public void Dispose()
        {
            SiraEvents.MissionFinish -= SiraEvents_MissionFinish;
            SiraEvents.StandardFinished -= SiraEvents_StandardFinished;
            SiraEvents.MultiplayerFinished -= SiraEvents_MultiplayerFinished;
            SiraEvents.MultiplayerDisconnected -= SiraEvents_MultiplayerDisconnected;
        }
    }
}