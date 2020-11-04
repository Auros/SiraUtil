using Zenject;
using SiraUtil.Tools;

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
                Container.BindInstance(_config.SongControl.ExitKeyCode).WithId("ExitCode");
                Container.BindInstance(_config.SongControl.PauseToggleKeyCode).WithId("PauseToggleCode");
                Container.BindInterfacesTo<MultiSongControl>().AsSingle().NonLazy();
            }
        }
    }
}