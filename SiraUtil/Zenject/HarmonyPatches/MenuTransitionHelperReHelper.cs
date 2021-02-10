using HarmonyLib;
using SiraUtil.Events;
using System.Collections.Generic;

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
            public static void Postfix(MultiplayerLevelScenesTransitionSetupDataSO multiplayerLevelScenesTransitionSetupData, LevelCompletionResults levelCompletionResults, Dictionary<string, LevelCompletionResults> otherPlayersCompletionResults)
            {
                SiraEvents.SendMultiplayerEvent(multiplayerLevelScenesTransitionSetupData, levelCompletionResults, otherPlayersCompletionResults);
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