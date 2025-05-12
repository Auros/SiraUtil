using SiraUtil.Affinity;
using SiraUtil.Services;
using SiraUtil.Zenject;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SpatialTracking;
using VRUIControls;
using Object = UnityEngine.Object;

namespace SiraUtil.Tools.FPFC
{
    internal class FPFCToggle : IAffinity, IAsyncInitializable, IDisposable
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
        private readonly ParentConstraint _vrPointerConstraint;

        public FPFCToggle(MainCamera mainCamera, IFPFCSettings fpfcSettings, List<IFPFCListener> fpfcListeners, IMenuControllerAccessor menuControllerAccessor, VRInputModule vrInputModule)
        {
            _mainCamera = mainCamera;
            _fpfcSettings = fpfcSettings;
            _fpfcListeners = fpfcListeners;
            _menuControllerAccessor = menuControllerAccessor;

            _leftControllerConstraint = menuControllerAccessor.LeftController.gameObject.AddComponent<ParentConstraint>();
            _rightControllerConstraint = menuControllerAccessor.RightController.gameObject.AddComponent<ParentConstraint>();
            _vrPointerConstraint = vrInputModule.vrPointer.gameObject.AddComponent<ParentConstraint>();
        }

        [AffinityPatch(typeof(SettingsApplicatorSO), nameof(SettingsApplicatorSO.ApplyGraphicSettings))]
        private void ApplyGraphicSettings()
        {
            if (_fpfcSettings.Enabled)
            {
                Application.targetFrameRate = (int)Math.Round(Screen.currentResolution.refreshRateRatio.value);
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

            _lastStereoTargetEyeMask = _mainCamera.camera.stereoTargetEye;

            _simpleCameraController = _mainCamera.camera.gameObject.AddComponent<SimpleCameraController>();

            Transform cameraTransform = _mainCamera.transform;
            ConfigureConstraint(_leftControllerConstraint, cameraTransform);
            ConfigureConstraint(_rightControllerConstraint, cameraTransform);
            ConfigureConstraint(_vrPointerConstraint, cameraTransform);

            FPFCSettings_Changed(_fpfcSettings);
        }

        public void Dispose()
        {
            _fpfcSettings.Changed -= FPFCSettings_Changed;

            Object.Destroy(_leftControllerConstraint);
            Object.Destroy(_rightControllerConstraint);
            Object.Destroy(_vrPointerConstraint);
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

            SetConstraintEnabled(_menuControllerAccessor.LeftController, _leftControllerConstraint, true);
            SetConstraintEnabled(_menuControllerAccessor.RightController, _rightControllerConstraint, true);
            SetConstraintEnabled(_vrPointerConstraint, true);

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

            SetConstraintEnabled(_menuControllerAccessor.LeftController, _leftControllerConstraint, false);
            SetConstraintEnabled(_menuControllerAccessor.RightController, _rightControllerConstraint, false);
            SetConstraintEnabled(_vrPointerConstraint, false);

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ConfigureConstraint(ParentConstraint constraint, Transform sourceTransform)
        {
            constraint.AddSource(new ConstraintSource { sourceTransform = sourceTransform, weight = 1 });
            constraint.constraintActive = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetConstraintEnabled(VRController vrController, ParentConstraint parentConstraint, bool enabled)
        {
            vrController.enabled = !enabled;
            vrController.mouseMode = enabled;
            SetConstraintEnabled(parentConstraint, enabled);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetConstraintEnabled(ParentConstraint parentConstraint, bool enabled)
        {
            parentConstraint.enabled = enabled;
            parentConstraint.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
    }
}