using IPA.Loader;
using SiraUtil.Zenject.Internal;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using Zenject;

namespace SiraUtil.Zenject
{
    internal class ZenjectManager
    {
        private const string _initialSceneName = "PCInit";

        private bool _initialSceneConstructionRegistered;
        private readonly HashSet<ZenjectorDatum> _zenjectors = new();

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
        }

        private void ContextDecorator_ContextInstalling(IEnumerable<ContextBinding> installerBindings)
        {
            if (!_initialSceneConstructionRegistered)
                return;
        }

        private void SceneManager_activeSceneChanged(Scene _, Scene newScene)
        {
            // The following is to handle loading the game from 'Uninit Mode', when SteamVR has not initialized the project context and original scene get initially loaded and constructed.
            if (newScene.name == _initialSceneName)
            {
                _initialSceneConstructionRegistered = true;
            }
            else if (!_initialSceneConstructionRegistered)
            {
                GameScenesManager gameScenesManager = ProjectContext.Instance.Container.Resolve<GameScenesManager>();
                void GameScenesManager_transitionDidFinishEvent(ScenesTransitionSetupDataSO _, DiContainer container)
                {
                    gameScenesManager.transitionDidFinishEvent -= GameScenesManager_transitionDidFinishEvent;
                    // If we still have not registered yet, forcibly restart the game.
                    if (!_initialSceneConstructionRegistered)
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
            if (_initialSceneConstructionRegistered)
                yield break;
            if (menuTransitionsHelper != null)
            {
                Plugin.Log.Debug("Restarting Beat Saber");
                _initialSceneConstructionRegistered = true;
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