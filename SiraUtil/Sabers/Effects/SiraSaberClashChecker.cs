using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace SiraUtil.Sabers.Effects
{
    internal class SiraSaberClashChecker : SaberClashChecker
    {
        private Saber? _lastSaberA;
        private Saber? _lastSaberB;
        private bool _extraSabersDetected;
        private readonly HashSet<Saber> _sabers = new();
        public event Action<Saber, Saber>? NewSabersClashed;

        protected readonly DiContainer _container;
        protected readonly SaberManager _saberManager;
        protected readonly SiraSaberFactory _siraSaberFacotry;

        public SiraSaberClashChecker(DiContainer container, SaberManager saberManager, SiraSaberFactory siraSaberFactory)
        {
            _container = container;
            _saberManager = saberManager;
            _siraSaberFacotry = siraSaberFactory;
            _sabers.Add(_saberManager.leftSaber);
            _sabers.Add(_saberManager.rightSaber);

            _siraSaberFacotry.SaberCreated += SiraSaberFacotry_SaberCreated;
        }

        private void SiraSaberFacotry_SaberCreated(SiraSaber siraSaber)
        {
            _extraSabersDetected = true;
            _sabers.Add(siraSaber.Saber);
        }

        ~SiraSaberClashChecker()
        {
            _siraSaberFacotry.SaberCreated -= SiraSaberFacotry_SaberCreated;
        }

        /// <summary>
        /// Checks if any of the registered sabers are clashing.
        /// </summary>
        /// <param name="clashingPoint">The point that the sabers are clashing at.</param>
        /// <returns>Are any sabers clashing?</returns>
        public override bool AreSabersClashing(out Vector3 clashingPoint)
        {
            if (!_extraSabersDetected)
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
                                NewSabersClashed?.Invoke(_lastSaberA, _lastSaberB);
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
    }
}
