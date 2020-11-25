using IPA;
using Zenject;
using IPA.Loader;
using HarmonyLib;
using UnityEngine;
using System.Linq;
using IPA.Utilities;
using SiraUtil.Sabers;
using SiraUtil.Zenject;
using IPA.Config.Stores;
using System.Reflection;
using SiraUtil.Services;
using System.Collections;
using SiraUtil.Installers;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;

namespace SiraUtil
{
    /// <summary>
    /// The main Plugin class for SiraUtil.
    /// </summary>
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static IPALogger Log { get; set; }
        internal static Harmony Harmony { get; set; }

        private readonly ZenjectManager _zenjectManager;

        /// <summary>
        /// The initialization/entry point of SiraUtil.
        /// </summary>
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
            zenjector.OnMenu<SiraMenuInstaller>();
            zenjector.OnGame<SiraSaberInstaller>();

            zenjector.OnGame<SiraSaberEffectInstaller>()
                .Mutate<SaberBurnMarkArea>(InstallSaberArea)
                .Mutate<SaberBurnMarkSparkles>(InstallSaberSparkles)
                .Mutate<ObstacleSaberSparkleEffectManager>(InstallObstacleEffectManager)
                .Expose<SaberClashEffect>()
                .ShortCircuitForMultiplayer();

            zenjector.OnGame<SiraGameLevelInstaller>()
                .Mutate<PrepareLevelCompletionResults>((ctx, completionResults) =>
                {
                    var binding = completionResults.GetComponent<ZenjectBinding>();
                    var siraCompletionResults = completionResults.Upgrade<PrepareLevelCompletionResults, Submission.SiraPrepareLevelCompletionResults>();
                    binding.SetField("_ifNotBound", true);
                    ctx.Container.QueueForInject(siraCompletionResults);
                    ctx.Container.Unbind(typeof(PrepareLevelCompletionResults));
                    ctx.Container.Bind<PrepareLevelCompletionResults>().To<Submission.SiraPrepareLevelCompletionResults>().FromInstance(siraCompletionResults).AsCached();
                }).OnlyForStandard();

            zenjector.OnGame<SiraGameInstaller>(true).ShortCircuitForMultiplayer();

            // multi specific for toni
            zenjector.OnGame<SiraMultiGameInstaller>(false)
                .Mutate<SaberBurnMarkArea>(InstallSaberArea)
                .Mutate<SaberBurnMarkSparkles>(InstallSaberSparkles)
                .Mutate<ObstacleSaberSparkleEffectManager>(InstallObstacleEffectManager)
                .Expose<SaberClashEffect>().OnlyForMultiplayer();
        }

        /// <summary>
        /// The method called when SiraUtil is enabling.
        /// </summary>
        [OnEnable]
        public void OnEnable()
        {
            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
            Harmony.PatchAll(Assembly.GetExecutingAssembly());
            //UnityRequiredAttributeUndecorator.Patch(Harmony);
        }

        private void SceneManager_activeSceneChanged(Scene oldScene, Scene newScene)
        {
            Plugin.Log.Info($"{oldScene.name} -> {newScene.name}");
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

        /// <summary>
        /// The method called when SiraUtil is disabling.
        /// </summary>
        [OnDisable]
        public void OnDisable()
        {
            SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
            //UnityRequiredAttributeUndecorator.Unpatch(Harmony);
            Harmony.UnpatchAll("dev.auros.sirautil");
        }

        private void InstallSaberArea(MutationContext ctx, SaberBurnMarkArea burnArea)
        {
            var siraBurnArea = burnArea.Upgrade<SaberBurnMarkArea, SiraSaberBurnMarkArea>();
            ctx.Container.QueueForInject(siraBurnArea);
            ctx.Container.Bind<SaberBurnMarkArea>().To<SiraSaberBurnMarkArea>().FromInstance(siraBurnArea).AsCached();
        }

        private void InstallSaberSparkles(MutationContext ctx, SaberBurnMarkSparkles burnSparkles)
        {
            var siraBurnSparkles = burnSparkles.Upgrade<SaberBurnMarkSparkles, SiraSaberBurnMarkSparkles>();
            ctx.Container.QueueForInject(siraBurnSparkles);
            ctx.Container.Bind<SaberBurnMarkSparkles>().To<SiraSaberBurnMarkSparkles>().FromInstance(siraBurnSparkles).AsCached();
        }

        private void InstallObstacleEffectManager(MutationContext ctx, ObstacleSaberSparkleEffectManager obstacleSparkles)
        {
            var siraObstacleSparkles = obstacleSparkles.Upgrade<ObstacleSaberSparkleEffectManager, SiraObstacleSaberSparkleEffectManager>();
            Object.Destroy(obstacleSparkles);
            ctx.Container.QueueForInject(siraObstacleSparkles);
            ctx.Container.Bind<ObstacleSaberSparkleEffectManager>().To<SiraObstacleSaberSparkleEffectManager>().FromInstance(siraObstacleSparkles).AsCached();
        }
    }
}