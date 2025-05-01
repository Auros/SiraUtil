using SiraUtil.Services;
using SiraUtil.Zenject;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SpatialTracking;

namespace SiraUtil.Tools.FPFC
{
    internal class FPFCToggle : IAsyncInitializable, IDisposable
    {
        public const string EnableArgument = "fpfc";
        public const string DisableArgument = "--no-sirautil-fpfc";

        private Pose? _lastPose = new();
        private StereoTargetEyeMask _lastStereoTargetEyeMask;
        private SimpleCameraController _simpleCameraController = null!;

        private readonly MainCamera _mainCamera;
        private readonly IFPFCSettings _fpfcSettings;
        private readonly List<IFPFCListener> _fpfcListeners;
        private readonly IMenuControllerAccessor _menuControllerAccessor;

        private readonly ParentConstraint _leftControllerConstraint;
        private readonly ParentConstraint _rightControllerConstraint;

        public FPFCToggle(MainCamera mainCamera, IFPFCSettings fpfcSettings, List<IFPFCListener> fpfcListeners, IMenuControllerAccessor menuControllerAccessor)
        {
            _mainCamera = mainCamera;
            _fpfcSettings = fpfcSettings;
            _fpfcListeners = fpfcListeners;
            _menuControllerAccessor = menuControllerAccessor;

            _leftControllerConstraint = menuControllerAccessor.LeftController.gameObject.AddComponent<ParentConstraint>();
            _rightControllerConstraint = menuControllerAccessor.RightController.gameObject.AddComponent<ParentConstraint>();
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

            _lastStereoTargetEyeMask = _mainCamera.camera.stereoTargetEye;

            _simpleCameraController = _mainCamera.camera.gameObject.AddComponent<SimpleCameraController>();

            Transform cameraTransform = _mainCamera.transform;
            _leftControllerConstraint.AddSource(new ConstraintSource { sourceTransform = cameraTransform, weight = 1 });
            _rightControllerConstraint.AddSource(new ConstraintSource { sourceTransform = cameraTransform, weight = 1 });

            FPFCSettings_Changed(_fpfcSettings);
        }

        public void Dispose()
        {
            _fpfcSettings.Changed -= FPFCSettings_Changed;
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
                _simpleCameraController.transform.position = _lastPose.Value.position;
                _simpleCameraController.transform.rotation = _lastPose.Value.rotation;
            }

            if (_mainCamera != null)
            {
                Camera camera = _mainCamera.camera;

                if (camera != null)
                {
                    _lastStereoTargetEyeMask = camera.stereoTargetEye;

                    camera.stereoTargetEye = StereoTargetEyeMask.None;
                    camera.fieldOfView = _fpfcSettings.FOV;
                    camera.ResetAspect();
                }

                if (_mainCamera.TryGetComponent(out TrackedPoseDriver trackedPoseDriver))
                {
                    trackedPoseDriver.enabled = false;
                }
            }

            SetControllerEnabled(_menuControllerAccessor.LeftController, _leftControllerConstraint, false);
            SetControllerEnabled(_menuControllerAccessor.RightController, _rightControllerConstraint, false);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (_fpfcSettings.LimitFrameRate)
            {
                Application.targetFrameRate = (int)Math.Round(Screen.currentResolution.refreshRateRatio.value);
            }

            foreach (var listener in _fpfcListeners)
            {
                listener.Enabled();
            }
        }

        private void DisableFPFC()
        {
            _simpleCameraController.enabled = false;

            SetControllerEnabled(_menuControllerAccessor.LeftController, _leftControllerConstraint, true);
            SetControllerEnabled(_menuControllerAccessor.RightController, _rightControllerConstraint, true);

            if (!_fpfcSettings.LockViewOnDisable)
            {
                _lastPose = new Pose(_simpleCameraController.transform.position, _simpleCameraController.transform.rotation);

                if (_mainCamera != null)
                {
                    Camera camera = _mainCamera.camera;

                    if (camera != null)
                    {
                        camera.stereoTargetEye = _lastStereoTargetEyeMask;
                    }

                    if (_mainCamera.TryGetComponent(out TrackedPoseDriver trackedPoseDriver))
                    {
                        trackedPoseDriver.enabled = true;
                    }
                }
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (_fpfcSettings.LimitFrameRate)
            {
                Application.targetFrameRate = -1;
            }

            foreach (var listener in _fpfcListeners)
            {
                listener.Disabled();
            }
        }

        private void SetControllerEnabled(VRController vrController, ParentConstraint parentConstraint, bool enabled)
        {
            vrController.enabled = enabled;
            vrController.mouseMode = !enabled;
            parentConstraint.constraintActive = !enabled;
        }
    }
}