using SiraUtil.Services;
using UnityEngine;
using Zenject;

namespace SiraUtil.Tools.FPFC
{
    internal class FPFCToggle : IInitializable
    {
        private readonly MainCamera _mainCamera;
        private readonly IMenuControllerAccessor _menuControllerAccessor;

        public FPFCToggle(MainCamera mainCamera, IMenuControllerAccessor menuControllerAccessor)
        {
            _mainCamera = mainCamera;
            _menuControllerAccessor = menuControllerAccessor;
        }

        public void Initialize()
        {
            Plugin.Log.Info(_mainCamera.GetHashCode().ToString());
            _mainCamera.camera.gameObject.AddComponent<SimpleCameraController>();
            UnityEngine.Resources.FindObjectsOfTypeAll<FirstPersonFlyingController>()[0].enabled = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}