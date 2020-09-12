using System;
using Zenject;
using System.Linq;
using UnityEngine;
using VRUIControls;
using IPA.Utilities;

namespace SiraUtil.Tools
{
    public class FPFCToggle : MonoBehaviour
    {
        public bool Enabled { get; private set; } = false;

        private KeyCode _toggleCode;
        private VRInputModule _vrInputModule;
        private GameScenesManager _gameScenesManager;
        private FirstPersonFlyingController _firstPersonFlyingController;
        
        [Inject]
        public void Construct([Inject(Id = "ToggleCode")] KeyCode toggleCode, GameScenesManager gameScenesManager)
        {
            _toggleCode = toggleCode;
            _gameScenesManager = gameScenesManager;
            Enabled = Environment.GetCommandLineArgs().Any(x => x == "fpfc");
        }

        protected void OnEnable()
        {
            if (_gameScenesManager != null)
                _gameScenesManager.transitionDidFinishEvent += GameScenesManager_transitionDidFinishEvent;
        }

        protected void OnDisable()
        {
            if (_gameScenesManager != null)
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
            if (Container.TryResolve<IDifficultyBeatmap>() != null)
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