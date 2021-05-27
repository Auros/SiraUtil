using System;
using HarmonyLib;
using IPA.Utilities;
using UnityEngine;

namespace SiraUtil.Zenject.HarmonyPatches
{
    [HarmonyPatch(typeof(GameScenesManager), "ReplaceScenes")]
    internal class ContextResetter
    {
        private static SiraPCScenesTransitionSetupDataSO _restartTransitionData;

        [HarmonyPrefix]
        public static bool Prefix(ref GameScenesManager __instance, ScenesTransitionSetupDataSO scenesTransitionSetupData)
        {
            if (!ZenjectManager.ProjectContextWentOff)
            {
                if (_restartTransitionData == null)
                {
                    _restartTransitionData = ScriptableObject.CreateInstance<SiraPCScenesTransitionSetupDataSO>();
                }
                __instance.ClearAndOpenScenes(_restartTransitionData, finishCallback: (_) =>
                {
                    var app = Resources.FindObjectsOfTypeAll<PCAppInit>()[0];
                    app.InvokeMethod<object, PCAppInit>("TransitionToNextScene");
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
                si.SetField("_sceneName", "PCInit");
                Init(new SceneInfo[] { si }, Array.Empty<SceneSetupData>());
                base.OnEnable();
            }
        }
    }
}