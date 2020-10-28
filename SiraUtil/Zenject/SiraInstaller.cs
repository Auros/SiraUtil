using System;
using Zenject;
using SiraUtil.Tools;
using SiraUtil.Services;
using SiraUtil.Interfaces;

namespace SiraUtil.Zenject
{
    internal class SiraInstaller : Installer<Config, SiraInstaller>
    {
        private readonly Config _config;

        public SiraInstaller(Config config)
        {
            _config = config;
        }

        public override void InstallBindings()
        {
            Container.BindInstance(_config).AsSingle().NonLazy();
            Container.BindInstance(_config.FPFCToggle).AsSingle();

            if (_config.FPFCToggle.Enabled)
            {
                Container.Bind<FPFCToggle>().FromNewComponentOnNewGameObject(nameof(FPFCToggle)).AsSingle().NonLazy();
            }
            Container.Bind(typeof(IInitializable), typeof(IDisposable), typeof(WebClient)).To<WebClient>().AsSingle();
            Container.BindInterfacesAndSelfTo<Localizer>().AsSingle().NonLazy();
            Container.Bind<Submission.Data>().AsSingle();

            // Make Zenject know this is a list
            Container.Bind<IModelProvider>().To<DummyProviderA>().AsSingle();
            Container.Bind<IModelProvider>().To<DummyProviderB>().AsSingle();
        }

        private class DummyProviderA : IModelProvider { public int Priority => -9999; public Type Type => typeof(DummyProviderB); }
        private class DummyProviderB : IModelProvider { public int Priority => -9999; public Type Type => typeof(DummyProviderA); }
    }
}