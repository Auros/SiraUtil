using System;
using System.Linq;
using UnityEngine;
using SiraUtil.Interfaces;
using System.Collections.Generic;

namespace SiraUtil.Sabers
{
    /// <summary>
    /// An upgraded obstacle manager which supports more than two sabers.
    /// </summary>
    public class SiraObstacleSaberSparkleEffectManager : ObstacleSaberSparkleEffectManager, ISaberRegistrar
    {
        private readonly List<ObstacleSparkleDatum> _obstacleSparkleData = new List<ObstacleSparkleDatum>();

        private bool _initted;

        /// <summary>
        /// The start method.
        /// </summary>
        public override void Start()
        {
            Run();
        }

        /// <summary>
        /// The initialization method.
        /// </summary>
        /// <param name="saberManager">The saber manager used to gather saber references.</param>
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

        /// <summary>
        /// The Unity Update method.
        /// </summary>
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

        /// <summary>
        /// The disable method.
        /// </summary>
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

        /// <summary>
        /// Gets a burn mark position for a specific saber type.
        /// </summary>
        /// <param name="saberType"></param>
        /// <returns></returns>
        public override Vector3 BurnMarkPosForSaberType(SaberType saberType)
        {
            if (_obstacleSparkleData.Count() >= 2)
            {
                return _obstacleSparkleData[0].saber != null && saberType == _obstacleSparkleData[0].saber.saberType
                    ? _obstacleSparkleData[0].burnMarkPosition
                    : _obstacleSparkleData[1].burnMarkPosition;
            }
            return Vector3.zero;
        }

        /// <summary>
        /// Registers a saber into the Sira obstacle effect manager.
        /// </summary>
        /// <param name="saber"></param>
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

        /// <summary>
        /// Unregisters a saber in the Sira obstacle effect manager.
        /// </summary>
        /// <param name="saber"></param>
        public void UnregisterSaber(Saber saber)
        {
            ObstacleSparkleDatum osd = _obstacleSparkleData.FirstOrDefault(o => o.saber == saber);
            if (osd != null)
            {
                _obstacleSparkleData.Remove(osd);
            }
        }

        /// <summary>
        /// Force a registered saber to change its color.
        /// </summary>
        /// <param name="_"></param>
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