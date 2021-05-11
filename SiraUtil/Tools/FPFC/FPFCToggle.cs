using SiraUtil.Services;
using System;
using UnityEngine;
using VRUIControls;
using Zenject;

namespace SiraUtil.Tools.FPFC
{
    internal class FPFCToggle : IInitializable, IDisposable
    {
        private readonly MainCamera _mainCamera;
        private readonly IFPFCSettings _fpfcSettings;
        private readonly VRInputModule _vrInputModule;
        private readonly Transform _originalControllerWrapper;
        private readonly IMenuControllerAccessor _menuControllerAccessor;
        private SimpleCameraController _simpleCameraController = null!;
        private readonly FPFCState _initialState = new();

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
            _simpleCameraController = _mainCamera.camera.gameObject.AddComponent<SimpleCameraController>();
            Resources.FindObjectsOfTypeAll<FirstPersonFlyingController>()[0].enabled = false;
            _simpleCameraController.enabled = false;

            _initialState.CameraFOV = _mainCamera.camera.fieldOfView;

            if (_fpfcSettings.Enabled)
                EnableFPFC();
        }

        private void FPFCSettings_Changed(bool on)
        {
            if (on)
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
            _simpleCameraController.enabled = false;
            _mainCamera.camera.fieldOfView = _initialState.CameraFOV;
            _menuControllerAccessor.LeftController.transform.SetParent(_originalControllerWrapper);
            _menuControllerAccessor.RightController.transform.SetParent(_originalControllerWrapper);
            _menuControllerAccessor.LeftController.enabled = true;
            _menuControllerAccessor.RightController.enabled = true;
            _vrInputModule.useMouseForPressInput = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void Dispose()
        {
            _fpfcSettings.Changed -= FPFCSettings_Changed;
        }
    }
}