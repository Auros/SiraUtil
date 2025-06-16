using SiraUtil.Affinity;
using SiraUtil.Services;
using SiraUtil.Zenject;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SpatialTracking;
using Object = UnityEngine.Object;

namespace SiraUtil.Tools.FPFC
{
    internal class FPFCToggle : IAffinity, IAsyncInitializable, IDisposable
    {
        public const string EnableArgument = "fpfc";
        public const string DisableArgument = "--no-sirautil-fpfc";

        private Pose? _lastPose = new();
        private StereoTargetEyeMask _initialStereoTargetEyeMask;
        private SimpleCameraController _simpleCameraController = null!;

        private readonly MainCamera _mainCamera;
        private readonly IFPFCSettings _fpfcSettings;
        private readonly List<IFPFCListener> _fpfcListeners;
        private readonly IMenuControllerAccessor _menuControllerAccessor;

        public FPFCToggle(MainCamera mainCamera, IFPFCSettings fpfcSettings, List<IFPFCListener> fpfcListeners, IMenuControllerAccessor menuControllerAccessor)
        {
            _mainCamera = mainCamera;
            _fpfcSettings = fpfcSettings;
            _fpfcListeners = fpfcListeners;
            _menuControllerAccessor = menuControllerAccessor;
        }

        [AffinityPatch(typeof(SettingsApplicatorSO), nameof(SettingsApplicatorSO.ApplyGraphicSettings))]
        private void ApplyGraphicSettings()
        {
            if (_fpfcSettings.Enabled)
            {
                if (_fpfcSettings.LimitFrameRate)
                {
                    Application.targetFrameRate = (int)Math.Round(Screen.currentResolution.refreshRateRatio.value);
                }

                QualitySettings.vSyncCount = _fpfcSettings.VSyncCount;
            }
        }

        public async Task InitializeAsync(CancellationToken token)
        {
            _fpfcSettings.Changed += FPFCSettings_Changed;

            if (_mainCamera.camera == null)
            {
                while (_mainCamera.camera == null)
                {
                    await Task.Yield();
                }
            }

            _initialStereoTargetEyeMask = _mainCamera.camera.stereoTargetEye;

            _simpleCameraController = _mainCamera.camera.gameObject.AddComponent<SimpleCameraController>();
            _simpleCameraController.StateChanged += OnCameraControllerStateChanged;

            FPFCSettings_Changed(_fpfcSettings);
        }

        public void Dispose()
        {
            _fpfcSettings.Changed -= FPFCSettings_Changed;

            if (_simpleCameraController != null)
            {
                _simpleCameraController.StateChanged -= OnCameraControllerStateChanged;
                Object.Destroy(_simpleCameraController);
            }
        }

        private void FPFCSettings_Changed(IFPFCSettings fpfcSettings)
        {
            if (_simpleCameraController == null)
            {
                return;
            }

            if (fpfcSettings.Enabled)
            {
                EnableFPFC();
            }
            else
            {
                DisableFPFC();
            }
        }

        private void EnableFPFC()
        {
            _simpleCameraController.enabled = true;

            _simpleCameraController.MoveSensitivity = _fpfcSettings.MoveSensitivity;
            _simpleCameraController.MouseSensitivity = _fpfcSettings.MouseSensitivity;

            if (_lastPose.HasValue)
            {
                _simpleCameraController.transform.SetPositionAndRotation(_lastPose.Value.position, _lastPose.Value.rotation);
            }

            if (_mainCamera != null)
            {
                Camera camera = _mainCamera.camera;

                if (camera != null)
                {
                    camera.stereoTargetEye = StereoTargetEyeMask.None;
                    camera.fieldOfView = _fpfcSettings.FOV;
                    camera.ResetAspect();
                }

                if (_mainCamera.TryGetComponent(out TrackedPoseDriver trackedPoseDriver))
                {
                    trackedPoseDriver.enabled = false;
                }
            }

            SetControllerEnabled(_menuControllerAccessor.LeftController, false);
            SetControllerEnabled(_menuControllerAccessor.RightController, false);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Application.targetFrameRate = _fpfcSettings.LimitFrameRate ? (int)Math.Round(Screen.currentResolution.refreshRateRatio.value) : -1;
            QualitySettings.vSyncCount = _fpfcSettings.VSyncCount;

            foreach (var listener in _fpfcListeners)
            {
                listener.Enabled();
            }
        }

        private void DisableFPFC()
        {
            _simpleCameraController.enabled = false;

            SetControllerEnabled(_menuControllerAccessor.LeftController, true);
            SetControllerEnabled(_menuControllerAccessor.RightController, true);

            if (!_fpfcSettings.LockViewOnDisable)
            {
                _lastPose = new Pose(_simpleCameraController.transform.position, _simpleCameraController.transform.rotation);

                if (_mainCamera != null)
                {
                    Camera camera = _mainCamera.camera;

                    if (camera != null)
                    {
                        camera.stereoTargetEye = _initialStereoTargetEyeMask;
                    }

                    if (_mainCamera.TryGetComponent(out TrackedPoseDriver trackedPoseDriver))
                    {
                        trackedPoseDriver.enabled = true;
                    }
                }
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Application.targetFrameRate = -1;
            QualitySettings.vSyncCount = 0;

            foreach (var listener in _fpfcListeners)
            {
                listener.Disabled();
            }
        }

        private void OnCameraControllerStateChanged(Vector3 position, Quaternion rotation)
        {
            _menuControllerAccessor.LeftController.transform.SetPositionAndRotation(position, rotation);
            _menuControllerAccessor.RightController.transform.SetPositionAndRotation(position, rotation);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetControllerEnabled(VRController vrController, bool enabled)
        {
            vrController.enabled = enabled;
            vrController.mouseMode = !enabled;
        }
    }
}