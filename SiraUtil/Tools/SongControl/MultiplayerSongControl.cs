namespace SiraUtil.Tools.SongControl
{
    internal class MultiplayerSongControl : ISongControl
    {
        //private readonly MultiplayerLocalActivePlayerInGameMenuViewController _multiplayerLocalActivePlayerInGameMenuViewController;
        //
        //internal MultiplayerSongControl(MultiplayerLocalActivePlayerInGameMenuViewController multiplayerLocalActivePlayerInGameMenuViewController)
        //{
        //    _multiplayerLocalActivePlayerInGameMenuViewController = multiplayerLocalActivePlayerInGameMenuViewController;
        //}

        public bool IsPaused => false;// _multiplayerLocalActivePlayerInGameMenuViewController.isActiveAndEnabled;

        public void Continue()
        {
            //_multiplayerLocalActivePlayerInGameMenuViewController.ResumeButtonPressed();
        }

        public void Pause()
        {
           // _multiplayerLocalActivePlayerInGameMenuViewController.ShowMenu();
        }

        public void Quit()
        {
            //_multiplayerLocalActivePlayerInGameMenuViewController.GiveUpButtonPressed();
        }

        public void Restart()
        {
            //_multiplayerLocalActivePlayerInGameMenuViewController.GiveUpButtonPressed();
        }
    }
}