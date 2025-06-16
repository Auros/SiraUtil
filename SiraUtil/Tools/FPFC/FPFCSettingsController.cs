using SiraUtil.Logging;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Management;
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

                if (value)
                    DeinitializeXRLoader();
                else
                    InitializeXRLoader();

                Changed?.Invoke(this);
            }
        }

        public bool LockViewOnDisable => _fpfcOptions.LockViewOnDisable;

        public bool LimitFrameRate => _fpfcOptions.LimitFrameRate;

        public int VSyncCount => _fpfcOptions.VSyncCount;

        public event Action<IFPFCSettings>? Changed;
        private readonly FPFCOptions _fpfcOptions;
        private readonly SiraLog _siraLog;

        public FPFCSettingsController(FPFCOptions fpfcOptions, SiraLog siraLog)
        {
            _fpfcOptions = fpfcOptions;
            _siraLog = siraLog;
        }

        public void Tick()
        {
            if (Input.GetKeyDown(_fpfcOptions.ToggleKeyCode))
            {
                Enabled = !Enabled;
            }
        }

        public void Initialize()
        {
            _fpfcOptions.Updated += ConfigUpdated;
            Enabled = !_fpfcOptions.Ignore;
        }

        private void ConfigUpdated(FPFCOptions _)
        {
            Changed?.Invoke(this);
        }

        public void Dispose()
        {
            _fpfcOptions.Updated -= ConfigUpdated;
        }

        // we unfortunately need to fully deinitialize/initialize the XR loader since OpenXR doesn't simply stop/start properly
        private void InitializeXRLoader()
        {
            XRManagerSettings manager = XRGeneralSettings.Instance.Manager;

            if (manager.activeLoader != null || !manager.activeLoaders.Any(l => l != null))
                return;

            _siraLog.Notice("Enabling XR Loader");
            manager.InitializeLoaderSync();

            if (!manager.isInitializationComplete)
            {
                _siraLog.Error("Failed to initialize XR loader");
                return;
            }

            manager.StartSubsystems();
        }

        private void DeinitializeXRLoader()
        {
            XRManagerSettings manager = XRGeneralSettings.Instance.Manager;

            if (manager.activeLoader == null)
                return;

            _siraLog.Notice("Disabling XR Loader");
            manager.DeinitializeLoader();
        }
    }
}