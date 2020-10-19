using Zenject;
using UnityEngine;

namespace SiraUtil.Tools
{
	public class MultiSongControl : ITickable
	{
		private readonly KeyCode _exitCode;
		private readonly MultiplayerLocalActivePlayerInGameMenuViewController _multiplayerLocalActivePlayerInGameMenuViewController;

		public MultiSongControl([Inject(Id = "ExitCode")] KeyCode exitCode, MultiplayerLocalActivePlayerInGameMenuViewController multiplayerLocalActivePlayerInGameMenuViewController)
		{
			_exitCode = exitCode;
			_multiplayerLocalActivePlayerInGameMenuViewController = multiplayerLocalActivePlayerInGameMenuViewController;
		}

		public void Tick()
		{
			if (Input.GetKeyDown(_exitCode))
			{
				_multiplayerLocalActivePlayerInGameMenuViewController.GiveUpButtonPressed();
				return;
			}
		}
	}
}
