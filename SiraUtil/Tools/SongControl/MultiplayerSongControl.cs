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

        public bool IsPaused => _multiplayerLocalActivePlayerInGameMenuViewController != null && _multiplayerLocalActivePlayerInGameMenuViewController.isActiveAndEnabled;

        public void Continue()
        {
            if (_multiplayerLocalActivePlayerInGameMenuViewController != null)
            {
                _multiplayerLocalActivePlayerInGameMenuViewController.ResumeButtonPressed();
            }
        }

        public void Pause()
        {
            if (_multiplayerLocalActivePlayerInGameMenuViewController != null)
            {
                _multiplayerLocalActivePlayerInGameMenuViewController.ShowMenu();
            }
        }

        public void Quit()
        {
            if (_multiplayerLocalActivePlayerInGameMenuViewController != null)
            {
                _multiplayerLocalActivePlayerInGameMenuViewController.GiveUpButtonPressed();
            }
        }

        public void Restart()
        {
            if (_multiplayerLocalActivePlayerInGameMenuViewController != null)
            {
                _multiplayerLocalActivePlayerInGameMenuViewController.GiveUpButtonPressed();
            }
        }
    }
}