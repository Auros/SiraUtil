using Zenject;
using UnityEngine;
using SiraUtil.Tools;

namespace SiraUtil.Zenject
{
    internal class SiraInstallerInit : ISiraInstaller
    {
        private readonly Config _config;

        public SiraInstallerInit(Config config) => _config = config;

        public void Install(DiContainer container, GameObject source)
        {
            SiraInstaller.Install(container, _config);
        }
    }

    internal class SiraInstaller : Installer<Config, SiraInstaller>
    {
        private readonly Config _config;
        
        public SiraInstaller(Config config) => _config = config;

        public override void InstallBindings()
        {

            Container.BindInstance(_config).AsSingle().NonLazy();
        }
    }

    [RequiresInstaller(typeof(SiraInstallerInit))]
    internal class SiraGameInstaller : global::Zenject.Installer
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
        }
    }
}
