using IPA.Utilities;
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
        /// <remarks>
        /// Do not set the saber unless you know what you're doing.
        /// </remarks>
        public Saber Saber { get; set; } = null!;
        internal SaberModelController Model => _saberModelController;
        internal Action<Saber, Color> ColorUpdated { get; set; } = null!;

        private NoteCutter _noteCutter = null!;
        private ColorManager _colorManager = null!;
        private SaberTypeObject _saberTypeObject = null!;
        private SaberModelProvider _saberModelProvider = null!;
        private SaberModelController _saberModelController = null!;
        private Queue<Action> _colorProcessNextFrame = new();
        private bool _constructedThisFrame = false;

        private static readonly FieldAccessor<Saber, Vector3>.Accessor SaberBladeTopPosition = FieldAccessor<Saber, Vector3>.GetAccessor("_saberBladeTopPos");
        private static readonly FieldAccessor<Saber, Vector3>.Accessor SaberBladeBottomPosition = FieldAccessor<Saber, Vector3>.GetAccessor("_saberBladeBottomPos");

        private static readonly FieldAccessor<Saber, Transform>.Accessor SaberHandleTransform = FieldAccessor<Saber, Transform>.GetAccessor("_handleTransform");
        private static readonly FieldAccessor<Saber, Transform>.Accessor SaberBladeTopTransform = FieldAccessor<Saber, Transform>.GetAccessor("_saberBladeTopTransform");
        private static readonly FieldAccessor<Saber, Transform>.Accessor SaberBladeBottomTransform = FieldAccessor<Saber, Transform>.GetAccessor("_saberBladeBottomTransform");

        [Inject]
        internal void Construct(NoteCutter noteCutter, ColorManager colorManager, SaberModelProvider saberModelProvider)
        {
            _noteCutter = noteCutter;
            _colorManager = colorManager;
            _saberModelProvider = saberModelProvider;

            _saberTypeObject = gameObject.AddComponent<SaberTypeObject>();
            Saber saber = Saber = gameObject.AddComponent<Saber>();
            Saber.SetField("_saberType", _saberTypeObject);

            GameObject top = new("Top");
            GameObject bottom = new("Bottom");
            top.transform.SetParent(transform);
            bottom.transform.SetParent(transform);
            top.transform.position = new Vector3(0f, 0f, 1f);

            SaberBladeTopTransform(ref saber) = top.transform;
            SaberHandleTransform(ref saber) = bottom.transform;
            SaberBladeBottomTransform(ref saber) = bottom.transform;
            SaberBladeTopPosition(ref saber) = top.transform.position;
            SaberBladeBottomPosition(ref saber) = bottom.transform.position;
        }

        internal void SetInitialType(SaberType saberType)
        {
            _saberTypeObject.SetField("_saberType", saberType);
            _saberModelController = _saberModelProvider.NewModel(saberType);
            _saberModelController.Init(transform, Saber);
            _constructedThisFrame = true;
        }

        internal void Update()
        {
            if (Saber != null && Saber.gameObject.activeInHierarchy && !Saber.disableCutting)
            {
                Saber saber = Saber;
                Transform topTransform = SaberBladeTopTransform(ref saber);
                Transform bottomTransform = SaberBladeBottomTransform(ref saber);
                Vector3 topPosition = SaberBladeTopPosition(ref saber) = topTransform.position;
                Vector3 bottomPosition = SaberBladeBottomPosition(ref saber) = bottomTransform.position;
                Saber.movementData.AddNewData(topPosition, bottomPosition, TimeHelper.time);
                _noteCutter.Cut(Saber);
            }

            if (_colorProcessNextFrame.Count > 0)
                _colorProcessNextFrame.Dequeue().Invoke();
        }

        internal void LateUpdate()
        {
            _constructedThisFrame = false;
        }

        /// <summary>
        /// Sets the type of the saber.
        /// </summary>
        public void SetType(SaberType newSaberType)
        {
            _saberTypeObject.SetField("_saberType", newSaberType);
            _saberModelController.SetColor(_colorManager.ColorForSaberType(newSaberType));
        }

        /// <summary>
        /// Sets the color of the saber.
        /// </summary>
        /// <param name="newColor">The new color.</param>
        public void SetColor(Color newColor)
        {
            if (!_constructedThisFrame)
            {
                _saberModelController.SetColor(newColor);
                ColorUpdated?.Invoke(Saber, newColor);
                return;
            }
            else
            {
                // Sabers created on the same frame that the model was constructed wont have their colors be updated, so we have it so the color is only set once per frame.
                _colorProcessNextFrame.Enqueue(() =>
                {
                    _saberModelController.SetColor(newColor);
                    ColorUpdated?.Invoke(Saber, newColor);
                });
            }
        }
    }
}