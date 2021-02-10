using System;
using Zenject;
using System.Collections.Generic;

namespace SiraUtil.Events
{
    /// <summary>
    /// A collection of events used by SIRA
    /// </summary>
    public class SiraEvents
    {
        internal static event EventHandler<SceneContextInstalledArgs> ContextInstalling;
        internal static event Action<StandardLevelScenesTransitionSetupDataSO, LevelCompletionResults> StandardFinished;
        internal static event Action<MissionLevelScenesTransitionSetupDataSO, MissionCompletionResults> MissionFinish;
        internal static event Action<MultiplayerLevelScenesTransitionSetupDataSO, LevelCompletionResults, Dictionary<string, LevelCompletionResults>> MultiplayerFinished;
        internal static event Action<MultiplayerLevelScenesTransitionSetupDataSO, DisconnectedReason> MultiplayerDisconnected;

        internal static void SendInstallEvent(string[] names, Context context, DiContainer container, List<SceneDecoratorContext> decorators,  string mode = null, string transition = null, string midscene = null)
        {
            var args = new SceneContextInstalledArgs(names, container, new ModeInfo(transition, mode, midscene), decorators);
            ContextInstalling.Invoke(context, args);
        }

        internal static void SendStandardEvent(StandardLevelScenesTransitionSetupDataSO data, LevelCompletionResults results)
        {
            StandardFinished?.Invoke(data, results);
        }

        internal static void SendMissionEvent(MissionLevelScenesTransitionSetupDataSO data, MissionCompletionResults results)
        {
            MissionFinish?.Invoke(data, results);
        }

        internal static void SendMultiplayerEvent(MultiplayerLevelScenesTransitionSetupDataSO data, LevelCompletionResults results, Dictionary<string, LevelCompletionResults> remotePlayerResults)
        {
            MultiplayerFinished?.Invoke(data, results, remotePlayerResults);
        }

        internal static void SendDisconnectEvent(MultiplayerLevelScenesTransitionSetupDataSO data, DisconnectedReason reason)
        {
            MultiplayerDisconnected?.Invoke(data, reason);
        }

        internal class SceneContextInstalledArgs : EventArgs
        {
            public string[] Names { get; }
            public ModeInfo ModeInfo { get; }
            public DiContainer Container { get; }
            public List<SceneDecoratorContext> Decorators { get; }

            public SceneContextInstalledArgs(string[] names, DiContainer container, ModeInfo modeInfo, List<SceneDecoratorContext> decorators)
            {
                Names = names;
                ModeInfo = modeInfo;
                Container = container;
                Decorators = decorators;
            }
        }

        internal class ModeInfo
        {
            public string Gamemode { get; }
            public string Transition { get; }
            public string MidScene { get; }

            public ModeInfo(string gamemode, string transition, string midScene)
            {
                Gamemode = gamemode ?? "";
                Transition = transition ?? "";
                MidScene = midScene ?? "";
            }
        }
    }
}