using SiraUtil.Services;
using UnityEngine;

namespace SiraUtil.Tools.FPFC
{
    internal class MenuFPFCListener : IFPFCListener
    {
        private readonly IMenuControllerAccessor _menuControllerAccessor;

        private const string HandleName = "MenuHandle";
        private bool _rightDefaultState;
        private bool _leftDefaultState;

        public MenuFPFCListener(IMenuControllerAccessor menuControllerAccessor)
        {
            _menuControllerAccessor = menuControllerAccessor;
        }

        public void Enabled()
        {
            Transform leftHandle = _menuControllerAccessor.LeftController.transform.Find(HandleName);
            Transform rightHandle = _menuControllerAccessor.RightController.transform.Find(HandleName);

            _leftDefaultState = leftHandle.gameObject.activeSelf;
            _rightDefaultState = rightHandle.gameObject.activeSelf;

            leftHandle.gameObject.SetActive(false);
            rightHandle.gameObject.SetActive(false);
        }

        public void Disabled()
        {
            Transform leftHandle = _menuControllerAccessor.LeftController.transform.Find(HandleName);
            Transform rightHandle = _menuControllerAccessor.RightController.transform.Find(HandleName);

            leftHandle.gameObject.SetActive(_leftDefaultState);
            rightHandle.gameObject.SetActive(_rightDefaultState);
        }
    }
}