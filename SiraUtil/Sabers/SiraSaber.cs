using Zenject;
using UnityEngine;
using SiraUtil.Services;
using System.Collections;
using System;

namespace SiraUtil.Sabers
{
    /// <summary>
    /// A SiraSaber is an extra saber with some useful extension methods. The SiraSaber object is on the same GameObject as the normal Saber object, it's not an overridden version of the default Saber class.
    /// </summary>
    public class SiraSaber : MonoBehaviour
    {
        /// <summary>
        /// The next saber type to generate. Zenject factories don't easily support multiple parameters so set this to dictate what the next saber type will be (it can be changed, but model providers that use the saber type to dictate the model won't appear properly).
        /// </summary>
        public static SaberType nextType = SaberType.SaberB;

        /// <summary>
        /// The active saber being controlled by this SiraSaber.
        /// </summary>
        public Saber Saber => _saber;
        private Saber _saber;
        private NoteCutter _noteCutter;
        private ColorManager _colorManager;
        private SaberProvider _saberProvider;
        private SaberTypeObject _saberTypeObject;
        private IEnumerator _changeColorCoroutine = null;
        private SiraSaberEffectManager _siraSaberEffectManager;

        /// <summary>
        /// Called when the model of the saber is setup.
        /// </summary>
        public event Action<SiraSaber, SaberModelController> ModelDidInit;

        [Inject]
        internal void Construct(NoteCutter noteCutter, ColorManager colorManager, SaberProvider saberProvider, SiraSaberEffectManager siraSaberEffectManager)
        {
            _noteCutter = noteCutter;
            _colorManager = colorManager;
            _saberProvider = saberProvider;
            _siraSaberEffectManager = siraSaberEffectManager;
            // Create all the stuff thats supposed to be on the saber
            _saberTypeObject = gameObject.AddComponent<SaberTypeObject>();
            Accessors.ObjectSaberType(ref _saberTypeObject) = nextType;

            // Create and populate the saber object
            _saber = gameObject.AddComponent<Saber>();
            Accessors.SaberObjectType(ref _saber) = _saberTypeObject;
            var top = new GameObject("Top");
            var bottom = new GameObject("Bottom");
            top.transform.SetParent(transform);
            bottom.transform.SetParent(transform);
            top.transform.position = new Vector3(0f, 0f, 1f);

            Accessors.SaberHandleTransform(ref _saber) = bottom.transform;
            Accessors.SaberBladeTopTransform(ref _saber) = top.transform;
            Accessors.SaberBladeBottomTransform(ref _saber) = bottom.transform;
            Accessors.SaberBladeTopPosition(ref _saber) = top.transform.position;
            Accessors.SaberBladeBottomPosition(ref _saber) = bottom.transform.position;

            _saberProvider.GetModel(smc =>
            {
                smc.Init(transform, _saber);
                ModelDidInit?.Invoke(this, smc);
            });

            _siraSaberEffectManager.SaberCreated(_saber);
        }

        /// <summary>
        /// The update method.
        /// </summary>
        public void Update()
        {
            if (_saber != null && _saber.gameObject.activeInHierarchy && !_saber.disableCutting)
            {
                var topTrans = Accessors.SaberBladeTopTransform(ref _saber);
                var botTrans = Accessors.SaberBladeBottomTransform(ref _saber);

                var topPos = Accessors.SaberBladeTopPosition(ref _saber) = topTrans.position;
                var botPos = Accessors.SaberBladeBottomPosition(ref _saber) = botTrans.position;
                
                int i = 0;
                var swingRatingCounters = Accessors.SwingRatingCounters(ref _saber);
                var unusedSwingRatingCounters = Accessors.UnusedSwingRatingCounters(ref _saber);
                while (i < swingRatingCounters.Count)
                {
                    var counter = swingRatingCounters[i];
                    if (counter.didFinish)
                    {
                        counter.Deinit();
                        swingRatingCounters.RemoveAt(i);
                        unusedSwingRatingCounters.Add(counter);
                    }
                    else
                    {
                        i++;
                    }
                }
                Accessors.MovementData(ref _saber).AddNewData(topPos, botPos, TimeHelper.time);
                _noteCutter.Cut(_saber);
            }
        }

        /// <summary>
        /// The destroy method.
        /// </summary>
        public void OnDestroy()
        {
            //_saberProvider.ControllerReady -= ModelsReady;
            _siraSaberEffectManager.SaberDestroyed(_saber);
        }

        /// <summary>
        /// Changes the type and color of the saber.
        /// </summary>
        /// <param name="type">The type of the new saber type.</param>
        public void SetType(SaberType type)
        {
            _saber.SetType(type, _colorManager);
        }

        /// <summary>
        /// Changes the type of the SiraSaber.
        /// </summary>
        /// <param name="type">The type of the new saber type.</param>
        public void ChangeType(SaberType type)
        {
            _saber.ChangeType(type);
        }

        /// <summary>
        /// Swaps the type of the saber.
        /// </summary>
        /// <param name="resetColor">Whether or not to change the color as well.</param>
        public void SwapType(bool resetColor = false)
        {
            var saberType = _saberTypeObject.saberType == SaberType.SaberA ? SaberType.SaberB : SaberType.SaberA;
            if (resetColor)
            {
                SetType(saberType);
            }
            else
            {
                ChangeType(saberType);
            }
        }

        /// <summary>
        /// Changes the color of the saber.
        /// </summary>
        /// <param name="color">The color you want the saber to be.</param>
        public void ChangeColor(Color color)
        {
            if (_changeColorCoroutine != null)
            {
                StopCoroutine(_changeColorCoroutine);
                _changeColorCoroutine = null;
            }
            StartCoroutine(ChangeColorCoroutine(color));
        }

        private IEnumerator ChangeColorCoroutine(Color color)
        {
            yield return new WaitForSecondsRealtime(0.05f);
            if (_saber.isActiveAndEnabled)
            {
                _saber.ChangeColorInstant(color);
            }
            _siraSaberEffectManager.ChangeColor(_saber);
            _changeColorCoroutine = null;
        }

        /// <summary>
        /// Set the Saber that lies in this SiraSaber. Really should only be used when the saber registered in this SiraSaber is destroyed or overridden.
        /// </summary>
        /// <param name="saber">The new saber.</param>
        public void SetSaber(Saber saber)
        {
            _saber = saber;
            _siraSaberEffectManager.SaberCreated(_saber);
        }

        #region Zenject

        /// <summary>
        /// The placeholder factory used in Zenject. Request this in the container to receive the factory.
        /// </summary>
        public class Factory : PlaceholderFactory<SiraSaber>
        {

        }

        /// <summary>
        /// A factory for dynamically generating new sabers.
        /// </summary>
        public class SaberFactory : IFactory<SiraSaber>
        {
            private readonly DiContainer _container;

            internal SaberFactory(DiContainer container)
            {
                _container = container;
            }

            /// <summary>
            /// Creates a new SiraSaber. Any sabers created this way are automatically inserted into the effect manager.
            /// </summary>
            /// <returns></returns>
            public SiraSaber Create()
            {
                var saberObject = new GameObject("SiraUtil Saber");
                SiraSaber sira = saberObject.AddComponent<SiraSaber>();
                _container.InjectGameObject(saberObject);
                return sira;
            }
        }
        #endregion Zenject
    }
}