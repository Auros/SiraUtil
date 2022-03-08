using SiraUtil.Affinity;
using SiraUtil.Logging;
using System;

namespace SiraUtil.Suite.Tests.Affinity
{
    [AffinityPatch(typeof(PlatformLeaderboardViewController), nameof(PlatformLeaderboardViewController.SetData))]
    internal class MainMenuViewControllerPatchTest : IAffinity
    {
        private readonly SiraLog _siraLog;

        public MainMenuViewControllerPatchTest(SiraLog siraLog)
        {
            _siraLog = siraLog;
        }

        [AffinityPrefix, AffinityPatch]
        protected void MenuDown()
        {
            _siraLog.Info($"Something happened!");
        }
    }
}