using UnityEngine;

namespace SiraUtil.Services.Controllers
{
    internal class GameMenuControllerAccessor : IMenuControllerAccessor
    {
        public VRController LeftController { get; }

        public VRController RightController { get; }

        public GameMenuControllerAccessor(PauseMenuManager pauseMenuManager)
        {
            Transform controllerWrapper = pauseMenuManager.transform.Find("MenuControllers");
            LeftController = controllerWrapper.Find("ControllerLeft").GetComponent<VRController>();
            RightController = controllerWrapper.Find("ControllerRight").GetComponent<VRController>();
        }
    }
}