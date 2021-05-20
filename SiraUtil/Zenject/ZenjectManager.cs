using IPA.Loader;
using SiraUtil.Affinity;
using SiraUtil.Zenject.Internal;
using SiraUtil.Zenject.Internal.Exposers;
using SiraUtil.Zenject.Internal.Instructors;
using SiraUtil.Zenject.Internal.Mutators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace SiraUtil.Zenject
{
    internal class ZenjectManager
    {
        public static bool InitialSceneConstructionRegistered { get; private set; }
        private const string _initialContextName = "AppCoreSceneContext";
        private const string _initialSceneName = "PCInit";

        private readonly ExposerManager _exposerManager = new();
        private readonly MutatorManager _mutatorManager = new();
        private readonly HashSet<ZenjectorDatum> _zenjectors = new();
        private readonly InstructorManager _instructorManager = new();

        internal void Add(Zenjector zenjector) => _zenjectors.Add(new ZenjectorDatum(zenjector));
        
        private void PluginManager_PluginEnabled(PluginMetadata plugin, bool _)
        {
            // Enables the zenjector of a plugin being enabled.
            ZenjectorDatum? datum = _zenjectors.FirstOrDefault(zen => zen.Zenjector.Metadata == plugin);
            if (datum is not null)
                datum.Enabled = true;
        }

        private void PluginManager_PluginDisabled(PluginMetadata plugin, bool _)
        {
            // Disables the zenjector of a plugin being disabled.
            ZenjectorDatum? datum = _zenjectors.FirstOrDefault(zen => zen.Zenjector.Metadata == plugin);
            if (datum is not null)
                datum.Enabled = false;
        }

        public void Enable()
        {
            PluginManager.PluginEnabled += PluginManager_PluginEnabled;
            PluginManager.PluginDisabled += PluginManager_PluginDisabled;
            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
            Harmony.ContextDecorator.ContextInstalling += ContextDecorator_ContextInstalling;

            // This will set the default state for every Zenjector when SiraUtil enables.
            foreach (var zenDatum in _zenjectors)
                zenDatum.Enabled = PluginManager.GetPluginFromId(zenDatum.Zenjector.Metadata.Id) != null;
        }

        private void ContextDecorator_ContextInstalling(Context mainContext, IEnumerable<ContextBinding> installerBindings)
        {
            if (mainContext.name == _initialContextName)
                InitialSceneConstructionRegistered = true;

            if (!InitialSceneConstructionRegistered)
                return;
            
            IEnumerable<MonoBehaviour>? injectableList = null;
            bool isDecorator = mainContext is SceneDecoratorContext;

            foreach (var zenDatum in _zenjectors)
            {
                if (!zenDatum.Enabled)
                    continue;

                Zenjector zenjector = zenDatum.Zenjector;

                // Mutate and expose anything marked to be mutated and exposed.
                if (isDecorator)
                {
                    foreach (var set in zenjector.MutateSets)
                    {
                        _mutatorManager.Install(set, mainContext, ref injectableList);
                    }
                    foreach (var set in zenjector.ExposeSets)
                    {
                        _exposerManager.Install(set, mainContext, ref injectableList);
                    }
                }

                // Install every normal install set.
                foreach (var set in zenjector.InstallSets)
                {
                    foreach (var binding in installerBindings)
                    {
                        if (set.installFilter.ShouldInstall(binding))
                        {
                            Plugin.Log.Debug($"Installing: {set.installerType.Name} onto {binding.installerType}");
                            IInstructor? instructor = _instructorManager.InstructorForSet(set);
                            if (instructor is null)
                            {
                                Plugin.Log.Warn($"Could not find instatiation instructor for the type ${set.installerType}");
                                continue;
                            }
                            instructor.Install(set, binding);
                        }
                    }
                }

                // Install every installerless binding set.
                foreach (var instruction in zenjector.InstallInstructions)
                {
                    foreach (var binding in installerBindings)
                    {
                        if (instruction.baseInstaller == binding.installerType)
                        {
                            instruction.onInstall(binding.context.Container);
                        }
                    }
                }
            }
        }

        private void SceneManager_activeSceneChanged(Scene _, Scene newScene)
        {
            // The following is to handle loading the game from 'Uninit Mode', when SteamVR has not initialized the project context and original scene get initially loaded and constructed.
            if (newScene.name == _initialSceneName)
            {
                InitialSceneConstructionRegistered = true;
            }
            else if (!InitialSceneConstructionRegistered)
            {
                GameScenesManager gameScenesManager = ProjectContext.Instance.Container.Resolve<GameScenesManager>();
                void GameScenesManager_transitionDidFinishEvent(ScenesTransitionSetupDataSO _, DiContainer container)
                {
                    gameScenesManager.transitionDidFinishEvent -= GameScenesManager_transitionDidFinishEvent;
                    // If we still have not registered yet, forcibly restart the game.
                    if (!InitialSceneConstructionRegistered)
                    {
                        // Only continue when we are able to. 
                        MenuTransitionsHelper menuTransitionsHelper = container.TryResolve<MenuTransitionsHelper>();
                        gameScenesManager.StartCoroutine(RestartBeatSaberASAP(gameScenesManager, menuTransitionsHelper));
                    }
                }
                gameScenesManager.transitionDidFinishEvent += GameScenesManager_transitionDidFinishEvent;
            }
        }

        private IEnumerator RestartBeatSaberASAP(GameScenesManager gameScenesManager, MenuTransitionsHelper menuTransitionsHelper)
        {
            // This waits until the menu transition finishes before forcibly restarting Beat Saber
            yield return gameScenesManager.waitUntilSceneTransitionFinish;
            if (InitialSceneConstructionRegistered)
                yield break;
            if (menuTransitionsHelper != null)
            {
                Plugin.Log.Debug("Restarting Beat Saber");
                InitialSceneConstructionRegistered = true;
                menuTransitionsHelper?.RestartGame();
            }
        }

        public void Disable()
        {
            Harmony.ContextDecorator.ContextInstalling -= ContextDecorator_ContextInstalling;
            SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
            PluginManager.PluginDisabled -= PluginManager_PluginDisabled;
            PluginManager.PluginEnabled -= PluginManager_PluginEnabled;
        }

        private class ZenjectorDatum
        {
            public bool Enabled { get; set; }
            public Zenjector Zenjector { get; }

            public ZenjectorDatum(Zenjector zenjector)
            {
                Zenjector = zenjector;
            }
        }
    }
}