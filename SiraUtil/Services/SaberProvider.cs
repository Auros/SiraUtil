using System;
using Zenject;

namespace SiraUtil.Services
{
    /// <summary>
    /// A model provider for the saber. Use this to get an instance of the saber model.
    /// </summary>
    public class SaberProvider
    {
        private readonly DiContainer _container;

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

        public SaberProvider(DiContainer container)
        {
            _container = container;
        }

        /// <summary>
        /// Is the model available?
        /// </summary>
        /// <param name="callback">If the model is available, this will invoke the event <see cref="ControllerReady"/> for any listeners to know when it's safe to get a model.</param>
        /// <returns>Whether or not the model is available.</returns>
        public bool IsSafe(bool callback = true)
        {
            var safe =  ModelPrefab != null;
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
            var smc = ModelPrefab == null ? null : _container.InstantiatePrefab(ModelPrefab).GetComponent<SaberModelController>();
            return smc;
        }
    }
}