using System;
using Zenject;
using UnityEngine;
using IPA.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace SiraUtil.Tools
{
    public class SongControl : IInitializable, IDisposable, ITickable
    {
        private bool _isPaused = false;
        private readonly PauseController _pauseController;

        private readonly KeyCode _exitCode;
        private readonly KeyCode _restartCode;
        private readonly KeyCode _pauseToggleCode;


        public SongControl([Inject(Id = "ExitCode")] KeyCode exitCode, [Inject(Id = "RestartCode")] KeyCode restartCode, [Inject(Id = "PauseToggleCode")] KeyCode pauseToggleCode, PauseController pauseController)
        {
            _isPaused = false;
            _pauseController = pauseController;

            _exitCode = exitCode;
            _restartCode = restartCode;
            _pauseToggleCode = pauseToggleCode;
        }

        public void Initialize()
        {
            _pauseController.didPauseEvent += OnPause;
            _pauseController.didResumeEvent += OnResume;
			/*var decorator = _context.GetField<List<SceneDecoratorContext>, SceneContext>("_decoratorContexts").FirstOrDefault(x => x.DecoratedContractName == "Environment");
			if (decorator != null)
			{
				var hud = decorator.GetField<List<MonoBehaviour>, SceneDecoratorContext>("_injectableMonoBehaviours").FirstOrDefault(x => x.GetType() == typeof(CoreGameHUDController));
				if (hud != null)
				{
					Plugin.Log.Info("It's gamer time.");
				}
			}*/
        }

        private void OnPause()
        {
            _isPaused = true;
        }

        private void OnResume()
        {
            _isPaused = false;
        }

        public void Tick()
        {
            if (Input.GetKeyDown(_exitCode))
            {
                _pauseController.HandlePauseMenuManagerDidPressMenuButton();
                return;
            }
            if (Input.GetKeyDown(_restartCode))
            {
                _pauseController.HandlePauseMenuManagerDidPressRestartButton();
                return;
            }
            if (Input.GetKeyDown(_pauseToggleCode))
            {
                if (_isPaused)
                {
                    _pauseController.HandlePauseMenuManagerDidPressContinueButton();
                }
                else
                {
					_pauseController.Pause();
                }
                return;
            }
        }

        public void Dispose()
        {
            _pauseController.didPauseEvent -= OnPause;
            _pauseController.didResumeEvent -= OnResume;
        }
    }
}