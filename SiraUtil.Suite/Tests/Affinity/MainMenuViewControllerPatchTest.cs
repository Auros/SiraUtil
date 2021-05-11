using SiraUtil.Affinity;

namespace SiraUtil.Suite.Tests.Affinity
{
    internal class MainMenuViewControllerPatchTest : IAffinity
    {
        [AffinityPatch(typeof(MainMenuViewController), nameof(MainMenuViewController.HandleMenuButton))]
        protected void MenuDown()
        {
            Plugin.Log.Info("Menu button was pressed.");
        }
    }
}