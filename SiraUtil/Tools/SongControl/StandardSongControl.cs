namespace SiraUtil.Tools.SongControl
{
    internal class StandardSongControl : ISongControl
    {
        private readonly PauseController _pauseController;
        private readonly PauseMenuManager _pauseMenuManager;

        public StandardSongControl(PauseController pauseController, PauseMenuManager pauseMenuManager)
        {
            _pauseController = pauseController;
            _pauseMenuManager = pauseMenuManager;
        }

        public bool IsPaused => _pauseController._paused == PauseController.PauseState.Paused;

        public void Continue()
        {
            _pauseMenuManager.ContinueButtonPressed();
        }

        public void Pause()
        {
            _pauseController.Pause();
        }

        public void Quit()
        {
            _pauseMenuManager.MenuButtonPressed();
        }

        public void Restart()
        {
            _pauseMenuManager.RestartButtonPressed();
        }
    }
}