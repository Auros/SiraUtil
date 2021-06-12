using SiraUtil.Logging;
using System;
using UnityEngine;
using Zenject;

namespace SiraUtil.Sabers
{
    /// <summary>
    /// A service responsible for creating new <see cref="SiraSaber"/>s.
    /// </summary>
    public class SiraSaberFactory
    {
        private readonly SiraLog _siraLog;
        private readonly DiContainer _container;

        /// <summary>
        /// Called when a new saber is created.
        /// </summary>
        public event Action<SiraSaber>? SaberCreated;
        internal event Action<Saber, Color>? ColorUpdated;

        internal SiraSaberFactory(SiraLog siraLog, DiContainer container)
        {
            _siraLog = siraLog;
            _container = container;
        }

        /// <summary>
        /// Spawns in a new <see cref="SiraSaber"/>
        /// </summary>
        /// <param name="saberType">The type of the saber.</param>
        /// <returns>The created saber.</returns>
        public SiraSaber Spawn(SaberType saberType)
        {
            return Spawn<Saber>(saberType);
        }

        /// <summary>
        /// Spawns in a new <see cref="SiraSaber"/> with a custom backing saber type.
        /// </summary>
        /// <typeparam name="TBackingSaber">The custom type of the saber.</typeparam>
        /// <param name="saberType">The type of the saber.</param>
        /// <returns>The created saber.</returns>
        public SiraSaber Spawn<TBackingSaber>(SaberType saberType) where TBackingSaber : Saber
        {
            _siraLog.Debug($"Creating a new saber. Type: ({saberType}).");
            GameObject siraSaberGameObject = new("SiraUtil | SiraSaber");
            SiraSaber siraSaber = _container.InstantiateComponent<SiraSaber>(siraSaberGameObject);
            siraSaber.ColorUpdated = UpdateColorInternal;
            siraSaber.Setup<TBackingSaber>(saberType);
            SaberCreated?.Invoke(siraSaber);
            return siraSaber;
        }

        private void UpdateColorInternal(Saber saber, Color color)
            => ColorUpdated?.Invoke(saber, color);
    }
}