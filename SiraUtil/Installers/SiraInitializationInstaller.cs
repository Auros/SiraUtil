using IPA.Loader;
using SiraUtil.Logging;
using SiraUtil.Services.Events;
using SiraUtil.Web;
using SiraUtil.Web.Zenject;
using SiraUtil.Zenject;
using Zenject;

namespace SiraUtil.Installers
{
    internal class SiraInitializationInstaller : Installer
    {
        private readonly PluginMetadata _siraUtil;
        private readonly SiraLogManager _siraLogManager;
        private readonly ZenjectManager _zenjectManager;
        private readonly HttpServiceManager _httpServiceManager;

        public SiraInitializationInstaller(PluginMetadata siraUtil, ZenjectManager zenjectManager)
        {
            _siraUtil = siraUtil;
            _zenjectManager = zenjectManager;
            _siraLogManager = new(zenjectManager);
            _httpServiceManager = new(zenjectManager, _siraUtil);
        }

        public override void InstallBindings()
        {
            // Takes every active zenjector and adds them to the SiraLogger
            _siraLogManager.Clear();
            foreach (var zenjector in _zenjectManager.ActiveZenjectors)
                if (zenjector.Logger is not null)
                    _siraLogManager.AddLogger(zenjector.Metadata.Assembly, zenjector.Logger);
            Container.Bind<SiraLog>().AsTransient().OnInstantiated<SiraLog>(SiraLogCreated);

            // Install all UBinders
            foreach (var zenjector in _zenjectManager.ActiveZenjectors)
                if (zenjector.UBinderType is not null && zenjector.UBinderValue is not null)
                    Container.Bind(zenjector.UBinderType).FromInstance(zenjector.UBinderValue).AsSingle();

            // Takes every active zenjector and adds them to the http service.
            _httpServiceManager.Clear();
            foreach (var zenjector in _zenjectManager.ActiveZenjectors)
                if (zenjector.HttpServiceType is not null)
                    _httpServiceManager.AddService(zenjector.Metadata.Assembly);
            Container.Bind<IHttpService>().To<ContainerizedHttpService>().AsTransient().OnInstantiated<IHttpService>(HttpServiceCreated);

            // Bind any global services
            Container.BindInterfacesTo<FinishEventDispatcher>().AsSingle();
        }

        private void SiraLogCreated(InjectContext ctx, SiraLog siraLog)
        {
            // When a SiraLog is instantiated, add its backing logger and its default value for debug mode.
            SiraLogManager.LoggerContext loggerContext = _siraLogManager.LoggerFromAssembly(ctx.ObjectType.Assembly);
            siraLog.Setup(loggerContext.logger, ctx.ObjectType.Name, loggerContext.debugMode);
        }

        private void HttpServiceCreated(InjectContext ctx, IHttpService httpService)
        {
            // When an HttpService is instantied, add its backing service.
            IHttpService container = _httpServiceManager.ServiceFromAssembly(ctx.ObjectType.Assembly);
            (httpService as ContainerizedHttpService)!.Setup(container);
        }
    }
}