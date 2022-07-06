namespace SiraUtil.Submissions
{
    internal sealed class MultiplayerDisplayer : SubmissionDisplayer
    {
        public MultiplayerDisplayer(GameServerLobbyFlowCoordinator gameServerLobbyFlowCoordinator, MultiplayerResultsViewController multiplayerResultsViewController) : base(gameServerLobbyFlowCoordinator, multiplayerResultsViewController)
        {

        }
    }
}