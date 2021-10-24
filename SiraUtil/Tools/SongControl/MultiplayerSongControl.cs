using Zenject;

namespace SiraUtil.Tools.SongControl
{
    internal class MultiplayerSongControl : ISongControl
    {
        private readonly MultiplayerLocalActivePlayerInGameMenuViewController? _multiplayerLocalActivePlayerInGameMenuViewController;
        
        internal MultiplayerSongControl([InjectOptional] MultiplayerLocalActivePlayerInGameMenuViewController multiplayerLocalActivePlayerInGameMenuViewController)
        {
            _multiplayerLocalActivePlayerInGameMenuViewController = multiplayerLocalActivePlayerInGameMenuViewController;
        }

        public bool IsPaused => _multiplayerLocalActivePlayerInGameMenuViewController != null ? _multiplayerLocalActivePlayerInGameMenuViewController.isActiveAndEnabled : false;

        public void Continue()
        {
            _multiplayerLocalActivePlayerInGameMenuViewController?.ResumeButtonPressed();
        }

        public void Pause()
        {
            _multiplayerLocalActivePlayerInGameMenuViewController?.ShowMenu();
        }

        public void Quit()
        {
            _multiplayerLocalActivePlayerInGameMenuViewController?.GiveUpButtonPressed();
        }

        public void Restart()
        {
            _multiplayerLocalActivePlayerInGameMenuViewController?.GiveUpButtonPressed();
        }
    }
}