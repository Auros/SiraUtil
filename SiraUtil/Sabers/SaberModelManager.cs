using SiraUtil.Extras;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SiraUtil.Sabers
{
    /// <summary>
    /// Manages sabers and their models
    /// </summary>
    public class SaberModelManager : IDisposable
    {
        private readonly ColorManager _colorManager;
        private readonly SiraSaberFactory _siraSaberFactory;
        private readonly Dictionary<Saber, SaberModelController> _saberModelLink = new();

        internal SaberModelManager(ColorManager colorManager, SiraSaberFactory siraSaberFactory)
        {
            _colorManager = colorManager;
            _siraSaberFactory = siraSaberFactory;
            _siraSaberFactory.SaberCreated += SiraSaberFactory_SaberCreated;
        }

        private void SiraSaberFactory_SaberCreated(SiraSaber siraSaber)
        {
            _saberModelLink.Add(siraSaber.Saber, siraSaber.Model);
        }

        /// <summary>
        /// Gets the saber model controller associated with a Saber, or null if it can't be found.
        /// </summary>
        /// <param name="saber">The saber to get the model for.</param>
        /// <returns>The model controller of the saber, or null if it can't be found.</returns>
        public SaberModelController? GetSaberModelController(Saber saber)
        {
            if (_saberModelLink.TryGetValue(saber, out SaberModelController smc))
                return smc;
            else
            {
                smc = saber.GetComponentInChildren<SaberModelController>();
                if (smc != null)
                {
                    _saberModelLink.Add(saber, smc);
                    return smc;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the color of a saber.
        /// </summary>
        /// <param name="saber">The saber to get the color of.</param>
        /// <returns>The color of the saber, or the color of the saber type if the saber's physical color can't be found.</returns>
        public Color GetPhysicalSaberColor(Saber saber)
        {
            SaberModelController? saberModelController = GetSaberModelController(saber);
            if (saberModelController != null)
            {
                Color color = saberModelController.GetColor();
                return color;
            }
            return _colorManager.ColorForSaberType(saber.saberType);
        }

        /// <summary>
        /// Disposes this object.
        /// </summary>
        public void Dispose()
        {
            _siraSaberFactory.SaberCreated -= SiraSaberFactory_SaberCreated;
        }
    }
}
