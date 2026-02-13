using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace SiraUtil.Tools.FPFC
{
    // This is a modified version of Unity's SimpleCameraController which is included in the Universal Render Pipeline template.
    internal class SimpleCameraController : MonoBehaviour
    {
        private readonly CameraState targetCameraState = new(new Vector3(0, 1.7f, 0), Quaternion.identity);

        private readonly bool invertY = true;
        private readonly AnimationCurve mouseSensitivityCurve = new(new Keyframe(0.75f, 0.75f, 0f, 0f), new Keyframe(0.75f, 0.75f, 0f, 0f));

        internal event Action<Vector3, Quaternion>? StateChanged;

        public float MouseSensitivity { get; set; } = 5f;

        public float MoveSensitivity { get; set; } = 3f;

        protected void OnEnable()
        {
            InputSystem.onBeforeUpdate += OnBeforeUpdate;
        }

        protected void OnDisable()
        {
            InputSystem.onBeforeUpdate -= OnBeforeUpdate;
        }

        private void OnBeforeUpdate()
        {
            if (InputState.currentUpdateType != InputUpdateType.Dynamic)
            {
                return;
            }

            Vector2 mouseMovement = GetInputLookRotation() * 0.05f * MouseSensitivity;

            if (invertY)
            {
                mouseMovement.y = -mouseMovement.y;
            }

            float mouseSensitivityFactor = mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);
            targetCameraState.Yaw += mouseMovement.x * mouseSensitivityFactor;
            targetCameraState.Pitch += mouseMovement.y * mouseSensitivityFactor;

            Vector3 translation = GetInputTranslationDirection(targetCameraState.Rotation) * Time.deltaTime * MoveSensitivity;
            targetCameraState.Position += translation;

            targetCameraState.UpdateTransform(transform);

            transform.GetPositionAndRotation(out Vector3 position, out Quaternion rotation);
            StateChanged?.Invoke(position, rotation);
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
            return direction;
        }

        private Vector2 GetInputLookRotation()
        {
            return new Vector2(Input.GetAxis("MouseX"), Input.GetAxisRaw("MouseY"));
        }

        private class CameraState
        {
            private float pitch;

            public CameraState(Vector3 position, Quaternion rotation)
            {
                Position = position;
                Rotation = rotation;
            }

            public Vector3 Position { get; set; }

            public float Yaw { get; set; }

            public float Pitch
            {
                get => pitch;
                set => pitch = Mathf.Clamp(value, -90f, 90f);
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