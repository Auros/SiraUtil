using IPA;
using HarmonyLib;
using System.Reflection;
using IPALogger = IPA.Logging.Logger;
using SiraUtil.Sabers;
using UnityEngine;

namespace SiraUtil
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; set; }

        private readonly Harmony _harmony;

        [Init]
        public Plugin(IPALogger logger)
        {
            Log = logger;
            Instance = this;

            _harmony = new Harmony("dev.auros.sirautil");
        }

        [OnEnable]
        public void OnEnable()
        {
            _harmony?.PatchAll(Assembly.GetExecutingAssembly());

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneManager_sceneLoaded;

            ExtraSabers.Touch();
        }

        //EmptyTransition TO MenuViewControllers

        private void SceneManager_sceneLoaded(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.LoadSceneMode arg1)
        {
            Log.Info("SCENE LOADED: " + arg0.name);

            /*if (arg0.name == "GameCore")
            {
                Log.Info("detected gamecore: creating saber test");
                new GameObject().AddComponent<SaberTest>();
            }*/
        }

        private void SceneManager_activeSceneChanged(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.Scene arg1)
        {
            Log.Info($"CHANGED FROM {arg0.name} TO {arg1.name}");
        }


        [OnDisable]
        public void OnDisable()
        {
            ExtraSabers.Untouch();

            _harmony?.UnpatchAll();
        }


    }
}