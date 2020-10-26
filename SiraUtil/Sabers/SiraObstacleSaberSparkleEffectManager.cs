using System;
using System.Linq;
using UnityEngine;
using System.Reflection;
using SiraUtil.Interfaces;
using System.Collections.Generic;

namespace SiraUtil.Sabers
{
    public class SiraObstacleSaberSparkleEffectManager : ObstacleSaberSparkleEffectManager, ISaberRegistrar
    {
        private readonly List<ObstacleSparkleDatum> _obstacleSparkleData = new List<ObstacleSparkleDatum>();

        public SiraObstacleSaberSparkleEffectManager()
        {
            //_sparkleEndEvent = Extensions.GetEventHandlers<ObstacleSaberSparkleEffectManager, Action<SaberType>>(this, "sparkleEffectDidEndEvent");
            //_sparkleStartEvent = Extensions.GetEventHandlers<ObstacleSaberSparkleEffectManager, Action<SaberType>>(this, "sparkleEffectDidStartEvent");
            ObstacleSaberSparkleEffectManager original = GetComponent<ObstacleSaberSparkleEffectManager>();
            foreach (FieldInfo info in original.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                info.SetValue(this, info.GetValue(original));
            }
            Destroy(original);
        }

        private bool _initted;

        public override void Start()
        {
            Run();
        }

        public void Initialize(SaberManager saberManager)
        {
            Run();
            _sabers = new Saber[2];
            _sabers[0] = saberManager.leftSaber;
            _sabers[1] = saberManager.rightSaber;
            _obstacleSparkleData[0].saber = _sabers[0];
            _obstacleSparkleData[1].saber = _sabers[1];
        }

        private void Run()
        {
            if (!_initted)
            {
                base.Start();
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
                _initted = true;
            }
        }

        public override void Update()
        {
            for (int i = 0; i < _obstacleSparkleData.Count; i++)
            {
                _obstacleSparkleData[i].wasSystemActive = _obstacleSparkleData[i].isSystemActive;
                _obstacleSparkleData[i].isSystemActive = false;
            }
            foreach (ObstacleController obstacleController in _beatmapObjectManager.activeObstacleControllers)
            {
                Bounds bounds = obstacleController.bounds;
                for (int i = 0; i < _obstacleSparkleData.Count; i++)
                {
                    ObstacleSparkleDatum osd = _obstacleSparkleData[i];
                    if (osd.saber != null && osd.saber.isActiveAndEnabled && GetBurnMarkPos(bounds, obstacleController.transform, osd.saber.saberBladeBottomPos, osd.saber.saberBladeTopPos, out Vector3 pos))
                    {
                        osd.isSystemActive = true;
                        osd.burnMarkPosition = pos;
                        osd.sparkleEffect.SetPositionAndRotation(pos, GetEffectRotation(pos, obstacleController.transform, bounds));
                        _hapticFeedbackController.PlayHapticFeedback(osd.saber.saberType.Node(), _rumblePreset);
                        if (!osd.wasSystemActive)
                        {
                            osd.sparkleEffect.StartEmission();
                            Extensions.GetEventHandlers<ObstacleSaberSparkleEffectManager, Action<SaberType>>(this, "sparkleEffectDidStartEvent")?.Invoke(osd.saber.saberType);
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
                    Extensions.GetEventHandlers<ObstacleSaberSparkleEffectManager, Action<SaberType>>(this, "sparkleEffectDidEndEvent")?.Invoke(osd.saber.saberType);
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

        public override Vector3 BurnMarkPosForSaberType(SaberType saberType)
        {
            if (_obstacleSparkleData.Count() >= 2)
            {
                if (_obstacleSparkleData[0].saber != null && saberType == _obstacleSparkleData[0].saber.saberType)
                {
                    return _obstacleSparkleData[0].burnMarkPosition;
                }
                return _obstacleSparkleData[1].burnMarkPosition;
            }
            return Vector3.zero;
        }

        public void RegisterSaber(Saber saber)
        {
            var osd = new ObstacleSparkleDatum
            {
                saber = saber,
                sparkleEffect = Instantiate(_obstacleSaberSparkleEffectPrefab)
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

        public void ChangeColor(Saber _) { }

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