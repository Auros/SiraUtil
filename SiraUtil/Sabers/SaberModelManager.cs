using SiraUtil.Extras;
using SiraUtil.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace SiraUtil.Sabers
{
    /// <summary>
    /// Manages sabers and their models
    /// </summary>
    public class SaberModelManager : ILateTickable, IDisposable
    {
        private readonly SiraLog _siraLog;
        private readonly ColorManager _colorManager;
        private readonly SiraSaberFactory _siraSaberFactory;
        private readonly Dictionary<Saber, SiraSaber> _siraSaberLink = [];
        private readonly Dictionary<Saber, SaberModelController> _saberModelLink = [];
        private readonly List<DesperationContract> _desperationList = [];
        private readonly List<DesperationContract> _salvationList = [];
        private readonly Queue<Action> _colorUpdateQueue = [];

        /// <summary>
        /// Called when a saber's color has been changed.
        /// </summary>
        public event Action<Saber, Color>? ColorUpdated;

        internal SaberModelManager(SiraLog siraLog, ColorManager colorManager, SiraSaberFactory siraSaberFactory)
        {
            _siraLog = siraLog;
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
            {
                return smc;
            }
            else
            {
                foreach (Transform child in saber.gameObject.transform)
                {
                    if (child.gameObject.TryGetComponent(out SaberModelController saberModelController))
                    {
                        _siraLog.Debug("Found a new saber model controller.");
                        smc = saberModelController;
                        break;
                    }
                }
                if (smc != null)
                {
                    _siraLog.Debug("Adding model to cache, establishing a link.");
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
                if (saberModelController != null)
                {
                    _siraLog.Debug("Enqueing color update.");
                    _colorUpdateQueue.Enqueue(() =>
                    {
                        _siraLog.Debug("Updated color of saber.");
                        saberModelController.SetColor(color);
                        ColorUpdated?.Invoke(saber, color);
                    });

                    DesperationContract contract = _desperationList.FirstOrDefault(d => d.Saber == saber && d.color == color);
                    if (contract is not null)
                        _salvationList.Add(contract);
                }
                else
                {
                    if (!_desperationList.Any(d => d.Saber == saber))
                    {
                        // The desperation list is designed for sabers still in the progress of initialization.
                        // It will continue retrying up to a set amount.
                        _siraLog.Debug("Could not find saber model controller. Adding it to the desperation list.");
                        _desperationList.Add(new DesperationContract(saber, color));
                    }
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

            foreach (DesperationContract desperator in _desperationList)
            {
                if (desperator.Saber != null)
                    SetColor(desperator.Saber, desperator.color);
            }
            foreach (DesperationContract salvation in _salvationList)
                _desperationList.Remove(salvation);
            _salvationList.Clear();
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

        private class DesperationContract
        {
            private const int _maxAccessTimes = 10;
            public readonly Color color;
            private int _accessed = 0;

            public Saber? Saber
            {
                get
                {
                    _accessed++;
                    if (_accessed == _maxAccessTimes)
                    {
                        Saber? saber = field;
                        field = null;
                        return saber;
                    }
                    return field;
                }
            }

            public DesperationContract(Saber saber, Color color)
            {
                Saber = saber;
                this.color = color;
            }
        }
    }
}