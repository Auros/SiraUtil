using UnityEngine;
using System.Linq;
using SiraUtil.Interfaces;
using System.Collections.Generic;

namespace SiraUtil.Sabers
{
	public class SiraSaberClashChecker : SaberClashChecker, ISaberRegistrar
    {
        private readonly HashSet<Saber> _sabers = new HashSet<Saber>();

		public void Initialize(SaberManager saberManager)
		{
			_sabers.Clear();
			Init(saberManager);
			_sabers.Add(_leftSaber);
			_sabers.Add(_rightSaber);
		}
		
        public override bool AreSabersClashing(out Vector3 clashingPoint)
        {
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
							_clashingPoint = clashingPoint2;
							clashingPoint = _clashingPoint;
							_sabersAreClashing = true;
							return _sabersAreClashing;
						}
						else
						{
							_sabersAreClashing = false;
						}
					}
				}
			}
			clashingPoint = _clashingPoint;
			return _sabersAreClashing;
		}

		public void ChangeColor(Saber _) { }

		public void RegisterSaber(Saber saber)
		{
			_sabers.RemoveWhere(x => x == null);
			_sabers.Add(saber);
		}

		public void UnregisterSaber(Saber saber)
		{
			_sabers.RemoveWhere(x => x == null);
			_sabers.Remove(saber);
		}
	}
}