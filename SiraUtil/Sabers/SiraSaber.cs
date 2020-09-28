using Zenject;
using UnityEngine;
using System.Collections;

namespace SiraUtil.Sabers
{
    /// <summary>
    /// A SiraSaber is an extra saber with some useful extension methods. The SiraSaber object is on the same GameObject as the normal Saber object, it's not an overridden version of the default Saber class.
    /// </summary>
    public class SiraSaber : MonoBehaviour
    {
        public static SaberType nextType = SaberType.SaberB;

        public Saber Saber => _saber;
        private Saber _saber;
        private ColorManager _colorManager;
        private SaberTypeObject _saberTypeObject;
        private IEnumerator _changeColorCoroutine = null;
        private ISaberModelController _saberModelController;
        private SiraSaberEffectManager _siraSaberEffectManager;

        [Inject]
        public void Construct(ColorManager colorManager, ISaberModelController modelController, SiraSaberEffectManager siraSaberEffectManager)
        {
            // Woohoo! We received the saber model from Zenject!
            _saberModelController = modelController;
            _saberModelController.Init(transform, nextType);
            _siraSaberEffectManager = siraSaberEffectManager;

            _colorManager = colorManager;

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

            Accessors.TopPos(ref _saber) = top.transform;
            Accessors.BottomPos(ref _saber) = bottom.transform;
			Accessors.HandlePos(ref _saber) = bottom.transform;

            _siraSaberEffectManager.SaberCreated(_saber);
        }

        public void Update()
        {
            if (_saber)
            {
				Accessors.Time(ref _saber) += Time.deltaTime;
                if (!_saber.disableCutting)
                {
                    Vector3 topPosition = Accessors.TopPos(ref _saber).position;
                    Vector3 bottomPosition = Accessors.BottomPos(ref _saber).position;
                    int i = 0;
                    while (i < Accessors.SwingRatingCounters(ref _saber).Count)
                    {
                        SaberSwingRatingCounter swingCounter = Accessors.SwingRatingCounters(ref _saber)[i];
                        if (swingCounter.didFinish)
                        {
                            swingCounter.Deinit();
                            Accessors.SwingRatingCounters(ref _saber).RemoveAt(i);
							Accessors.UnusedSwingRatingCounters(ref _saber).Add(swingCounter);
                        }
                        else
                        {
                            i++;
                        }
                    }
                    SaberMovementData.Data lastAddedData = Accessors.MovementData(ref _saber).lastAddedData;
					Accessors.MovementData(ref _saber).AddNewData(topPosition, bottomPosition, Accessors.Time(ref _saber));
                    if (!_saber.disableCutting)
                    {
						Accessors.Cutter(ref _saber).Cut(_saber, topPosition, bottomPosition, lastAddedData.topPos, lastAddedData.bottomPos);
                    }
                }
            }
        }

        public void OnDestroy()
        {
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
        public class Factory : PlaceholderFactory<SiraSaber>
        {

        }

        /// <summary>
        /// A factory for dynamically generating new sabers.
        /// </summary>
        public class SaberFactory : IFactory<SiraSaber>
        {
            private readonly DiContainer _container;

            public SaberFactory(DiContainer container)
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