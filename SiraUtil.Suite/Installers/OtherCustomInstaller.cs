using ModestTree;
using Zenject;

namespace SiraUtil.Suite.Installers
{
    internal class OtherCustomInstaller : Installer
    {
        private readonly GameplayCoreSceneSetupData _gameplayCoreSceneSetupData;

        public OtherCustomInstaller(GameplayCoreSceneSetupData gameplayCoreSceneSetupData)
        {
            _gameplayCoreSceneSetupData = gameplayCoreSceneSetupData;
            Assert.IsNotNull(_gameplayCoreSceneSetupData);
            Assert.IsNotNull(_gameplayCoreSceneSetupData.difficultyBeatmap);
            Plugin.Log.Info(_gameplayCoreSceneSetupData.difficultyBeatmap.SerializedName());
        }

        public override void InstallBindings()
        {
            Plugin.Log.Notice($"Installing from {nameof(OtherCustomInstaller)}");
        }
    }
}