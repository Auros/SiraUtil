using SiraUtil.Affinity;
using SiraUtil.Logging;

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

        [AffinityPatch(typeof(MirroredNoteController<INoteMirrorable>), "Mirror")]
        private void BlahBlahBlah(MirroredNoteController<INoteMirrorable> __instance)
        {

        }
    }
}