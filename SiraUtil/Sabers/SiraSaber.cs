using Zenject;
using UnityEngine;
using System.Collections;

namespace SiraUtil.Sabers
{
    public class SiraSaber : MonoBehaviour
    {

        public static SaberType nextType = SaberType.SaberB;

        private Saber _saber;
        private ColorManager _colorManager;
        private SaberTypeObject _saberTypeObject;
        private IEnumerator _changeColorCoroutine = null;
        private ISaberModelController _saberModelController;

        [Inject]
        public void Construct(ISaberModelController modelController, ColorManager colorManager)
        {
            // Woohoo! We received the saber model from Zenject!
            _saberModelController = modelController;
            _saberModelController.Init(transform, nextType);

            _colorManager = colorManager;

            // Create all the stuff thats supposed to be on the saber
            _saberTypeObject = gameObject.AddComponent<SaberTypeObject>();
            Utilities.ObjectSaberType(ref _saberTypeObject) = nextType;

            // Create and populate the saber object
            _saber = gameObject.AddComponent<Saber>();
            Utilities.SaberObjectType(ref _saber) = _saberTypeObject;
            GameObject top = new GameObject("Top");
            GameObject bottom = new GameObject("Bottom");
            top.transform.SetParent(transform);
            bottom.transform.SetParent(transform);
            top.transform.position = new Vector3(0f, 0f, 1f);

            Utilities.TopPos(ref _saber) = top.transform;
            Utilities.BottomPos(ref _saber) = bottom.transform;
            Utilities.HandlePos(ref _saber) = bottom.transform;
        }

        public void Update()
        {
            if (_saber)
            {
                Utilities.Time(ref _saber) += Time.deltaTime;
                Vector3 topPosition = Utilities.TopPos(ref _saber).position;
                Vector3 bottomPosition = Utilities.BottomPos(ref _saber).position;
                int i = 0;
                while (i < Utilities.SwingRatingCounters(ref _saber).Count)
                {
                    SaberSwingRatingCounter swingCounter = Utilities.SwingRatingCounters(ref _saber)[i];
                    if (swingCounter.didFinish)
                    {
                        swingCounter.Deinit();
                        Utilities.SwingRatingCounters(ref _saber).RemoveAt(i);
                        Utilities.UnusedSwingRatingCounters(ref _saber).Add(swingCounter);
                    }
                    else
                    {
                        i++;
                    }
                }
                SaberMovementData.Data lastAddedData = Utilities.MovementData(ref _saber).lastAddedData;
                Utilities.MovementData(ref _saber).AddNewData(topPosition, bottomPosition, Utilities.Time(ref _saber));
                if (!_saber.disableCutting)
                {
                    Utilities.Cutter(ref _saber).Cut(_saber, topPosition, bottomPosition, lastAddedData.topPos, lastAddedData.bottomPos);
                }
            }
        }

        public void SetType(SaberType type)
        {
            _saber.SetType(type, _colorManager);
        }

        public void ChangeType(SaberType type)
        {
            _saber.ChangeType(type);
        }

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
            _saber.ChangeColor(color);
        }

        /// <summary>
        /// Set the Saber that lies in this SiraSaber. Don't call unless you know what you're doing.
        /// </summary>
        /// <param name="saber"></param>
        public void SetSaber(Saber saber)
        {
            _saber = saber;
        }

        #region Zenject
        public class Factory : PlaceholderFactory<SiraSaber>
        {

        }

        public class SaberFactory : IFactory<SiraSaber>
        {
            private readonly DiContainer _container;

            public SaberFactory(DiContainer container)
            {
                _container = container;
            }

            public SiraSaber Create()
            {
                GameObject saberObject = new GameObject("SiraUtil Saber");
                SiraSaber sira = saberObject.AddComponent<SiraSaber>();
                _container.InjectGameObject(saberObject);
                return sira;
            }
        }
        #endregion Zenject
    }
}