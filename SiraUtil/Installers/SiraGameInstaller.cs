using SiraUtil.Services;
using SiraUtil.Tools;
using Zenject;

namespace SiraUtil.Zenject
{
    internal class SiraGameInstaller : Installer
    {
        private readonly Config _config;

        public SiraGameInstaller(Config config)
        {
            _config = config;
        }

        public override void InstallBindings()
        {
            if (_config.SongControl.Enabled)
            {
                Container.BindInterfacesTo<SongControl>().AsSingle();
            }
            Container.BindInterfacesAndSelfTo<SiraSaberEffectManager>().AsSingle();
        }
    }
}