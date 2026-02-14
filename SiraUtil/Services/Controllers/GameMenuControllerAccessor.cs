using UnityEngine;
using Zenject;

namespace SiraUtil.Services.Controllers
{
    internal class GameMenuControllerAccessor : IMenuControllerAccessor
    {
        public VRController LeftController { get; }
        public VRController RightController { get; }
        public Transform Parent { get; }

        public GameMenuControllerAccessor([InjectOptional] PauseMenuManager pauseMenuManager, [InjectOptional] MultiplayerLocalActivePlayerInGameMenuViewController activeViewController, Context context)
        {
            MultiplayerLocalInactivePlayerInGameMenuViewController? inactive = null;
            if (pauseMenuManager == null && activeViewController == null && (inactive = context.GetComponentInChildren<MultiplayerLocalInactivePlayerInGameMenuViewController>()) == null)
            {
                throw new System.Exception("Cannot find menu controllers!");
            }

            Transform controllerWrapper = pauseMenuManager != null ? pauseMenuManager!.transform.Find("MenuControllers") : activeViewController != null ? activeViewController.transform.Find("MenuControllers") : inactive!.transform.Find("MenuControllers");
            LeftController = controllerWrapper.Find("ControllerLeft").GetComponent<VRController>();
            RightController = controllerWrapper.Find("ControllerRight").GetComponent<VRController>();
            Parent = controllerWrapper.transform;
        }
    }
}