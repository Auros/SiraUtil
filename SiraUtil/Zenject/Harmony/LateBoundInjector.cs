using HarmonyLib;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SiraUtil.Zenject.Harmony
{
    [HarmonyPatch(typeof(GameScenesManager), "ReplaceScenes")]
    internal class LateBoundInjector
    {
        private const string _initialSceneName = "PCInit";
        private static SiraPCScenesTransitionSetupDataSO _restartTransitionData = null!;

        [HarmonyPrefix]
        public static bool Prefix(GameScenesManager __instance)
        {
            if (!ZenjectManager.InitialSceneConstructionRegistered)
            {
                if (_restartTransitionData == null)
                {
                    _restartTransitionData = ScriptableObject.CreateInstance<SiraPCScenesTransitionSetupDataSO>();
                }
                __instance.ClearAndOpenScenes(_restartTransitionData, finishCallback: (_) =>
                {
                    PCAppInit pcAppInit = SceneManager.GetSceneByName(_initialSceneName).GetRootGameObjects()[0].GetComponent<PCAppInit>();
                    pcAppInit.TransitionToNextScene();
                });
                return false;
            }
            return true;
        }

        internal class SiraPCScenesTransitionSetupDataSO : ScenesTransitionSetupDataSO
        {
            protected override void OnEnable()
            {
                var si = CreateInstance<SceneInfo>();
                si._sceneName = _initialSceneName;
                Init(new SceneInfo[] { si }, Array.Empty<SceneSetupData>());
                base.OnEnable();
            }
        }
    }
}