using UnityEngine;
using Zenject;

namespace SiraUtil.Services.Controllers
{
    internal class GameMenuControllerAccessor : IMenuControllerAccessor
    {
        public VRController LeftController { get; }
        public VRController RightController { get; }
        public Transform Parent { get; }

        public GameMenuControllerAccessor([InjectOptional] PauseMenuManager pauseMenuManager, [InjectOptional] MultiplayerLocalActivePlayerInGameMenuViewController multiplayerLocalActivePlayerInGameMenuViewController)
        {
            if (pauseMenuManager is null && multiplayerLocalActivePlayerInGameMenuViewController is null)
            {
                throw new System.Exception("Cannot find menu controllers!");
            }
            Transform controllerWrapper = pauseMenuManager is not null ? pauseMenuManager!.transform.Find("MenuControllers") : multiplayerLocalActivePlayerInGameMenuViewController.transform.Find("MenuControllers");
            LeftController = controllerWrapper.Find("ControllerLeft").GetComponent<VRController>();
            RightController = controllerWrapper.Find("ControllerRight").GetComponent<VRController>();
            Parent = controllerWrapper.transform;
        }
    }
}