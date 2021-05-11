namespace SiraUtil.Services.Controllers
{
    internal class MenuMenuControllerAccessor : IMenuControllerAccessor
    {
        public VRController LeftController { get; }

        public VRController RightController { get; }

        public MenuMenuControllerAccessor(MenuPlayerController menuPlayerController)
        {
            LeftController = menuPlayerController.leftController;
            RightController = menuPlayerController.rightController;
        }
    }
}