namespace SiraUtil.Tools.SongControl
{
    /// <summary>
    /// An interface to controlling the song state.
    /// </summary>
    public interface ISongControl
    {
        /// <summary>
        /// Whether or not the game is paused (paused for standard/campaigns/tutorial, menu open for multiplayer).
        /// </summary>
        bool IsPaused { get; }

        /// <summary>
        /// Quits the song. In multiplayer this will make you give up and put you in spectator mode.
        /// </summary>
        void Quit();

        /// <summary>
        /// Pauses the song. In multiplayer, this only brings up the menu.
        /// </summary>
        void Pause();

        /// <summary>
        /// Restarts the song. In multiplayer, this will make you give up and put you into spectator mode.
        /// </summary>
        void Restart();

        /// <summary>
        /// Continues the song if already paused.
        /// </summary>
        void Continue();
    }
}