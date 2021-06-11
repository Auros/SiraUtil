using SiraUtil.Extras;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SiraUtil.Sabers
{
    /// <summary>
    /// Manages sabers and their models
    /// </summary>
    public class SaberModelManager : ILateTickable, IDisposable
    {
        private readonly ColorManager _colorManager;
        private readonly SiraSaberFactory _siraSaberFactory;
        private readonly Dictionary<Saber, SiraSaber> _siraSaberLink = new();
        private readonly Dictionary<Saber, SaberModelController> _saberModelLink = new();
        private readonly Queue<Action> _colorUpdateQueue = new();

        /// <summary>
        /// Called when a saber's color has been changed.
        /// </summary>
        public event Action<Saber, Color>? ColorUpdated;

        internal SaberModelManager(ColorManager colorManager, SiraSaberFactory siraSaberFactory)
        {
            _colorManager = colorManager;
            _siraSaberFactory = siraSaberFactory;
            _siraSaberFactory.SaberCreated += SiraSaberFactory_SaberCreated;
        }

        private void SiraSaberFactory_SaberCreated(SiraSaber siraSaber)
        {
            _siraSaberLink.Add(siraSaber.Saber, siraSaber);
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
        /// Efficiently changes the color of a saber.
        /// </summary>
        /// <param name="saber">The saber to change the color of.</param>
        /// <param name="color">The color to change the saber to.</param>
        public void SetColor(Saber saber, Color color)
        {
            if (_siraSaberLink.TryGetValue(saber, out SiraSaber siraSaber))
            {
                siraSaber.SetColor(color);
            }
            else
            {
                SaberModelController? saberModelController = GetSaberModelController(saber);
                if (saberModelController is not null)
                {
                    _colorUpdateQueue.Enqueue(() =>
                    {
                        saberModelController.SetColor(color);
                        ColorUpdated?.Invoke(saber, color);
                    });
                }
            }
        }

        /// <summary>
        /// Object tick loop.
        /// </summary>
        /// <remarks>
        /// This is called by Zenject. Please don't call it manually.
        /// </remarks>
        public void LateTick()
        {
            while (_colorUpdateQueue.Count > 0)
                _colorUpdateQueue.Dequeue().Invoke();
        }

        /// <summary>
        /// Disposes this object.
        /// </summary>
        /// <remarks>
        /// This is called by Zenject. Please don't call it manually.
        /// </remarks>
        public void Dispose()
        {
            _siraSaberFactory.SaberCreated -= SiraSaberFactory_SaberCreated;
        }
    }
}