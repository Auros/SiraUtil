    using UnityEngine;

namespace SiraUtil.Tools.FPFC
{
    // This is a modified version of Unity's SimpleCameraController which is included in the Universal Render Pipeline template.
    internal class SimpleCameraController : MonoBehaviour
    {
        private readonly CameraState _targetCameraState = new();
        private readonly CameraState _interpolatingCameraState = new();
        
        private float _boost = 0.1f;
        private readonly bool _invertY = true;
        private readonly float _positionLerpTime = 0.0f;
        private readonly float _rotationLerpTime = 0.0f;
        private readonly AnimationCurve _mouseSensitivityCurve = new(new Keyframe(0.75f, 0.75f, 0f, 0f), new Keyframe(0.75f, 0.75f, 0f, 0f));

        public float MouseSensitivity { get; set; } = 10f;
        public bool AllowInput { get; set; } = false;

        protected void Awake()
        {
            _targetCameraState.SetFromTransform(transform);
            _interpolatingCameraState.SetFromTransform(transform);
        }

        private Vector3 GetInputTranslationDirection()
        {
            Vector3 direction = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                direction += Vector3.forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                direction += Vector3.back;
            }
            if (Input.GetKey(KeyCode.A))
            {
                direction += Vector3.left;
            }
            if (Input.GetKey(KeyCode.D))
            {
                direction += Vector3.right;
            }
            return direction;
        }

        private Vector2 GetInputLookRotation()
        {
            return new Vector2(Input.GetAxis("MouseX"), Input.GetAxisRaw("MouseY")) * MouseSensitivity;
        }

        private float BoostFactor()
        {
            return Input.mouseScrollDelta.y * 0.2f;
        }

        protected void Update()
        {
            if (!AllowInput)
                return;

            CameraState holdingCameraState = new();
            holdingCameraState.Read(_targetCameraState);

            Vector2 mouseMovement = GetInputLookRotation() * 0.05f;
            print(mouseMovement);
            if (_invertY)
                mouseMovement.y = -mouseMovement.y;

            float mouseSensitivityFactor = _mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);
            holdingCameraState.yaw += mouseMovement.x * mouseSensitivityFactor;
            holdingCameraState.pitch += mouseMovement.y * mouseSensitivityFactor;
            Vector3 translation = GetInputTranslationDirection() * Time.deltaTime * 2f;

            _boost += BoostFactor();
            translation *= Mathf.Pow(2.0f, _boost);
            holdingCameraState.Translate(translation);

            float positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / _positionLerpTime) * Time.deltaTime);
            float rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / _rotationLerpTime) * Time.deltaTime);

            if (_interpolatingCameraState.AboveBounds(holdingCameraState, rotationLerpPct))
            {
                return;
            }

            _targetCameraState.Read(holdingCameraState);
            _interpolatingCameraState.LerpTowards(_targetCameraState, positionLerpPct, rotationLerpPct);
            _interpolatingCameraState.UpdateTransform(transform);
        }

        private class CameraState
        {
            public float yaw;
            public float pitch;
            public float roll;
            public float x;
            public float y;
            public float z;

            public void SetFromTransform(Transform t)
            {
                pitch = t.eulerAngles.x;
                yaw = t.eulerAngles.y;
                roll = t.eulerAngles.z;
                x = t.position.x;
                y = t.position.y;
                z = t.position.z;
            }

            public void Translate(Vector3 translation)
            {
                Vector3 rotatedTranslation = Quaternion.Euler(pitch, yaw, roll) * translation;

                x += rotatedTranslation.x;
                y += rotatedTranslation.y;
                z += rotatedTranslation.z;
            }

            public void LerpTowards(CameraState target, float positionLerpPct, float rotationLerpPct)
            {
                yaw = Mathf.Lerp(yaw, target.yaw, rotationLerpPct);
                pitch = Mathf.Lerp(pitch, target.pitch, rotationLerpPct);
                roll = Mathf.Lerp(roll, target.roll, rotationLerpPct);

                x = Mathf.Lerp(x, target.x, positionLerpPct);
                y = Mathf.Lerp(y, target.y, positionLerpPct);
                z = Mathf.Lerp(z, target.z, positionLerpPct);
            }

            public bool AboveBounds(CameraState target, float rotationLerpPct)
            {
                pitch = Mathf.Lerp(pitch, target.pitch, rotationLerpPct);
                return Mathf.Abs(pitch) > 90f;
            }

            public void UpdateTransform(Transform t)
            {
                t.eulerAngles = new Vector3(pitch, yaw, roll);
                t.position = new Vector3(x, y, z);
            }

            public void Read(CameraState newState)
            {
                x = newState.x;
                y = newState.y;
                z = newState.z;
                yaw = newState.yaw;
                roll = newState.roll;
                pitch = newState.pitch;
            }
        }
    }
}