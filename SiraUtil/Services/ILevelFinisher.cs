using System;

namespace SiraUtil.Services
{
    /// <summary>
    /// A collection of events for when a level finishes. Avoid using this in the game scene.
    /// </summary>
    public interface ILevelFinisher
    {
        /// <summary>
        /// 
        /// </summary>
        event Action<StandardLevelScenesTransitionSetupDataSO, LevelCompletionResults> StandardLevelDidFinish;

        /// <summary>
        /// 
        /// </summary>
        event Action<MissionLevelScenesTransitionSetupDataSO, MissionCompletionResults> MissionLevelDidFinish;

        /// <summary>
        /// 
        /// </summary>
        event Action<MultiplayerLevelScenesTransitionSetupDataSO, MultiplayerResultsData> MultiplayerLevelDidFinish;

        /// <summary>
        /// 
        /// </summary>
        event Action<MultiplayerLevelScenesTransitionSetupDataSO, DisconnectedReason> LocalPlayerDidDisconnectFromMultiplayer;

        /// <summary>
        /// 
        /// </summary>
        event Action<DisconnectedReason> MultiplayerLevelDisconnected;

        /// <summary>
        /// 
        /// </summary>
        event Action<LevelCompletionResults> StandardLevelFinished;

        /// <summary>
        /// 
        /// </summary>
        event Action<MissionCompletionResults> MissionLevelFinished;

        /// <summary>
        /// 
        /// </summary>
        event Action<MultiplayerResultsData> MultiplayerLevelFinished;
    }
}