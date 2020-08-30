using IPA;
using HarmonyLib;
using UnityEngine;
using System.Linq;
using SiraUtil.Zenject;
using IPA.Config.Stores;
using System.Reflection;
using System.Collections;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;

namespace SiraUtil
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static IPALogger Log { get; set; }
        internal static Harmony Harmony { get; set; }

        private readonly SiraInstallerInit _siraInstallerInit;

        [Init]
        public Plugin(IPA.Config.Config conf, IPALogger logger)
        {
            Log = logger;
            Config config = conf.Generated<Config>();
            Harmony = new Harmony("dev.auros.sirautil");
            _siraInstallerInit = new SiraInstallerInit(config);
        }

        [OnEnable]
        public void OnEnable()
        {
            Harmony.PatchAll(Assembly.GetExecutingAssembly());
            Installer.RegisterAppInstaller(_siraInstallerInit);
            Installer.RegisterGameCoreInstaller<SiraGameInstaller>();
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
            Installer.UnregisterAppInstaller(_siraInstallerInit);
            Installer.UnregisterGameCoreInstaller<SiraGameInstaller>();
            SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        }
    }
}