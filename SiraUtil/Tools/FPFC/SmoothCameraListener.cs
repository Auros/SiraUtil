using JetBrains.Annotations;
using System.ComponentModel;
using UnityEngine;
using Zenject;

namespace SiraUtil.Tools.FPFC
{
    internal class SmoothCameraListener : MonoBehaviour
    {
        private IFPFCSettings? _fpfcSettings;
        private SmoothCameraController _smoothCameraController = null!;
        private SmoothCamera _smoothCamera = null!;

        [Inject]
        [UsedImplicitly]
        private void Construct(IFPFCSettings fpfcSettings, SmoothCameraController smoothCameraController)
        {
            _fpfcSettings = fpfcSettings;
            _smoothCameraController = smoothCameraController;
            _smoothCamera = smoothCameraController._smoothCamera;
        }

        protected void OnEnable()
        {
            if (_fpfcSettings != null)
            {
                _fpfcSettings.PropertyChanged += OnFPFCSettingsPropertyChanged;
                UpdateState();
            }
        }

        protected void Start() => OnEnable();

        protected void OnDisable()
        {
            _fpfcSettings?.PropertyChanged -= OnFPFCSettingsPropertyChanged;
        }

        private void OnFPFCSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IFPFCSettings.Enabled))
            {
                UpdateState();
            }
        }

        private void UpdateState()
        {
            if (_fpfcSettings!.Enabled)
            {
                _smoothCamera.enabled = false;
            }
            else
            {
                _smoothCameraController.ActivateSmoothCameraIfNeeded();
            }
        }
    }
}