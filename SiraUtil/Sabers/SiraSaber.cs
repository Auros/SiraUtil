using SiraUtil.Extras;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SiraUtil.Sabers
{
    /// <summary>
    /// A SiraSaber is an extra saber with some useful extension methods. The SiraSaber object is on the same GameObject as the normal Saber object, it's not an overridden version of the default Saber class.
    /// </summary>
    public class SiraSaber : MonoBehaviour
    {
        /// <summary>
        /// The saber this <see cref="SiraSaber"/> is referencing.
        /// </summary>
        public Saber Saber { get; private set; } = null!;
        internal SaberModelController Model { get; private set; } = null!;
        internal Action<Saber, Color> ColorUpdated { get; set; } = null!;

        private NoteCutter _noteCutter = null!;
        private ColorManager _colorManager = null!;
        private SaberTypeObject _saberTypeObject = null!;
        private SaberModelProvider _saberModelProvider = null!;
        private SaberModelContainer.InitData _saberModelContainerInitData = new();
        private TimeHelper _timeHelper = null!;
        private readonly Queue<Action> _colorProcessNextFrame = new();
        private bool _constructedThisFrame = false;

        [Inject]
        internal void Construct(
            NoteCutter noteCutter,
            ColorManager colorManager,
            SaberModelProvider saberModelProvider,
            [InjectOptional] SaberModelContainer.InitData saberModelContainerInitData,
            TimeHelper timeHelper)
        {
            _noteCutter = noteCutter;
            _colorManager = colorManager;
            _saberModelProvider = saberModelProvider;
            _timeHelper = timeHelper;

            if (saberModelContainerInitData != null)
            {
                _saberModelContainerInitData = saberModelContainerInitData;
            }
        }

        internal void Setup<T>(SaberType saberType) where T : Saber
        {
            _saberTypeObject = gameObject.AddComponent<SaberTypeObject>();
            Saber saber = Saber = gameObject.AddComponent<T>();
            gameObject.layer = LayerMask.NameToLayer("Saber");
            Saber._saberType = _saberTypeObject;

            GameObject top = new("Top");
            GameObject bottom = new("Bottom");
            top.transform.SetParent(transform);
            bottom.transform.SetParent(transform);
            top.transform.position = new Vector3(0f, 0f, 1f);

            saber._saberBladeTopTransform = top.transform;
            saber._handleTransform = bottom.transform;
            saber._saberBladeBottomTransform = bottom.transform;
            saber._saberBladeTopPos = top.transform.position;
            saber._saberBladeBottomPos = bottom.transform.position;

            _saberTypeObject._saberType = saberType;
            Model = _saberModelProvider.NewModel(saberType);
            Model.Init(transform, Saber, _saberModelContainerInitData.trailTintColor);
            _constructedThisFrame = true;
        }

        /// <inheritdoc/>
        protected void Update()
        {
            if (Saber != null && Saber.gameObject.activeInHierarchy && Saber.enabled)
            {
                Saber saber = Saber;
                Transform topTransform = saber._saberBladeTopTransform;
                Transform bottomTransform = saber._saberBladeBottomTransform;
                Vector3 topPosition = saber._saberBladeTopPos = topTransform.position;
                Vector3 bottomPosition = saber._saberBladeBottomPos = bottomTransform.position;
                Saber.movementDataForLogic.AddNewData(topPosition, bottomPosition, _timeHelper.Time);
                _noteCutter.Cut(Saber);
            }

            if (_colorProcessNextFrame.Count > 0)
            {
                _colorProcessNextFrame.Dequeue().Invoke();
            }
        }

        /// <inheritdoc/>
        protected void LateUpdate()
        {
            _constructedThisFrame = false;
        }

        /// <summary>
        /// Sets the type of the saber.
        /// </summary>
        public void SetType(SaberType newSaberType)
        {
            _saberTypeObject._saberType = newSaberType;
            Model.SetColor(_colorManager.ColorForSaberType(newSaberType));
        }

        /// <summary>
        /// Sets the color of the saber.
        /// </summary>
        /// <param name="newColor">The new color.</param>
        public void SetColor(Color newColor)
        {
            if (!_constructedThisFrame)
            {
                Model.SetColor(newColor);
                ColorUpdated?.Invoke(Saber, newColor);
                return;
            }
            else
            {
                // Sabers created on the same frame that the model was constructed wont have their colors be updated, so we have it so the color is only set once per frame.
                _colorProcessNextFrame.Enqueue(() =>
                {
                    Model.SetColor(newColor);
                    ColorUpdated?.Invoke(Saber, newColor);
                });
            }
        }
    }
}