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
            Assert.IsNotNull(_gameplayCoreSceneSetupData.beatmapKey);
            Plugin.Log.Info(_gameplayCoreSceneSetupData.beatmapKey.SerializedName());
        }

        public override void InstallBindings()
        {

        }
    }
}