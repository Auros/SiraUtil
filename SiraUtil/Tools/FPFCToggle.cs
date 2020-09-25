using System;
using Zenject;
using System.Linq;
using UnityEngine;
using VRUIControls;
using IPA.Utilities;
using System.Collections;

namespace SiraUtil.Tools
{
    public class FPFCToggle : MonoBehaviour
    {
        public bool Enabled { get; private set; } = false;

        private float _cameraFOV;
        private KeyCode _toggleCode;
        private float _moveSensitivity;
        private VRInputModule _vrInputModule;
        private GameScenesManager _gameScenesManager;
        private FirstPersonFlyingController _firstPersonFlyingController;
        private MenuScenesTransitionSetupDataSO _menuScenesTransitionSetupDataSO;

        [Inject]
        public void Construct([Inject(Id = "ToggleCode")] KeyCode toggleCode, [Inject(Id = "CameraFOV")] float cameraFOV, [Inject(Id = "MoveSensitivity")] float moveSensitivity, GameScenesManager gameScenesManager, MenuScenesTransitionSetupDataSO menuScenesTransitionSetupDataSO)
        {
            _cameraFOV = cameraFOV;
            _toggleCode = toggleCode;
            _moveSensitivity = moveSensitivity;
            _gameScenesManager = gameScenesManager;
            Enabled = Environment.GetCommandLineArgs().Any(x => x == "fpfc");
            _menuScenesTransitionSetupDataSO = menuScenesTransitionSetupDataSO;
        }

        protected void Start()
        {
            _gameScenesManager.transitionDidFinishEvent += GameScenesManager_transitionDidFinishEvent;
        }

        protected void OnDestroy()
        {
            _gameScenesManager.transitionDidFinishEvent -= GameScenesManager_transitionDidFinishEvent;
        }

        protected void Update()
        {
            if (Input.GetKeyDown(_toggleCode))
            {
                Toggle(!Enabled);
            }
        }

        private void GameScenesManager_transitionDidFinishEvent(ScenesTransitionSetupDataSO setupData, DiContainer Container)
        {
            if ((setupData == _menuScenesTransitionSetupDataSO || Container.TryResolve<IDifficultyBeatmap>() != null) && Enabled)
            {
                Toggle(!true);
                Toggle(!false);
            }
        }

        public void Toggle(bool state)
        {
            if (_firstPersonFlyingController == null)
            {
                _firstPersonFlyingController = Resources.FindObjectsOfTypeAll<FirstPersonFlyingController>().FirstOrDefault();
                _vrInputModule = _firstPersonFlyingController.GetField<VRInputModule, FirstPersonFlyingController>("_vrInputModule");
                _vrInputModule.transform.SetParent(transform);
                _firstPersonFlyingController.transform.SetParent(transform);

                _firstPersonFlyingController.SetField("_cameraFov", _cameraFOV);
                _firstPersonFlyingController.SetField("_moveSensitivity", _moveSensitivity);
                _firstPersonFlyingController.GetField<Camera, FirstPersonFlyingController>("_camera").fieldOfView = _cameraFOV;
            }

            _vrInputModule.enabled = state;
            _vrInputModule.gameObject.SetActive(true);
            _firstPersonFlyingController.enabled = state;
            _firstPersonFlyingController.gameObject.SetActive(state);
            Cursor.lockState = state ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !state;
            Enabled = state;
        }
    }
}