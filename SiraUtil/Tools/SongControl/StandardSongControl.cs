using IPA.Utilities;

namespace SiraUtil.Tools.SongControl
{
    internal class StandardSongControl : ISongControl
    {
        private readonly PauseController _pauseController;

        public StandardSongControl(PauseController pauseController)
        {
            _pauseController = pauseController;
        }

        public bool IsPaused => _pauseController.GetField<bool, PauseController>("_paused");

        public void Continue()
        {
            _pauseController.HandlePauseMenuManagerDidPressContinueButton();
        }

        public void Pause()
        {
            _pauseController.Pause();
        }

        public void Quit()
        {
            _pauseController.HandlePauseMenuManagerDidPressMenuButton();
        }

        public void Restart()
        {
            _pauseController.HandlePauseMenuManagerDidPressRestartButton();
        }
    }
}