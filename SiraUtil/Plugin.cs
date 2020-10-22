using IPA;
using IPA.Loader;
using HarmonyLib;
using UnityEngine;
using System.Linq;
using SiraUtil.Sabers;
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

        private readonly ZenjectManager _zenjectManager;

        [Init]
        public Plugin(IPA.Config.Config conf, IPALogger logger, PluginMetadata metadata)
        {
            Log = logger;
            Config config = conf.Generated<Config>();
            Harmony = new Harmony("dev.auros.sirautil");

			// Set Config Verison
			config.Version = metadata.Version;

            // Setup Zenjector
            _zenjectManager = new ZenjectManager();
            PluginInitInjector.AddInjector(typeof(Zenjector), (prev, __, meta) =>
            {
                if (prev != null)
                {
                    return prev;
                }
                var zen = new Zenjector(meta.Id);
                _zenjectManager.Add(zen);
                return zen;
            });

            // Setup Own Zenject Stuff
            var zenjector = new Zenjector("SiraUtil");
            _zenjectManager.Add(zenjector);

            zenjector.OnApp<SiraInstaller>().WithParameters(config);
			zenjector.OnGame<SiraSaberInstaller>();

			zenjector.OnGame<SiraSaberEffectInstaller>()
				.Mutate<SaberBurnMarkArea>((ctx, obj) =>
				{
					var burnArea = obj as SaberBurnMarkArea;
					// Override (or modify) the component BEFORE it's installed
					var siraBurnArea = burnArea.gameObject.AddComponent<SiraSaberBurnMarkArea>();
					ctx.Container.QueueForInject(siraBurnArea);
					ctx.Container.Bind<SaberBurnMarkArea>().To<SiraSaberBurnMarkArea>().FromInstance(siraBurnArea).AsCached();
				})
				.Mutate<SaberBurnMarkSparkles>((ctx, obj) =>
				{
					var burnSparkles = obj as SaberBurnMarkSparkles;
					var siraBurnSparkles = burnSparkles.gameObject.AddComponent<SiraSaberBurnMarkSparkles>();
					ctx.Container.QueueForInject(siraBurnSparkles);
					ctx.Container.Bind<SaberBurnMarkSparkles>().To<SiraSaberBurnMarkSparkles>().FromInstance(siraBurnSparkles).AsCached();
				})
				.Mutate<ObstacleSaberSparkleEffectManager>((ctx, obj) =>
				{
					var obstacleSparkles = obj as ObstacleSaberSparkleEffectManager;
					var siraObstacleSparkles = obstacleSparkles.gameObject.AddComponent<SiraObstacleSaberSparkleEffectManager>();
					Object.Destroy(obstacleSparkles);
					ctx.Container.QueueForInject(siraObstacleSparkles);
					ctx.Container.Bind<ObstacleSaberSparkleEffectManager>().To<SiraObstacleSaberSparkleEffectManager>().FromInstance(siraObstacleSparkles).AsCached();
				})
				.ShortCircuitForMultiplayer();

			zenjector.OnGame<SiraGameInstaller>().ShortCircuitForMultiplayer();
        }

        [OnEnable]
        public void OnEnable()
        {
            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
            Harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        private void SceneManager_activeSceneChanged(Scene oldScene, Scene newScene)
        {
			//Plugin.Log.Info($"{oldScene.name} -> {newScene.name}");
            if (newScene.name == "MenuViewControllers" && !ZenjectManager.ProjectContextWentOff)
            {
                SharedCoroutineStarter.instance.StartCoroutine(BruteForceRestart());
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
            SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
            Harmony.UnpatchAll("dev.auros.sirautil");
        }
    }
}