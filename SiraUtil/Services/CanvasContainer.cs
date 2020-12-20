using System;
using Zenject;
using UnityEngine;

namespace SiraUtil.Services
{
    internal class CanvasContainer : IInitializable, IDisposable
    {
        public Canvas CurvedCanvasTemplate { get; internal set; }
        private readonly GameScenesManager _gameScenesManager;

        internal CanvasContainer(GameScenesManager gameScenesManager)
        {
            _gameScenesManager = gameScenesManager;
        }

        public void Initialize()
        {
            _gameScenesManager.transitionDidFinishEvent += SceneTransitioned;
        }

        private void SceneTransitioned(ScenesTransitionSetupDataSO transitionData, DiContainer container)
        {
            if (CurvedCanvasTemplate == null)
            {
                MainMenuViewController view = container.TryResolve<MainMenuViewController>();
                if (view != null)
                {
                    CurvedCanvasTemplate = view.GetComponent<Canvas>();
                }
            }
        }

        public void Dispose()
        {
            _gameScenesManager.transitionDidFinishEvent -= SceneTransitioned;
        }

        internal class DummyRaycaster : UnityEngine.EventSystems.BaseRaycaster
        {
            public override Camera eventCamera => Camera.main;

            public override void Raycast(UnityEngine.EventSystems.PointerEventData eventData, System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult> resultAppendList)
            {

            }
        }
    }
}
