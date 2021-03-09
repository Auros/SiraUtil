using System.Collections.Generic;
using HarmonyLib;
using SiraUtil.Events;

namespace SiraUtil.Zenject.HarmonyPatches
{
    internal class MenuTransitionHelperReHelper
    {
        [HarmonyPatch(typeof(MenuTransitionsHelper), nameof(MenuTransitionsHelper.HandleMainGameSceneDidFinish))]
        internal class StartStandard
        {
            public static void Postfix(StandardLevelScenesTransitionSetupDataSO standardLevelScenesTransitionSetupData, LevelCompletionResults levelCompletionResults)
            {
                SiraEvents.SendStandardEvent(standardLevelScenesTransitionSetupData, levelCompletionResults);
            }
        }

        [HarmonyPatch(typeof(MenuTransitionsHelper), nameof(MenuTransitionsHelper.HandleMissionLevelSceneDidFinish))]
        internal class StartMission
        {
            public static void Postfix(MissionLevelScenesTransitionSetupDataSO missionLevelScenesTransitionSetupData, MissionCompletionResults missionCompletionResults)
            {
                SiraEvents.SendMissionEvent(missionLevelScenesTransitionSetupData, missionCompletionResults);
            }
        }

        [HarmonyPatch(typeof(MenuTransitionsHelper), nameof(MenuTransitionsHelper.HandleMultiplayerLevelDidFinish))]
        internal class StartMultiplayer
        {
            public static void Postfix(MultiplayerLevelScenesTransitionSetupDataSO multiplayerLevelScenesTransitionSetupData, MultiplayerResultsData multiplayerResultsData)
            {
                var dict = new Dictionary<string, LevelCompletionResults>();
                foreach (var player in multiplayerResultsData.otherPlayersData)
                {
                    dict.Add(player.connectedPlayer.userId, player.levelCompletionResults);
                }
                SiraEvents.SendMultiplayerEvent(multiplayerLevelScenesTransitionSetupData, multiplayerResultsData.localPlayerResultData.levelCompletionResults, dict);
            }
        }

        [HarmonyPatch(typeof(MenuTransitionsHelper), nameof(MenuTransitionsHelper.HandleMultiplayerLevelDidDisconnect))]
        internal class MultiplayerDisconnect
        {
            public static void Postfix(MultiplayerLevelScenesTransitionSetupDataSO multiplayerLevelScenesTransitionSetupData, DisconnectedReason disconnectedReason)
            {
                SiraEvents.SendDisconnectEvent(multiplayerLevelScenesTransitionSetupData, disconnectedReason);
            }
        }
    }
}