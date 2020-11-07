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

        public SongControl(Config.SongControlOptions songControlOptions, PauseController pauseController)
        {
            _isPaused = false;
            _pauseController = pauseController;

            _exitCode = songControlOptions.ExitKeyCode;
            _restartCode = songControlOptions.RestartKeyCode;
            _pauseToggleCode = songControlOptions.PauseToggleKeyCode;
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
    }
}