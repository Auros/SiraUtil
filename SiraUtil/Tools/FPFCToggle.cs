using System;
using Zenject;
using System.Linq;
using UnityEngine;
using VRUIControls;
using IPA.Utilities;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace SiraUtil.Tools
{
    public class FPFCToggle : MonoBehaviour
    {
        public bool Enabled { get; private set; } = false;

        private bool _start;
        private VRInputModule _vrInputModule;
		private Config.FPFCToggleOptions _options;
        private FirstPersonFlyingController _firstPersonFlyingController;

        [Inject]
        public void Construct(Config.FPFCToggleOptions options)
        {
			_options = options;
            _start = Enabled = _options.Enabled && Environment.GetCommandLineArgs().Any(x => x.ToLower() == "fpfc");
        }

        protected void Start()
        {
			SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        }

		private async void SceneManager_activeSceneChanged(Scene oldScene, Scene newScene)
		{
			if (newScene.name == "MenuViewControllers" || newScene.name == "GameCore")
			{
				await Task.Run(() => Thread.Sleep(200));
				Toggle(!Enabled);
				await Task.Run(() => Thread.Sleep(200));
				Toggle(!Enabled);
			}
		}

		protected void OnDestroy()
        {
			SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
		}

        protected void Update()
        {
            if (Input.GetKeyDown(_options.ToggleKeyCode))
            {
                if (!_start)
                {
                    _start = true;
                    return;
                }
				Refresh();
				Toggle(!Enabled);
            }
        }

        public void Toggle(bool state)
        {
            if (_firstPersonFlyingController == null)
            {
				Refresh();
            }
            _firstPersonFlyingController.GetField<Camera, FirstPersonFlyingController>("_camera").enabled = false;
            _firstPersonFlyingController.GetField<Camera, FirstPersonFlyingController>("_camera").enabled = true;

            _vrInputModule.enabled = state;
            _vrInputModule.gameObject.SetActive(true);
            _firstPersonFlyingController.enabled = state;
            Cursor.lockState = state ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !state;
            Enabled = state;
        }

		public void Refresh()
		{
			_firstPersonFlyingController = Resources.FindObjectsOfTypeAll<FirstPersonFlyingController>().FirstOrDefault();
			_vrInputModule = _firstPersonFlyingController.GetField<VRInputModule, FirstPersonFlyingController>("_vrInputModule");
			_vrInputModule.transform.SetParent(transform);
			_firstPersonFlyingController.transform.SetParent(transform);

			_firstPersonFlyingController.SetField("_cameraFov", _options.CameraFOV);
			_firstPersonFlyingController.SetField("_moveSensitivity", _options.MoveSensitivity);
			_firstPersonFlyingController.GetField<Camera, FirstPersonFlyingController>("_camera").fieldOfView = _options.CameraFOV;
		}
    }
}