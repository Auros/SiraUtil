using UnityEngine;
using Zenject;

namespace SiraUtil.Tools.SongControl
{
    internal class SongControlManager : ITickable
    {
        private readonly ISongControl _songControl;
        private readonly SongControlOptions _songControlOptions;

        public SongControlManager(ISongControl songControl, SongControlOptions songControlOptions)
        {
            _songControl = songControl;
            _songControlOptions = songControlOptions;
        }

        public void Tick()
        {
            if (Input.GetKeyDown(_songControlOptions.ExitKeyCode))
            {
                _songControl.Quit();
            }
            else if (Input.GetKeyDown(_songControlOptions.RestartKeyCode))
            {
                _songControl.Restart();
            }
            else if (Input.GetKeyDown(_songControlOptions.PauseToggleKeyCode))
            {
                if (_songControl.IsPaused)
                {
                    _songControl.Continue();
                }
                else
                {
                    _songControl.Pause();
                }
            }
        }
    }
}