using UnityEngine;
using System.Linq;
using SiraUtil.Interfaces;
using System.Collections.Generic;

namespace SiraUtil.Sabers
{
    /// <summary>
    /// An upgraded obstacle manager which supports more than two sabers.
    /// </summary>
    public class SiraObstacleSaberSparkleEffectManager : ObstacleSaberSparkleEffectManager, ISaberRegistrar
    {
        /// <summary>
        /// Force a registered saber to change its color.
        /// </summary>
        /// <param name="_"></param>
        public void ChangeColor(Saber _)
        {
            
        }

        /// <summary>
        /// The initialization method.
        /// </summary>
        /// <param name="saberManager">The saber manager used to gather saber references.</param>
        public void Initialize(SaberManager saberManager)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public override void Start()
        {
            Recalculate();
            RegisterSaber(_saberManager.leftSaber);
            RegisterSaber(_saberManager.rightSaber);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Update()
        {
            if (_sabers == null || _sabers.Any(s => s == null))
            {
                Recalculate();
            }
            if (_isSystemActive.Length > 2)
            {
                for (int i = 2; i < _isSystemActive.Length; i++)
                {
                    _wasSystemActive[i] = _isSystemActive[i];
                    _isSystemActive[i] = false;
                }
            }
            base.Update();
        }


        /// <summary>
        /// Gets a burn mark position for a specific saber type.
        /// </summary>
        /// <param name="saberType"></param>
        /// <returns></returns>
        public override Vector3 BurnMarkPosForSaberType(SaberType saberType)
        {
            return _sabers.Count() >= 2
                ? _sabers[0] != null && saberType == _sabers[0].saberType
                    ? _burnMarkPositions[0]
                    : _burnMarkPositions[1]
                : Vector3.zero;
        }

        /// <summary>
        /// Registers a saber into the Sira obstacle effect manager.
        /// </summary>
        /// <param name="saber"></param>
        public void RegisterSaber(Saber saber)
        {
            var index = _sabers.IndexOf(saber);
            if (index == -1)
            {
                var effect = Instantiate(_obstacleSaberSparkleEffectPrefab);
                effect.color = _colorManager.GetObstacleEffectColor();

                var saberList = _sabers.ToList();
                saberList.Add(saber);
                _sabers = saberList.ToArray();

                var effectList = _effects.ToList();
                effectList.Add(effect);
                _effects = effectList.ToArray();

                var transformList = _effectsTransforms.ToList();
                transformList.Add(effect.transform);
                _effectsTransforms = transformList.ToArray();

                _burnMarkPositions = new Vector3[_burnMarkPositions.Length + 1];
                _wasSystemActive = new bool[_wasSystemActive.Length + 1];
                _isSystemActive = new bool[_isSystemActive.Length + 1];
            }
        }

        /// <summary>
        /// Unregisters a saber in the Sira obstacle effect manager.
        /// </summary>
        /// <param name="saber"></param>
        public void UnregisterSaber(Saber saber)
        {
            var index = _sabers.IndexOf(saber);
            if (index != -1)
            {
                var saberList = _sabers.ToList();
                saberList.Remove(saber);
                _sabers = saberList.ToArray();

                var effect = _effects[index];
                var effectList = _effects.ToList();
                effectList.Remove(effect);
                _effects = effectList.ToArray();

                var transform = _effectsTransforms[index];
                var transformList = _effectsTransforms.ToList();
                transformList.Remove(transform);
                _effectsTransforms = transformList.ToArray();

                Destroy(effect);

                var burnList = _burnMarkPositions.ToList();
                burnList.RemoveAt(index);
                _burnMarkPositions = burnList.ToArray();

                var isActiveList = _isSystemActive.ToList();
                isActiveList.RemoveAt(index);
                _isSystemActive = isActiveList.ToArray();

                var wasSystemList = _wasSystemActive.ToList();
                wasSystemList.RemoveAt(index);
                _wasSystemActive = wasSystemList.ToArray();
            }
        }

        /// <summary>
        /// Recalculates saber objects.
        /// </summary>
        public void Recalculate()
        {
            IEnumerable<Saber> sabers = new List<Saber>();
            if (_sabers != null)
            {
                sabers = _sabers.Where(s => s != null);

                foreach (var burnEffect in _effects)
                {
                    DestroyImmediate(burnEffect);
                }
            }

            _effects = new ObstacleSaberSparkleEffect[0];
            _effectsTransforms = new Transform[0];
            _burnMarkPositions = new Vector3[0];
            _wasSystemActive = new bool[0];
            _isSystemActive = new bool[0];
            _sabers = new Saber[0];

            foreach (var saber in sabers)
            {
                RegisterSaber(saber);
            }
        }
    }
}