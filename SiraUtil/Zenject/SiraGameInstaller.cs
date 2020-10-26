using Zenject;
using SiraUtil.Tools;
using SiraUtil.Services;

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
                Container.BindInstance(_config.SongControl.ExitKeyCode).WithId("ExitCode");
                Container.BindInstance(_config.SongControl.RestartKeyCode).WithId("RestartCode");
                Container.BindInstance(_config.SongControl.PauseToggleKeyCode).WithId("PauseToggleCode");
                Container.BindInterfacesTo<SongControl>().AsSingle().NonLazy();
            }
            Container.BindInterfacesAndSelfTo<Submission>().AsSingle();
        }
    }
}