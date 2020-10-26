using System;
using Zenject;
using UnityEngine;

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

        public void Exit()
        {
            
        }

        public void Play()
        {
            
        }

        public void Pause()
        {
            
        }

        public void Restart()
        {
            
        }
    }
}