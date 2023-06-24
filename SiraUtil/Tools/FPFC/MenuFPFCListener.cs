using SiraUtil.Services;
using UnityEngine;

namespace SiraUtil.Tools.FPFC
{
    internal class MenuFPFCListener : IFPFCListener
    {
        private readonly IMenuControllerAccessor _menuControllerAccessor;

        private const string HandleName = "MenuHandle";
        //private bool _rightDefaultState;
        //private bool _leftDefaultState;

        private Pose _leftDefaultPose;
        private Pose _rightDefaultPose;

        public MenuFPFCListener(IMenuControllerAccessor menuControllerAccessor)
        {
            _menuControllerAccessor = menuControllerAccessor;
        }

        public void Enabled()
        {
            Transform leftHandle = _menuControllerAccessor.LeftController.transform.Find(HandleName);
            Transform rightHandle = _menuControllerAccessor.RightController.transform.Find(HandleName);

            //_leftDefaultState = leftHandle.gameObject.activeSelf;
            //_rightDefaultState = rightHandle.gameObject.activeSelf;

            _leftDefaultPose = new Pose(leftHandle.localPosition, leftHandle.localRotation);
            _rightDefaultPose = new Pose(rightHandle.localPosition, rightHandle.localRotation);

            leftHandle.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            rightHandle.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            //leftHandle.gameObject.SetActive(false);
            //rightHandle.gameObject.SetActive(false);
        }

        public void Disabled()
        {
            Transform leftHandle = _menuControllerAccessor.LeftController.transform.Find(HandleName);
            Transform rightHandle = _menuControllerAccessor.RightController.transform.Find(HandleName);

            leftHandle.SetLocalPositionAndRotation(_leftDefaultPose.position, _leftDefaultPose.rotation);
            rightHandle.SetLocalPositionAndRotation(_rightDefaultPose.position, _rightDefaultPose.rotation);

            //leftHandle.gameObject.SetActive(_leftDefaultState);
            //rightHandle.gameObject.SetActive(_rightDefaultState);
        }
    }
}