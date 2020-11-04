using System;
using Zenject;
using System.Linq;
using SiraUtil.Events;
using System.Collections.Generic;

namespace SiraUtil.Services
{
    /// <summary>
    /// A model provider for the saber. Use this to get an instance of the saber model.
    /// </summary>
    public class SaberProvider : ITickable, IDisposable
    {
        private bool _gameCoreInstalling;
        private  DiContainer _container;
        private readonly List<DelegateWrapper> _queuedActions = new List<DelegateWrapper>(); 
        private readonly List<DelegateWrapper> _queuedNormalActions = new List<DelegateWrapper>();

        private SaberModelController _saberModelController;
        internal SaberModelController ModelPrefab
        {
            get => _saberModelController;
            set
            {
                _saberModelController = value;
                ControllerReady?.Invoke();
            }
        }

        /// <summary>
        /// Invoked when the model provider is setup.
        /// </summary>
        public event Action ControllerReady;

        public SaberProvider(DiContainer container, SceneContext sceneContext, [InjectOptional] IDifficultyBeatmap beatmap)
        {
            _container = container;

            _gameCoreInstalling = beatmap != null;
            SiraEvents.ContextInstalling += SiraEvents_ContextInstalling;
        }

        private void SiraEvents_ContextInstalling(object sender, SiraEvents.SceneContextInstalledArgs e)
        {
            if (e.Name == nameof(GameplayCoreInstaller))
            {
                _container = e.Container;
                _gameCoreInstalling = true;
                foreach (var action in _queuedActions)
                {
                    action.actionObj.Invoke(GetModel());
                }
            }
        }

        /// <summary>
        /// Is the model available?
        /// </summary>
        /// <param name="callback">If the model is available, this will invoke the event <see cref="ControllerReady"/> for any listeners to know when it's safe to get a model.</param>
        /// <returns>Whether or not the model is available.</returns>
        public bool IsSafe(bool callback = true)
        {
            var safe = ModelPrefab != null;
            if (safe)
            {
                if (callback)
                {
                    ControllerReady?.Invoke();
                }
            }
            return safe;
        }

        /// <summary>
        /// Get an instance of the current saber model. This CAN be null! Only use this when you know the model is present. You can subscribe to <see cref="ControllerReady"/> and then call <see cref="IsSafe(bool)"/> to ensure that you get the model. SiraSaber's handle this automatically.
        /// </summary>
        /// <returns>The current saber model.</returns>
        public SaberModelController GetModel()
        {
            var smc = ModelPrefab == null ? null : _container.InstantiatePrefab(ModelPrefab.gameObject).GetComponent<SaberModelController>();
            return smc;
        }

        public void GetModel(Action<SaberModelController> callback)
        {
            GetModel<SaberModelController>(callback);
        }

        public void GetModel<T>(Action<T> callback) where T : SaberModelController
        {
            if (_gameCoreInstalling)
            {
                var smc = ModelPrefab == null ? null : _container.InstantiatePrefab(ModelPrefab.gameObject).GetComponent<SaberModelController>();
                if (smc == null)
                {
                    _queuedNormalActions.Add(new DelegateWrapper().Wrap(callback));
                }
                else
                {
                    callback.Invoke((T)GetModel());
                }
            }
            else
            {
                _queuedActions.Add(new DelegateWrapper().Wrap(callback));
            }
            Update();
        }

        internal void Update()
        {
            if (IsSafe(false))
            {
                foreach (var action in _queuedNormalActions)
                {
                    action.actionObj.Invoke(GetModel());
                }
                _queuedNormalActions.Clear();
            }
        }

        public void Dispose()
        {
            SiraEvents.ContextInstalling -= SiraEvents_ContextInstalling;
        }

        public void Tick()
        {
            if (_queuedNormalActions.Count() > 0)
            {
                Update();
            }
        }

        private class DelegateWrapper
        {
            public Action<object> actionObj;

            public DelegateWrapper Wrap<T>(Action<T> callback)
            {
                actionObj = delegate (object obj)
                {
                    callback((T)obj);
                };
                return this;
            }
        }
    }
}