using UnityEngine;

namespace SiraUtil.Services.Controllers
{
    internal class MenuMenuControllerAccessor : IMenuControllerAccessor
    {
        public VRController LeftController { get; }

        public VRController RightController { get; }

        public Transform Parent => _menuPlayerController.transform;

        private readonly MenuPlayerController _menuPlayerController;

        public MenuMenuControllerAccessor(MenuPlayerController menuPlayerController)
        {
            _menuPlayerController = menuPlayerController;
            LeftController = menuPlayerController.leftController;
            RightController = menuPlayerController.rightController;
        }
    }
}