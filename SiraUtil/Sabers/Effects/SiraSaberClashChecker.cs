using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace SiraUtil.Sabers.Effects
{
    internal interface ISiraClashChecker
    {
        event Action<Saber, Saber>? NewSabersClashed;

        bool ExtraSabersDetected { get; }

        bool AreSabersClashing(ref bool sabersAreClashing, ref Vector3 localClashingPoint, ref int prevGetFrameNum, out Vector3 clashingPoint);
    }

    internal class SiraSaberClashChecker : SaberClashChecker, ISiraClashChecker
    {
        private Saber? _lastSaberA;
        private Saber? _lastSaberB;
        private readonly HashSet<Saber> _sabers = [];
        public event Action<Saber, Saber>? NewSabersClashed;

        protected readonly DiContainer _container;
        protected readonly SaberManager _saberManager;
        protected readonly SiraSaberFactory _siraSaberFactory;

        public bool ExtraSabersDetected { get; private set; }

        public SiraSaberClashChecker(DiContainer container, SaberManager saberManager, SiraSaberFactory siraSaberFactory)
        {
            _container = container;
            _saberManager = saberManager;
            _siraSaberFactory = siraSaberFactory;
            _sabers.Add(_saberManager.leftSaber);
            _sabers.Add(_saberManager.rightSaber);
            _siraSaberFactory.SaberCreated += SiraSaberFactory_SaberCreated;
        }

        private void SiraSaberFactory_SaberCreated(SiraSaber siraSaber)
        {
            ExtraSabersDetected = true;
            _sabers.Add(siraSaber.Saber);
        }

        ~SiraSaberClashChecker()
        {
            _siraSaberFactory.SaberCreated -= SiraSaberFactory_SaberCreated;
        }

        public bool AreSabersClashing(ref bool sabersAreClashing, ref Vector3 localClashingPoint, ref int prevGetFrameNum, out Vector3 clashingPoint)
        {
            if (_leftSaber.movementDataForLogic.lastAddedData.time < 0.1f)
            {
                clashingPoint = localClashingPoint;
                return false;
            }
            if (prevGetFrameNum == Time.frameCount)
            {
                clashingPoint = localClashingPoint;
                return sabersAreClashing;
            }
            prevGetFrameNum = Time.frameCount;
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

                        if (SegmentToSegmentDist(saberBladeBottomPos, saberBladeTopPos, saberBladeBottomPos2, saberBladeTopPos2, out Vector3 clashingPoint2) < 0.08f && saberA.isActiveAndEnabled && saberB.isActiveAndEnabled)
                        {
                            if (_lastSaberA == null && _lastSaberB == null)
                            {
                                _lastSaberA = saberA;
                                _lastSaberB = saberB;
                                NewSabersClashed?.Invoke(_lastSaberA, _lastSaberB);
                            }
                            localClashingPoint = clashingPoint2;
                            clashingPoint = localClashingPoint;
                            sabersAreClashing = true;
                            return sabersAreClashing;
                        }
                        else
                        {
                            _lastSaberA = null;
                            _lastSaberB = null;
                            sabersAreClashing = false;
                        }
                    }
                }
            }
            clashingPoint = localClashingPoint;
            return sabersAreClashing;
        }
    }
}