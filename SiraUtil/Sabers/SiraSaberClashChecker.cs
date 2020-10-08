using System.Linq;
using UnityEngine;
using IPA.Utilities;
using System.Reflection;
using SiraUtil.Interfaces;
using System.Collections.Generic;

namespace SiraUtil.Sabers
{
	// Not a perfect implementation. It won't spawn more than one clash effect.
	public class SiraSaberClashChecker : SaberClashChecker, ISaberRegistrar
	{
		private SaberClashChecker _instance;
		private readonly List<Saber> _sabers = new List<Saber>();
		private readonly PropertyAccessor<SaberClashChecker, Vector3>.Setter ClashingPoint = PropertyAccessor<SaberClashChecker, Vector3>.GetSetter("clashingPoint");
		private readonly PropertyAccessor<SaberClashChecker, bool>.Setter SabersAreClashing = PropertyAccessor<SaberClashChecker, bool>.GetSetter("sabersAreClashing");

		public SiraSaberClashChecker()
		{
			SaberClashChecker original = GetComponent<SaberClashChecker>();
			foreach (FieldInfo info in original.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
			{
				info.SetValue(this, info.GetValue(original));
			}
			Destroy(original);
		}

		public override void Start()
		{
			_instance = this;
			_sabers.Add(_leftSaber);
			_sabers.Add(_rightSaber);
			Resources.FindObjectsOfTypeAll<SaberClashEffect>().First().SetField("_saberClashChecker", _instance);
		}

		public override void Update()
		{
			for (int i = 0; i < _sabers.Count; i++)
			{
				for (int h = 0; h < _sabers.Count; h++)
				{
					if (i > h)
					{
						Saber iSaber = _sabers[i];
						Saber hSaber = _sabers[h];

						if (iSaber.isActiveAndEnabled && hSaber.isActiveAndEnabled)
						{
							Vector3 isbTop = iSaber.saberBladeTopPos;
							Vector3 hsbTop = hSaber.saberBladeTopPos;
							Vector3 isbBot = iSaber.saberBladeBottomPos;
							Vector3 hsbBot = hSaber.saberBladeBottomPos;
							if (isbBot == hsbBot)
							{
								SabersAreClashing(ref _instance, false);
								return;
							}
							if (SegmentToSegmentDist(isbBot, isbTop, hsbBot, hsbTop, out Vector3 clashPoint) < _minDistanceToClash)
							{
								ClashingPoint(ref _instance, clashPoint);
								SabersAreClashing(ref _instance, true);
								return;
							}
							SabersAreClashing(ref _instance, false);
						}
					}
				}
			}
		}

		public void RegisterSaber(Saber saber)
		{
			_sabers.Add(saber);
		}

		public void UnregisterSaber(Saber saber)
		{
			_sabers.Remove(saber);
		}

		public void ChangeColor(Saber saber)
		{

		}
	}
}