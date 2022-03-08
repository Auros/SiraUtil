using SiraUtil.Affinity;
using SiraUtil.Logging;
using System;

namespace SiraUtil.Suite.Tests.Affinity
{
    internal class MainMenuViewControllerPatchTest : IAffinity
    {
        private readonly SiraLog _siraLog;

        public MainMenuViewControllerPatchTest(SiraLog siraLog)
        {
            _siraLog = siraLog;
        }

        [AffinityPatch(typeof(PlatformLeaderboardViewController), nameof(PlatformLeaderboardViewController.SetData))]
        [AffinityPatch(typeof(MainMenuViewController), nameof(MainMenuViewController.HandleMenuButton))]
        protected void MenuDown()
        {
            _siraLog.Info($"Something happened@");
        }
    }
}