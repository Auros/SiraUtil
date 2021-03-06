using Zenject;
using SiraUtil.Tools;
using SiraUtil.Services;

namespace SiraUtil.Installers
{
    internal class SiraMultiGameInstaller : Installer
    {
        private readonly Config _config;

        public SiraMultiGameInstaller(Config config)
        {
            _config = config;
        }

        public override void InstallBindings()
        {
            if (_config.SongControl.Enabled)
            {
                Container.BindInterfacesTo<MultiSongControl>().AsSingle();
            }
            Container.BindInterfacesAndSelfTo<SiraSaberEffectManager>().AsSingle();
        }
    }
}