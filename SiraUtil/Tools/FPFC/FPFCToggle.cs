using SiraUtil.Services;
using SiraUtil.Zenject;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;
using VRUIControls;
using Zenject;

namespace SiraUtil.Tools.FPFC
{
    internal class FPFCToggle : IAsyncInitializable, ITickable, IDisposable
    {
        public const string Argument = "fpfc";

        private Pose? _lastPose = new();
        private readonly FPFCState _initialState = new();
        private SimpleCameraController _simpleCameraController = null!;

        private readonly MainCamera _mainCamera;
        private readonly EventSystem _eventSystem;
        private readonly IFPFCSettings _fpfcSettings;
        private readonly VRInputModule _vrInputModule;
        private readonly List<IFPFCListener> _fpfcListeners;
        private readonly IMenuControllerAccessor _menuControllerAccessor;
        private readonly Transform _previousEventSystemTransformParent;
        private bool _didFirstFocus = false;

        public FPFCToggle(MainCamera mainCamera, IFPFCSettings fpfcSettings, VRInputModule vrInputModule, List<IFPFCListener> fpfcListeners, IMenuControllerAccessor menuControllerAccessor)
        {
            _mainCamera = mainCamera;
            _fpfcSettings = fpfcSettings;
            _vrInputModule = vrInputModule;
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

            if (_fpfcSettings.Enabled)
                _mainCamera.camera.transform.parent.gameObject.transform.position = new Vector3(0f, 1.7f, 0f);
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

            if (_vrInputModule != null)
                _vrInputModule.useMouseForPressInput = true;

            if (_didFirstFocus)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            foreach (var listener in _fpfcListeners)
                listener.Enabled();
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

            if (_vrInputModule != null)
                _vrInputModule.useMouseForPressInput = false;

            _simpleCameraController.AllowInput = false;

            if (XRSettings.enabled)
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

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            foreach (var listener in _fpfcListeners)
                listener.Disabled();
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
    }
}