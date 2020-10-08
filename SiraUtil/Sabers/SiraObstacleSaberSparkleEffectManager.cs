using System;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using System.Reflection;
using SiraUtil.Interfaces;
using System.Collections.Generic;

namespace SiraUtil.Sabers
{
	public class SiraObstacleSaberSparkleEffectManager : ObstacleSaberSparkleEffectManager, ISaberRegistrar
	{
		private readonly List<ObstacleSparkleDatum> _obstacleSparkleData = new List<ObstacleSparkleDatum>();

		private readonly Action<SaberType> _sparkleEndEvent;
		private readonly Action<SaberType> _sparkleStartEvent;

		public SiraObstacleSaberSparkleEffectManager()
		{
			_sparkleEndEvent = Extensions.GetEventHandlers<ObstacleSaberSparkleEffectManager, Action<SaberType>>(this, "sparkleEffectDidEndEvent");
			_sparkleStartEvent = Extensions.GetEventHandlers<ObstacleSaberSparkleEffectManager, Action<SaberType>>(this, "sparkleEffectDidStartEvent");
			ObstacleSaberSparkleEffectManager original = GetComponent<ObstacleSaberSparkleEffectManager>();
			foreach (FieldInfo info in original.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
			{
				info.SetValue(this, info.GetValue(original));
			}
			Destroy(original);
		}

		public override void Start()
		{
			for (int i = 0; i < _sabers.Length; i++)
			{
				var obstacleSparkle = new ObstacleSparkleDatum
				{
					saber = _sabers[i],
					sparkleEffect = _effects[i],
					effectTransform = _effectsTransforms[i]
				};
				_obstacleSparkleData.Add(obstacleSparkle);
			}
		}

		public override void Update()
		{
			for (int i = 0; i < _obstacleSparkleData.Count; i++)
			{
				_obstacleSparkleData[i].wasSystemActive = _obstacleSparkleData[i].isSystemActive;
				_obstacleSparkleData[i].isSystemActive = false;
			}
			foreach (ObstacleController obstacleController in _obstaclePool.activeItems)
			{
				Bounds bounds = obstacleController.bounds;
				for (int i = 0; i < _obstacleSparkleData.Count; i++)
				{
					ObstacleSparkleDatum osd = _obstacleSparkleData[i];
					if (osd.saber.isActiveAndEnabled && GetBurnMarkPos(bounds, obstacleController.transform, osd.saber.saberBladeBottomPos, osd.saber.saberBladeTopPos, out Vector3 pos))
					{
						osd.isSystemActive = true;
						osd.burnMarkPosition = pos;
						osd.sparkleEffect.SetPositionAndRotation(pos, GetEffectRotation(pos, obstacleController.transform, bounds));
						XRNode node = osd.saber.saberType == SaberType.SaberA ? XRNode.LeftHand : XRNode.RightHand;
						_hapticFeedbackController.ContinuousRumble(node);
						if (!osd.wasSystemActive)
						{
							osd.sparkleEffect.StartEmission();
							_sparkleStartEvent?.Invoke(osd.saber.saberType);
						}
					}
				}
			}
			for (int i = 0; i < _obstacleSparkleData.Count; i++)
			{
				ObstacleSparkleDatum osd = _obstacleSparkleData[i];
				if (!osd.isSystemActive && osd.wasSystemActive)
				{
					osd.sparkleEffect.StopEmission();
					_sparkleEndEvent?.Invoke(osd.saber.saberType);
				}
			}
		}

		public override void OnDisable()
		{
			if (_hapticFeedbackController != null)
			{
				for (int i = 0; i < _obstacleSparkleData.Count; i++)
				{
					if (_obstacleSparkleData[i].isSystemActive)
					{
						_obstacleSparkleData[i].isSystemActive = false;
					}
				}
			}
		}

		public void RegisterSaber(Saber saber)
		{
			var osd = new ObstacleSparkleDatum
			{
				saber = saber,
				sparkleEffect = Instantiate(_obstacleSaberSparkleEffectPefab)
			};
			osd.sparkleEffect.color = _colorManager.GetObstacleEffectColor();
			osd.effectTransform = osd.sparkleEffect.transform;
			_obstacleSparkleData.Add(osd);
		}

		public void UnregisterSaber(Saber saber)
		{
			ObstacleSparkleDatum osd = _obstacleSparkleData.FirstOrDefault(o => o.saber == saber);
			if (osd != null)
			{
				_obstacleSparkleData.Remove(osd);
			}
		}
		public void ChangeColor(Saber saber)
		{

		}

		private class ObstacleSparkleDatum
		{
			public Saber saber;
			public bool isSystemActive;
			public bool wasSystemActive;
			public Vector3 burnMarkPosition;
			public Transform effectTransform;
			public ObstacleSaberSparkleEffect sparkleEffect;
		}
	}
}