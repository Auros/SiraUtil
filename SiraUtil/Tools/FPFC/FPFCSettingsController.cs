using System;
using UnityEngine;
using Zenject;

namespace SiraUtil.Tools.FPFC
{
    internal class FPFCSettingsController : IFPFCSettings, IInitializable, ITickable, IDisposable
    {
        private bool _enabled = true;
        public bool Ignore => _fpfcOptions.Ignore;
        public float FOV => _fpfcOptions.CameraFOV;
        public float MoveSensitivity => _fpfcOptions.MoveSensitivity;
        public float MouseSensitivity => _fpfcOptions.MouseSensitivity;

        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                Changed?.Invoke(this);
            }
        }

        public event Action<IFPFCSettings>? Changed;
        private readonly FPFCOptions _fpfcOptions;

        public FPFCSettingsController(FPFCOptions fpfcOptions)
        {
            _fpfcOptions = fpfcOptions;
            Enabled = !_fpfcOptions.Ignore;
        }

        public void Tick()
        {
            if (Input.GetKeyDown(_fpfcOptions.ToggleKeyCode))
            {
                Enabled = !Enabled;
                Changed?.Invoke(this);
            }
        }

        public void Initialize()
        {
            _fpfcOptions.Updated += ConfigUpdated;
        }

        private void ConfigUpdated(FPFCOptions _)
        {
            Changed?.Invoke(this);
        }

        public void Dispose()
        {
            _fpfcOptions.Updated -= ConfigUpdated;
        }
    }
}