using JetBrains.Annotations;
using SiraUtil.Services;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SpatialTracking;
using Zenject;

namespace SiraUtil.Tools.FPFC
{
    // This is a modified version of Unity's SimpleCameraController which is included in the Universal Render Pipeline template.
    internal class CameraController : MonoBehaviour
    {
        private readonly CameraState targetCameraState = new(new Vector3(0, 1.7f, 0), Quaternion.identity);

        private IFPFCManager? _fpfcManager;
        private List<IFPFCListener> _fpfcListeners = null!;
        private IMenuControllerAccessor? _menuControllerAccessor;
        private PauseController? _pauseController;

        private Camera _camera = null!;
        private TrackedPoseDriver _trackedPoseDriver = null!;

        private StereoTargetEyeMask _initialStereoTargetEyeMask;

        private float mouseSensitivity = 5f;
        private float moveSensitivity = 3f;
        private bool invertY = true;

        [Inject]
        [UsedImplicitly]
        private void Construct(IFPFCManager fpfcManager, List<IFPFCListener> fpfcListeners, [InjectOptional] IMenuControllerAccessor? menuControllerAccessor, [InjectOptional] PauseController? pauseController)
        {
            _fpfcManager = fpfcManager;
            _fpfcListeners = fpfcListeners;
            _menuControllerAccessor = menuControllerAccessor;
            _pauseController = pauseController;
        }

        protected void Awake()
        {
            _camera = GetComponent<Camera>();
            _trackedPoseDriver = GetComponent<TrackedPoseDriver>();

            _initialStereoTargetEyeMask = _camera.stereoTargetEye;
        }

        protected void OnEnable()
        {
            if (_fpfcManager == null)
            {
                return;
            }

            _fpfcManager.Add(this);
            _fpfcManager.PropertyChanged += OnFpfcSettingsPropertyChanged;

            UpdateSettings();
            UpdateState();
        }

        protected void Start() => OnEnable();

        protected void OnDisable()
        {
            if (_fpfcManager == null)
            {
                return;
            }

            _fpfcManager.PropertyChanged -= OnFpfcSettingsPropertyChanged;
            _fpfcManager.Remove(this);

            Deactivate();
        }

        private void OnBeforeUpdate()
        {
            if (InputState.currentUpdateType != InputUpdateType.Dynamic)
            {
                return;
            }

            Vector2 mouseMovement = 0.05f * mouseSensitivity * GetInputLookRotation();

            if (invertY)
            {
                mouseMovement.y = -mouseMovement.y;
            }

            targetCameraState.Yaw += mouseMovement.x;
            targetCameraState.Pitch += mouseMovement.y;

            Vector3 translation = Time.deltaTime * moveSensitivity * GetInputTranslationDirection(targetCameraState.Rotation);
            targetCameraState.Position += translation;

            ApplyCameraState();
        }

        private void ApplyCameraState()
        {
            targetCameraState.UpdateTransform(transform);

            if (_menuControllerAccessor != null)
            {
                transform.GetPositionAndRotation(out Vector3 position, out Quaternion rotation);
                _menuControllerAccessor.LeftController.transform.SetPositionAndRotation(position, rotation);
                _menuControllerAccessor.RightController.transform.SetPositionAndRotation(position, rotation);
            }
        }

        private Vector3 GetInputTranslationDirection(Quaternion rotation)
        {
            Vector3 direction = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                direction += rotation * Vector3.forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                direction += rotation * Vector3.back;
            }
            if (Input.GetKey(KeyCode.A))
            {
                direction += rotation * Vector3.left;
            }
            if (Input.GetKey(KeyCode.D))
            {
                direction += rotation * Vector3.right;
            }
            if (Input.GetKey(KeyCode.E))
            {
                direction += Vector3.up;
            }
            if (Input.GetKey(KeyCode.Q))
            {
                direction += Vector3.down;
            }
            return direction.normalized;
        }

        private Vector2 GetInputLookRotation()
        {
            return new Vector2(Input.GetAxis("MouseX"), Input.GetAxisRaw("MouseY"));
        }

        private void SetControllerEnabled(VRController controller, bool enabled)
        {
            controller.enabled = enabled;
            controller.mouseMode = !enabled;
        }

        private void OnFpfcSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateSettings();

            if (e.PropertyName == nameof(IFPFCSettings.Enabled))
            {
                UpdateState();
            }
        }

        private void UpdateSettings()
        {
            moveSensitivity = _fpfcManager!.MoveSensitivity;
            mouseSensitivity = _fpfcManager.MouseSensitivity * 0.75f;
            invertY = _fpfcManager.InvertY;
        }

        private void UpdateState()
        {
            if (_fpfcManager!.Enabled)
            {
                Activate();
            }
            else
            {
                Deactivate();
            }
        }

        private void Activate()
        {
            InputSystem.onBeforeUpdate -= OnBeforeUpdate;
            InputSystem.onBeforeUpdate += OnBeforeUpdate;

            ApplyCameraState();

            if (_camera != null)
            {
                _camera.stereoTargetEye = StereoTargetEyeMask.None;
                _camera.fieldOfView = _fpfcManager!.FOV;
                _camera.ResetAspect();
            }

            if (_trackedPoseDriver)
            {
                _trackedPoseDriver.enabled = false;
            }

            if (_pauseController != null)
            {
                _pauseController.ignoreHMDUUnmountEvets = true;
            }

            if (_menuControllerAccessor != null)
            {
                SetControllerEnabled(_menuControllerAccessor.LeftController, false);
                SetControllerEnabled(_menuControllerAccessor.RightController, false);
            }

            foreach (IFPFCListener listener in _fpfcListeners)
            {
                listener.Enabled();
            }
        }

        private void Deactivate()
        {
            InputSystem.onBeforeUpdate -= OnBeforeUpdate;

            if (_menuControllerAccessor != null)
            {
                SetControllerEnabled(_menuControllerAccessor.LeftController, true);
                SetControllerEnabled(_menuControllerAccessor.RightController, true);
            }

            if (_pauseController != null)
            {
                _pauseController.ignoreHMDUUnmountEvets = false;
            }

            if (!_fpfcManager!.LockViewOnDisable)
            {
                if (_camera != null)
                {
                    _camera.stereoTargetEye = _initialStereoTargetEyeMask;
                }

                if (_trackedPoseDriver)
                {
                    _trackedPoseDriver.enabled = true;
                }
            }

            foreach (IFPFCListener listener in _fpfcListeners)
            {
                listener.Disabled();
            }
        }

        private class CameraState
        {
            public CameraState(Vector3 position, Quaternion rotation)
            {
                Position = position;
                Rotation = rotation;
            }

            public Vector3 Position { get; set; }

            public float Yaw { get; set; }

            public float Pitch
            {
                get;
                set => field = Mathf.Clamp(value, -90f, 90f);
            }

            public float Roll { get; set; }

            public Quaternion Rotation
            {
                get => Quaternion.Euler(Pitch, Yaw, Roll);
                set
                {
                    Vector3 eulerAngles = value.eulerAngles;
                    Pitch = eulerAngles.x;
                    Yaw = eulerAngles.y;
                    Roll = eulerAngles.z;
                }
            }

            public void UpdateTransform(Transform transform)
            {
                transform.localEulerAngles = new Vector3(Pitch, Yaw, Roll);
                transform.localPosition = Position;
            }
        }
    }
}