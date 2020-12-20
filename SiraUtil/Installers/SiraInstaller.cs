using System;
using Zenject;
using SiraUtil.Tools;
using SiraUtil.Services;
using SiraUtil.Interfaces;

namespace SiraUtil.Installers
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
            Container.BindInterfacesAndSelfTo<CanvasContainer>().AsSingle();
            if (!Container.HasBinding<SiraLogManager>())
            {
                Container.Bind<SiraLogManager>().AsSingle().IfNotBound();
            }
            Container.Bind<SiraLog>().AsTransient().OnInstantiated<SiraLog>((ctx, siraLogger) =>
            {
                var logManager = ctx.Container.Resolve<SiraLogManager>();
                var logger = logManager.LoggerFromAssembly(ctx.ObjectType.Assembly);
                siraLogger.DebugMode = logger.debugMode;
                siraLogger.Setup(logger.logger, ctx.ObjectType.Name);
            });
            Container.Bind<SiraClient>().AsTransient().OnInstantiated<SiraClient>((ctx, siraClient) =>
            {
                var webClient = ctx.Container.Resolve<WebClient>();
                var assembly = ctx.ObjectType.Assembly;
                siraClient.RootAssembly = assembly;
                siraClient.Client = webClient;
            });

            Container.BindInstance(_config).AsSingle();
            Container.BindInstance(_config.FPFCToggle).AsSingle();
            Container.BindInstance(_config.SongControl).AsSingle();

            if (_config.FPFCToggle.Enabled)
            {
                Container.Bind<FPFCToggle>().FromNewComponentOnNewGameObject(nameof(FPFCToggle)).AsSingle().NonLazy();
            }
            Container.Bind(typeof(IInitializable), typeof(IDisposable), typeof(WebClient)).To<WebClient>().AsSingle();
            Container.BindInterfacesAndSelfTo<Localizer>().AsSingle();
            Container.Bind<Submission.Data>().AsSingle();

            // Make Zenject know this is a list
            Container.Bind<IModelProvider>().To<DummyProviderA>().AsSingle();
            Container.Bind<IModelProvider>().To<DummyProviderB>().AsSingle();
        }

        private class DummyProviderA : IModelProvider { public int Priority => -9999; public Type Type => typeof(DummyProviderB); }
        private class DummyProviderB : IModelProvider { public int Priority => -9999; public Type Type => typeof(DummyProviderA); }
    }
}