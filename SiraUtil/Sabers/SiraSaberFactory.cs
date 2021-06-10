using SiraUtil.Logging;
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
            _siraLog.Debug($"Creating a new saber. Type: ({saberType}).");
            GameObject siraSaberGameObject = new("SiraUtil | SiraSaber");
            SiraSaber siraSaber = _container.InstantiateComponent<SiraSaber>(siraSaberGameObject);
            siraSaber.SetInitialType(saberType);
            return siraSaber;
        }
    }
}