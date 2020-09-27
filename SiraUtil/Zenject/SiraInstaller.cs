using System;
using Zenject;
using UnityEngine;
using System.Linq;
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

    [RequiresInstaller(typeof(SiraInstallerInit))]
    internal class SiraInstaller : Installer<Config, SiraInstaller>
    {
        private readonly Config _config;
        
        public SiraInstaller(Config config) => _config = config;

        public override void InstallBindings()
        {
            Container.BindInstance(_config).AsSingle().NonLazy();
            if (_config.FPFCToggle.Enabled)
            {
                Container.BindInstance(_config.FPFCToggle.Enabled).WithId("FPFCEnabled").WhenInjectedInto<FPFCToggle>();
                Container.BindInstance(_config.FPFCToggle.CameraFOV).WithId("CameraFOV").WhenInjectedInto<FPFCToggle>();
                Container.BindInstance(_config.FPFCToggle.ToggleKeyCode).WithId("ToggleCode").WhenInjectedInto<FPFCToggle>();
                Container.BindInstance(_config.FPFCToggle.MoveSensitivity).WithId("MoveSensitivity").WhenInjectedInto<FPFCToggle>();
                Container.Bind<FPFCToggle>().FromNewComponentOnRoot().AsSingle().NonLazy();
            }
            if (_config.Localization.Enabled)
            {
                Container.Bind(typeof(IInitializable), typeof(IDisposable), typeof(WebClient)).To<WebClient>().AsSingle();
                Container.BindInterfacesAndSelfTo<Localizer>().AsSingle().NonLazy();
            }
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

    [RequiresInstaller(typeof(SiraInstallerInit))]
    internal class SiraMenuTestInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Plugin.Log.Info("what? how?");
        }
    }

    [RequiresInstaller(typeof(SiraInstallerInit))]
    internal class SiraMenuTestInstaller2 : global::Zenject.Installer
    {
        public override void InstallBindings()
        {
            Plugin.Log.Info("what? how?");
        }
    }
}
