﻿using HarmonyLib;
using IPA;
using IPA.Config.Stores;
using IPA.Loader;
using IPA.Utilities.Async;
#if DEBUG
using SiraUtil.Affinity.Harmony.Generator;
#endif
using SiraUtil.Attributes;
using SiraUtil.Installers;
using SiraUtil.Tools.FPFC;
using SiraUtil.Tweaks;
using SiraUtil.Zenject;
using System.Reflection;
using System.Threading.Tasks;
using Zenject;
using Conf = IPA.Config.Config;
using IPALogger = IPA.Logging.Logger;

namespace SiraUtil
{
    [Slog, Plugin(RuntimeOptions.DynamicInit)]
    internal class Plugin
    {
        private readonly Harmony _harmony;
        private readonly PluginMetadata _pluginMetadata;
        private readonly ZenjectManager _zenjectManager;
        public static IPALogger Log { get; private set; } = null!;
        public const string ID = "dev.auros.sirautil";

        [Init]
        public Plugin(Conf conf, IPALogger logger, PluginMetadata pluginMetadata)
        {
            Config config = conf.Generated<Config>();
            config.Version = pluginMetadata.HVersion;

            Log = logger;
            _harmony = new Harmony(ID);
            _pluginMetadata = pluginMetadata;
            _zenjectManager = new ZenjectManager();

            // Adds the Zenjector type to BSIPA's Init Injection system so mods can receive it in their [Init] parameters.
            PluginInitInjector.AddInjector(typeof(Zenjector), ConstructZenjector);

            Zenjector zenjector = (ConstructZenjector(null!, null!, pluginMetadata) as Zenjector)!;
            zenjector.Install<SiraStandardCampaignMultiplayerInstaller>(Location.StandardPlayer | Location.CampaignPlayer | Location.MultiPlayer);
            zenjector.Install<SiraFullFPFCInstaller>(Location.Menu | Location.Player | Location.InactiveMultiPlayer | Location.Tutorial);
            zenjector.Install<SiraAllPlayersFPFCInstaller>(Location.Player | Location.InactiveMultiPlayer | Location.Tutorial);
            zenjector.Install<SiraInitializationInstaller>(Location.App, _zenjectManager, zenjector);
            zenjector.Install<SiraGameplayInstaller>(Location.Player | Location.Tutorial);
            zenjector.Install<SiraSingleplayerInstaller>(Location.Singleplayer);
            zenjector.Install<SiraMultiplayerInstaller>(Location.MultiPlayer);
            zenjector.Install<SiraSettingsInstaller>(Location.App, config);
            zenjector.Install<SiraGameCoreInstaller>(Location.GameCore);
            zenjector.Install<SiraMenuInstaller>(Location.Menu);

            zenjector.Install<SiraCreditsInstaller, GameObjectContext>("Credits");
            zenjector.Install<SiraFullFPFCInstaller, GameObjectContext>("Credits");

            zenjector.UseMetadataBinder<Plugin>();
            zenjector.UseLogger(logger);
            zenjector.UseHttpService();
        }

        [OnEnable]
        public void OnEnable()
        {
            _harmony.PatchAll(_pluginMetadata.Assembly);
            _zenjectManager.Enable();
        }

        [OnDisable]
        public void OnDisable()
        {
            _zenjectManager.Disable();

            // delay so DisposableManager is able to run before we unpatch on shutdown
            UnityMainThreadTaskScheduler.Factory.StartNew(() => _harmony.UnpatchSelf()).ContinueWith((task) => Log.Error($"Failed to unpatch\n{task.Exception}"), TaskContinuationOptions.OnlyOnFaulted);

#if DEBUG
            DynamicHarmonyPatchGenerator.Save();
#endif
        }

        private object? ConstructZenjector(object? previous, ParameterInfo _, PluginMetadata meta)
        {
            if (previous is not null)
                return previous;

            Zenjector zenjector = new(meta);
            _zenjectManager.Add(zenjector);
            return zenjector;
        }
    }
}