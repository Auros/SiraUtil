using Zenject;
using UnityEngine;

namespace SiraUtil.Tools
{
    public class MultiSongControl : ITickable
    {
        private readonly KeyCode _exitCode;
        private readonly KeyCode _pauseKeyCode;
        private readonly MultiplayerLocalActivePlayerInGameMenuViewController _multiplayerLocalActivePlayerInGameMenuViewController;

        public MultiSongControl(Config.SongControlOptions songControlOptions, MultiplayerLocalActivePlayerInGameMenuViewController multiplayerLocalActivePlayerInGameMenuViewController)
        {
            _exitCode = songControlOptions.ExitKeyCode;
            _pauseKeyCode = songControlOptions.PauseToggleKeyCode;
            _multiplayerLocalActivePlayerInGameMenuViewController = multiplayerLocalActivePlayerInGameMenuViewController;
        }

        public void Tick()
        {
            if (Input.GetKeyDown(_exitCode))
            {
                _multiplayerLocalActivePlayerInGameMenuViewController.GiveUpButtonPressed();
                return;
            }
            if (Input.GetKeyDown(_pauseKeyCode))
            {
                _multiplayerLocalActivePlayerInGameMenuViewController.ShowMenu();
                return;
            }
        }
    }
}