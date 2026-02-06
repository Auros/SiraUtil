using BeatmapEditor3D;
using JetBrains.Annotations;
using System.ComponentModel;
using UnityEngine;
using Zenject;

namespace SiraUtil.Tools.FPFC
{
    internal class EditBeatmapNavigationFpfcToggleController : MonoBehaviour
    {
        private IFPFCSettings? _fpfcSettings;
        private EditBeatmapNavigationViewController _viewController = null!;

        [Inject]
        [UsedImplicitly]
        private void Construct(IFPFCSettings fpfcSettings)
        {
            _fpfcSettings = fpfcSettings;
        }

        protected void Awake()
        {
            _viewController = GetComponent<EditBeatmapNavigationViewController>();
        }

        protected void OnEnable()
        {
            if (_fpfcSettings == null)
            {
                return;
            }

            _fpfcSettings.PropertyChanged += OnFpfcSettingsPropertyChanged;
            UpdateState();
        }

        protected void Start() => OnEnable();

        protected void OnDisable()
        {
            if (_fpfcSettings == null)
            {
                return;
            }

            _fpfcSettings.PropertyChanged -= OnFpfcSettingsPropertyChanged;
        }

        private void OnFpfcSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IFPFCSettings.Enabled))
            {
                UpdateState();
            }
        }

        private void UpdateState()
        {
            _viewController._enableFPFCToggle.isOn = _fpfcSettings!.Enabled;
        }
    }
}
