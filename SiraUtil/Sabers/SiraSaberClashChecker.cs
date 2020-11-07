using Zenject;
using UnityEngine;
using System.Linq;
using IPA.Utilities;
using SiraUtil.Interfaces;
using System.Collections.Generic;

namespace SiraUtil.Sabers
{
    /// <summary>
    /// An upgraded version of the SaberClashChecker which has support for more than two sabers.
    /// </summary>
    public class SiraSaberClashChecker : SaberClashChecker, ISaberRegistrar
    {
        private readonly HashSet<Saber> _sabers = new HashSet<Saber>();

        /// <summary>
        /// Whether or not more than two sabers are being activated.
        /// </summary>
        public bool MultiSaberMode { get; set; } = false;

        private static readonly FieldAccessor<SaberClashEffect, ParticleSystem>.Accessor GlowParticles = FieldAccessor<SaberClashEffect, ParticleSystem>.GetAccessor("_glowParticleSystem");
        private static readonly FieldAccessor<SaberClashEffect, ParticleSystem>.Accessor SparkleParticles = FieldAccessor<SaberClashEffect, ParticleSystem>.GetAccessor("_sparkleParticleSystem");

        /// <summary>
        /// The Zenject container used to gather lazy references.
        /// </summary>
        [Inject]
        protected DiContainer _container;

        private Saber _lastSaberA;
        private Saber _lastSaberB;

        /// <summary>
        /// The initialization method.
        /// </summary>
        /// <param name="saberManager">The saber manager used to gather saber references.</param>
        public void Initialize(SaberManager saberManager)
        {
            _sabers.Clear();
            Init(saberManager);
            _sabers.Add(_leftSaber);
            _sabers.Add(_rightSaber);
        }

        /// <summary>
        /// Checks if any of the registered sabers are clashing.
        /// </summary>
        /// <param name="clashingPoint">The point that the sabers are clashing at.</param>
        /// <returns>Are any sabers clashing?</returns>
        public override bool AreSabersClashing(out Vector3 clashingPoint)
        {
            if (!MultiSaberMode)
            {
                return base.AreSabersClashing(out clashingPoint);
            }
            _sabers.RemoveWhere(x => x == null);
            if (_leftSaber.movementData.lastAddedData.time < 0.1f)
            {
                clashingPoint = _clashingPoint;
                return false;
            }
            if (_prevGetFrameNum == Time.frameCount)
            {
                clashingPoint = _clashingPoint;
                return _sabersAreClashing;
            }
            _prevGetFrameNum = Time.frameCount;
            for (int i = 0; i < _sabers.Count; i++)
            {
                for (int h = 0; h < _sabers.Count; h++)
                {
                    if (i > h)
                    {
                        Saber saberA = _sabers.ElementAt(i);
                        Saber saberB = _sabers.ElementAt(h);
                        if (saberA == saberB || saberA == null || saberB == null)
                        {
                            break;
                        }
                        Vector3 saberBladeTopPos = saberA.saberBladeTopPos;
                        Vector3 saberBladeTopPos2 = saberB.saberBladeTopPos;
                        Vector3 saberBladeBottomPos = saberA.saberBladeBottomPos;
                        Vector3 saberBladeBottomPos2 = saberB.saberBladeBottomPos;

                        if (SegmentToSegmentDist(saberBladeBottomPos, saberBladeTopPos, saberBladeBottomPos2, saberBladeTopPos2, out var clashingPoint2) < 0.08f && saberA.isActiveAndEnabled && saberB.isActiveAndEnabled)
                        {
                            if (_lastSaberA == null && _lastSaberB == null)
                            {
                                _lastSaberA = saberA;
                                _lastSaberB = saberB;

                                // pseudo-lock dep
                                var clashEffect = _container.Resolve<SaberClashEffect>();
                                var glowPS = GlowParticles(ref clashEffect).main;
                                var sparkPS = SparkleParticles(ref clashEffect).main;

                                sparkPS.startColor = glowPS.startColor = Color.Lerp(_lastSaberA.GetColor(), _lastSaberB.GetColor(), 0.5f);
                            }
                            _clashingPoint = clashingPoint2;
                            clashingPoint = _clashingPoint;
                            _sabersAreClashing = true;
                            return _sabersAreClashing;
                        }
                        else
                        {
                            _lastSaberA = null;
                            _lastSaberB = null;
                            _sabersAreClashing = false;
                        }
                    }
                }
            }
            clashingPoint = _clashingPoint;
            return _sabersAreClashing;
        }

        /// <summary>
        /// Changes the color of a registered saber.
        /// </summary>
        /// <param name="_"></param>
        public void ChangeColor(Saber _) { MultiSaberMode = true; }

        /// <summary>
        /// Registers a saber into the clash checker.
        /// </summary>
        /// <param name="saber">The saber to register.</param>
        public void RegisterSaber(Saber saber)
        {
            _sabers.RemoveWhere(x => x == null);
            _sabers.Add(saber);
        }

        /// <summary>
        /// Unregisters a saber in the clash checker.
        /// </summary>
        /// <param name="saber">The saber to unregister.</param>
        public void UnregisterSaber(Saber saber)
        {
            _sabers.RemoveWhere(x => x == null);
            _sabers.Remove(saber);
        }
    }
}