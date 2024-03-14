using SiraUtil.Logging;
using SiraUtil.Services;
using SiraUtil.Zenject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SpatialTracking;
using UnityEngine.XR.Management;
using VRUIControls;
using Zenject;

namespace SiraUtil.Tools.FPFC
{
    internal class FPFCToggle : IAsyncInitializable, ITickable, IDisposable
    {
        public const string EnableArgument = "fpfc";
        public const string DisableArgument = "--no-sirautil-fpfc";

        private Pose? _lastPose = new();
        private readonly FPFCState _initialState = new();
        private SimpleCameraController _simpleCameraController = null!;

        private readonly SiraLog _siraLog;
        private readonly MainCamera _mainCamera;
        private readonly EventSystem _eventSystem;
        private readonly IFPFCSettings _fpfcSettings;
        private readonly List<IFPFCListener> _fpfcListeners;
        private readonly IMenuControllerAccessor _menuControllerAccessor;
        private readonly Transform _previousEventSystemTransformParent;
        private bool _didFirstFocus = false;

        public FPFCToggle(SiraLog siraLog, MainCamera mainCamera, IFPFCSettings fpfcSettings, VRInputModule vrInputModule, List<IFPFCListener> fpfcListeners, IMenuControllerAccessor menuControllerAccessor)
        {
            _siraLog = siraLog;
            _mainCamera = mainCamera;
            _fpfcSettings = fpfcSettings;
            _fpfcListeners = fpfcListeners;
            _menuControllerAccessor = menuControllerAccessor;

            _eventSystem = vrInputModule.GetComponent<EventSystem>();
            _previousEventSystemTransformParent = _eventSystem.transform.parent;

            _didFirstFocus = Application.isFocused;
        }

        public async Task InitializeAsync(CancellationToken token)
        {
            _fpfcSettings.Changed += FPFCSettings_Changed;

            if (_mainCamera.camera == null)
                while (_mainCamera.camera == null)
                    await Task.Yield();

            _initialState.Aspect = _mainCamera.camera.aspect;
            _initialState.CameraFOV = _mainCamera.camera.fieldOfView;
            _initialState.StereroTarget = _mainCamera.camera.stereoTargetEye;

            _simpleCameraController = _mainCamera.camera.transform.parent.gameObject.AddComponent<SimpleCameraController>();
            if (_fpfcSettings.Enabled)
                EnableFPFC();
        }

        private void FPFCSettings_Changed(IFPFCSettings fpfcSettings)
        {
            if (_simpleCameraController == null || !_simpleCameraController.enabled)
                return;

            if (fpfcSettings.Enabled)
            {
                if (!_simpleCameraController.AllowInput)
                    EnableFPFC();
                else
                {
                    if (_mainCamera != null && _mainCamera.camera != null)
                        _mainCamera.camera.fieldOfView = fpfcSettings.FOV;
                    _simpleCameraController.MoveSensitivity = _fpfcSettings.MoveSensitivity;
                    _simpleCameraController.MouseSensitivity = _fpfcSettings.MouseSensitivity;
                }
            }
            else if (_simpleCameraController.AllowInput)
            {
                DisableFPFC();
            }
        }

        private void EnableFPFC()
        {
            _simpleCameraController.AllowInput = true;
            _simpleCameraController.MoveSensitivity = _fpfcSettings.MoveSensitivity;
            _simpleCameraController.MouseSensitivity = _fpfcSettings.MouseSensitivity;
            if (_lastPose.HasValue)
            {
                _simpleCameraController.transform.position = _lastPose.Value.position;
                _simpleCameraController.transform.rotation = _lastPose.Value.rotation;
            }

            if (_mainCamera != null && _mainCamera.camera != null)
            {
                _mainCamera.camera.stereoTargetEye = StereoTargetEyeMask.None;
                _mainCamera.camera.aspect = Screen.width / (float)Screen.height;
                _mainCamera.camera.fieldOfView = _fpfcSettings.FOV;
            }

            if (_eventSystem != null)
                _eventSystem.gameObject.transform.SetParent(_simpleCameraController.transform);

            if (_menuControllerAccessor.LeftController != null && _menuControllerAccessor.RightController != null)
            {
                _menuControllerAccessor.LeftController.transform.SetParent(_simpleCameraController.transform);
                _menuControllerAccessor.RightController.transform.SetParent(_simpleCameraController.transform);
                _menuControllerAccessor.LeftController.transform.localPosition = Vector3.zero;
                _menuControllerAccessor.RightController.transform.localPosition = Vector3.zero;
                _menuControllerAccessor.LeftController.transform.localRotation = Quaternion.identity;
                _menuControllerAccessor.RightController.transform.localRotation = Quaternion.identity;
                _menuControllerAccessor.LeftController.enabled = false;
                _menuControllerAccessor.RightController.enabled = false;
            }

            if (_didFirstFocus)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (_mainCamera != null)
            {
                var poseDriver = _mainCamera.GetComponent<TrackedPoseDriver>();
                if (poseDriver != null)
                    poseDriver.enabled = false;
            }

            foreach (var listener in _fpfcListeners)
                listener.Enabled();

            DeinitializeXRLoader();
        }

        private void DisableFPFC()
        {
            if (_eventSystem != null)
                _eventSystem.gameObject.transform.SetParent(_previousEventSystemTransformParent);

            if (_menuControllerAccessor.LeftController != null && _menuControllerAccessor.RightController != null)
            {
                _menuControllerAccessor.LeftController!.transform.SetParent(_menuControllerAccessor.Parent);
                _menuControllerAccessor.RightController.transform.SetParent(_menuControllerAccessor.Parent);
                _menuControllerAccessor.LeftController.enabled = true;
                _menuControllerAccessor.RightController.enabled = true;
            }

            _simpleCameraController.AllowInput = false;

            if (!_fpfcSettings.LockViewOnDisable)
            {
                _lastPose = new Pose(_simpleCameraController.transform.position, _simpleCameraController.transform.rotation);
                _simpleCameraController.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

                if (_mainCamera != null && _mainCamera.camera != null)
                {
                    _mainCamera.camera.aspect = _initialState.Aspect;
                    _mainCamera.camera.fieldOfView = _initialState.CameraFOV;
                    _mainCamera.camera.stereoTargetEye = _initialState.StereroTarget;
                }
            }

            if (_mainCamera != null)
            {
                var poseDriver = _mainCamera.GetComponent<TrackedPoseDriver>();
                if (poseDriver != null)
                    poseDriver.enabled = true;
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            foreach (var listener in _fpfcListeners)
                listener.Disabled();

            InitializeXRLoader();
        }

        public void Dispose()
        {
            _fpfcSettings.Changed -= FPFCSettings_Changed;
        }

        public void Tick()
        {
            if (!_didFirstFocus && Application.isFocused && _fpfcSettings.Enabled)
            {
                _didFirstFocus = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
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