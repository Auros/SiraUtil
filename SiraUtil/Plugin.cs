using IPA;
using HarmonyLib;
using System.Reflection;
using IPALogger = IPA.Logging.Logger;
using UnityEngine.SceneManagement;
using SiraUtil.Zenject;
using System.Collections;
using UnityEngine;
using System.Linq;

namespace SiraUtil
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; set; }

        public Harmony Harmony { get; }

        [Init]
        public Plugin(IPALogger logger)
        {
            Log = logger;
            Instance = this;
            Harmony = new Harmony("dev.auros.sirautil");
        }

        [OnEnable]
        public void OnEnable()
        {
            Harmony.PatchAll(Assembly.GetExecutingAssembly());

            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        }

        private void SceneManager_activeSceneChanged(Scene oldScene, Scene newScene)
        {
            if (newScene.name == "MenuViewControllers")
            {
                if (Installer.NotAllAppInstallersAreInstalled)
                {
                    SharedCoroutineStarter.instance.StartCoroutine(BruteForceRestart());
                }
            }
        }

        private IEnumerator BruteForceRestart()
        {
            yield return new WaitForSecondsRealtime(1f);
            Resources.FindObjectsOfTypeAll<MenuTransitionsHelper>().FirstOrDefault()?.RestartGame();
        }

        [OnDisable]
        public void OnDisable()
        {
            Harmony.UnpatchAll();
        }
    }
}