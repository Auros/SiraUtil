using SiraUtil.Services;
using System;
using UnityEngine;
using UnityEngine.XR;
using VRUIControls;
using Zenject;

namespace SiraUtil.Tools.FPFC
{
    internal class FPFCToggle : IInitializable, IDisposable
    {
        public const string Argument = "fpfc";

        private Pose? _lastPose = new();
        private readonly FPFCState _initialState = new();
        private SimpleCameraController _simpleCameraController = null!;

        private readonly MainCamera _mainCamera;
        private readonly IFPFCSettings _fpfcSettings;
        private readonly VRInputModule _vrInputModule;
        private readonly Transform _originalControllerWrapper;
        private readonly IMenuControllerAccessor _menuControllerAccessor;

        public FPFCToggle(MainCamera mainCamera, IFPFCSettings fpfcSettings, VRInputModule vrInputModule, IMenuControllerAccessor menuControllerAccessor)
        {
            _mainCamera = mainCamera;
            _fpfcSettings = fpfcSettings;
            _vrInputModule = vrInputModule;
            _menuControllerAccessor = menuControllerAccessor;
            _originalControllerWrapper = menuControllerAccessor.LeftController.transform.parent;
        }

        public void Initialize()
        {
            _fpfcSettings.Changed += FPFCSettings_Changed;
            _simpleCameraController = _mainCamera.camera.transform.parent.gameObject.AddComponent<SimpleCameraController>();

            _initialState.Aspect = _mainCamera.camera.aspect;
            _initialState.CameraFOV = _mainCamera.camera.fieldOfView;
            _initialState.StereroTarget = _mainCamera.camera.stereoTargetEye;

            if (_fpfcSettings.Enabled)
                EnableFPFC();
        }

        private void FPFCSettings_Changed(IFPFCSettings fpfcSettings)
        {
            if (fpfcSettings.Enabled)
            {
                if (!_simpleCameraController.AllowInput)
                    EnableFPFC();
                else
                {
                    _mainCamera.camera.fieldOfView = fpfcSettings.FOV;
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

            _simpleCameraController.MouseSensitivity = _fpfcSettings.MouseSensitivity;
            if (_lastPose.HasValue)
            {
                _simpleCameraController.transform.position = _lastPose.Value.position;
                _simpleCameraController.transform.rotation = _lastPose.Value.rotation;
            }
            _mainCamera.camera.stereoTargetEye = StereoTargetEyeMask.None;
            _mainCamera.camera.aspect = Screen.width / (float)Screen.height;
            _mainCamera.camera.fieldOfView = _fpfcSettings.FOV;

            _menuControllerAccessor.LeftController.transform.SetParent(_simpleCameraController.transform);
            _menuControllerAccessor.RightController.transform.SetParent(_simpleCameraController.transform);
            _menuControllerAccessor.LeftController.transform.localPosition = Vector3.zero;
            _menuControllerAccessor.RightController.transform.localPosition = Vector3.zero;
            _menuControllerAccessor.LeftController.transform.localRotation = Quaternion.identity;
            _menuControllerAccessor.RightController.transform.localRotation = Quaternion.identity;
            _menuControllerAccessor.LeftController.enabled = false;
            _menuControllerAccessor.RightController.enabled = false;

            _vrInputModule.useMouseForPressInput = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void DisableFPFC()
        {
            _menuControllerAccessor.LeftController.transform.SetParent(_originalControllerWrapper);
            _menuControllerAccessor.RightController.transform.SetParent(_originalControllerWrapper);
            _menuControllerAccessor.LeftController.enabled = true;
            _menuControllerAccessor.RightController.enabled = true;
            _vrInputModule.useMouseForPressInput = false;
            _simpleCameraController.AllowInput = false;

            if (XRSettings.enabled)
            {
                _lastPose = new Pose(_simpleCameraController.transform.position, _simpleCameraController.transform.rotation);
                _simpleCameraController.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

                _mainCamera.camera.aspect = _initialState.Aspect;
                _mainCamera.camera.fieldOfView = _initialState.CameraFOV;
                _mainCamera.camera.stereoTargetEye = _initialState.StereroTarget;
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void Dispose()
        {
            _fpfcSettings.Changed -= FPFCSettings_Changed;
        }
    }
}