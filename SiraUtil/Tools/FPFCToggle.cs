using System;
using Zenject;
using System.Linq;
using UnityEngine;
using VRUIControls;
using IPA.Utilities;
using UnityEngine.SceneManagement;

namespace SiraUtil.Tools
{
    /// <summary>
    /// The FPFC manager.
    /// </summary>
    public class FPFCToggle : MonoBehaviour
    {
        /// <summary>
        /// The current state of the FPFC Toggler.
        /// </summary>
        public bool Enabled { get; private set; } = false;

        private bool _start;
        private Camera _camera;

        private IVRPlatformHelper _helper;
        private bool _fpfcCommandArgument;
        private VRInputModule _vrInputModule;
        private VRController _leftController;
        private VRController _rightController;
        private VRLaserPointer _vrLaserPointer;
        private Config.FPFCToggleOptions _options;
        private Transform _originalOriginLocation;
        private FirstPersonFlyingController _firstPersonFlyingController;

        [Inject]
        internal void Construct(IVRPlatformHelper helper, Config.FPFCToggleOptions options)
        {
            _helper = helper;
            _options = options;
            _fpfcCommandArgument = Environment.GetCommandLineArgs().Any(x => x.ToLower() == "fpfc");
            _start = options.Enabled && _fpfcCommandArgument;
            Enabled = _start;
        }

        /// <summary>
        /// The start method.
        /// </summary>
        protected void Start()
        {
            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        }

        private async void SceneManager_activeSceneChanged(Scene oldScene, Scene newScene)
        {
            if (_start && (newScene.name == "MenuViewControllers" || newScene.name == "GameCore"))
            {
                await Utilities.PauseChamp;
                Toggle(!Enabled);
                await Utilities.PauseChamp;
                Toggle(!Enabled);

                if (_vrLaserPointer == null)
                {
                    Refresh();
                }
                _camera.transform.localPosition = Vector3.zero;
                _leftController.transform.localPosition = Vector3.zero;
                _rightController.transform.localPosition = Vector3.zero;
                _vrLaserPointer.gameObject.SetActive(newScene.name != "GameCore");
                if (newScene.name == "GameCore")
                {
                    foreach (Parametric3SliceSpriteController fakeGlow in _leftController.transform.GetComponentsInChildren<Parametric3SliceSpriteController>())
                    {
                        fakeGlow.enabled = false;
                    }
                    foreach (Parametric3SliceSpriteController fakeGlow in _rightController.transform.GetComponentsInChildren<Parametric3SliceSpriteController>())
                    {
                        fakeGlow.enabled = false;
                    }
                }
            }
        }

        /// <summary>
        /// The destroy method.
        /// </summary>
        protected void OnDestroy()
        {
            SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        }

        /// <summary>
        /// The update method.
        /// </summary>
        protected void Update()
        {
            if (Input.GetKeyDown(_options.ToggleKeyCode))
            {
                if (!_start)
                {
                    _start = true;
                    if (!_options.OnFirstRequest)
                    {
                        return;
                    }
                }
                Refresh();
                Toggle(!Enabled);
            }
        }

        /// <summary>
        /// Toggles FPFC
        /// </summary>
        /// <param name="state">The state to toggle it to (true or false, on or off).</param>
        public void Toggle(bool state)
        {
            if (_start)
            {
                if (_firstPersonFlyingController == null)
                {
                    Refresh();
                }
                _camera.enabled = false;
                _camera.enabled = true;

                _vrInputModule.enabled = state;
                _vrInputModule.gameObject.SetActive(true);
                _firstPersonFlyingController.enabled = state;
                Cursor.lockState = state ? CursorLockMode.Locked : CursorLockMode.None;
                Cursor.visible = !state;
                Enabled = state;

                if (_options.OnFirstRequest)
                {
                    BruteForce(state);
                }
            }
        }

        private void BruteForce(bool state)
        {
            if (state)
            {
                Refresh();
                _firstPersonFlyingController.enabled = true;
                _firstPersonFlyingController.Start();
            }
            else
            {
                _vrInputModule.transform.SetParent(null);
                _firstPersonFlyingController.transform.SetParent(_originalOriginLocation);
                _camera.stereoTargetEye = StereoTargetEyeMask.Both;
                foreach (var go in _firstPersonFlyingController.GetField<GameObject[], FirstPersonFlyingController>("_controllerModels"))
                {
                    if (go != null)
                    {
                        go.SetActive(true);
                    }
                }
                var adjust = _firstPersonFlyingController.GetField<VRCenterAdjust, FirstPersonFlyingController>("_centerAdjust");
                _firstPersonFlyingController.enabled = false;
                adjust.enabled = true;
                adjust.HandleRoomCenterDidChange();
                adjust.HandleRoomRotationDidChange();
                _rightController.enabled = true;
                _leftController.enabled = true;
            }
        }

        /// <summary>
        /// Refreshes the FPFC Variables.
        /// </summary>
        public void Refresh()
        {
            _firstPersonFlyingController = Resources.FindObjectsOfTypeAll<FirstPersonFlyingController>().FirstOrDefault();
            _vrInputModule = _firstPersonFlyingController.GetField<VRInputModule, FirstPersonFlyingController>("_vrInputModule");
            _camera = _firstPersonFlyingController.GetField<Camera, FirstPersonFlyingController>("_camera");
            _vrLaserPointer = _firstPersonFlyingController.GetComponentInChildren<VRLaserPointer>();
            _vrInputModule.transform.SetParent(transform);
            if (_originalOriginLocation == null)
            {
                _originalOriginLocation = _firstPersonFlyingController.transform;
            }
            if (_helper.currentXRDeviceModel == XRDeviceModel.Unknown)
            {
                _camera.fieldOfView = _options.CameraFOV;
            }
            _firstPersonFlyingController.transform.SetParent(transform);
            _firstPersonFlyingController.SetField("_cameraFov", _options.CameraFOV);
            _firstPersonFlyingController.SetField("_moveSensitivity", _options.MoveSensitivity);
            _leftController = _firstPersonFlyingController.GetField<VRController, FirstPersonFlyingController>("_controller0");
            _rightController = _firstPersonFlyingController.GetField<VRController, FirstPersonFlyingController>("_controller1");
        }
    }
}