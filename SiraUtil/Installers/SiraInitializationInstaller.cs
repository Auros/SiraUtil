using IPA.Logging;
using SiraUtil.Logging;
using SiraUtil.Services.Events;
using SiraUtil.Submissions;
using SiraUtil.Tools.FPFC;
using SiraUtil.Web;
using SiraUtil.Web.SiraSync;
using SiraUtil.Web.SiraSync.Zenject;
using SiraUtil.Web.Zenject;
using SiraUtil.Zenject;
using Zenject;

namespace SiraUtil.Installers
{
    internal class SiraInitializationInstaller : Installer
    {
        private readonly Zenjector _siraUtil;
        private readonly SiraLogManager _siraLogManager;
        private readonly ZenjectManager _zenjectManager;
        private readonly SiraSyncManager _siraSyncManager;
        private readonly HttpServiceManager _httpServiceManager;

        public SiraInitializationInstaller(Zenjector siraUtil, ZenjectManager zenjectManager)
        {
            _siraUtil = siraUtil;
            _zenjectManager = zenjectManager;
            _siraLogManager = new(zenjectManager);
            _httpServiceManager = new(zenjectManager, _siraUtil.Metadata);
            _siraSyncManager = new SiraSyncManager(siraUtil, zenjectManager, _httpServiceManager);
        }

        public override void InstallBindings()
        {
            Container.BindInterfacesTo<FPFCFixDaemon>().AsSingle();
            Container.BindInterfacesTo<InputSpoofFPFCListener>().AsSingle();

            // Install all UBinders
            foreach (Zenjector zenjector in _zenjectManager.ActiveZenjectors)
            {
                if (zenjector.UBinderType is not null && zenjector.UBinderValue is not null)
                {
                    Container.Bind(zenjector.UBinderType).FromInstance(zenjector.UBinderValue).AsSingle();
                }
            }

            // Takes every active zenjector and adds them to the SiraLogger
            _siraLogManager.Clear();
            foreach (Zenjector zenjector in _zenjectManager.ActiveZenjectors)
            {
                if (zenjector.Logger is not null)
                {
                    _siraLogManager.AddLogger(zenjector.Metadata.Assembly, zenjector.Logger);
                }
            }

            Container.Bind<SiraLog>().AsTransient().OnInstantiated<SiraLog>(SiraLogCreated);
            Container.Bind<Logger>().FromMethod(CreateIPAChildLogger).AsTransient();

            // Takes every active zenjector and adds them to the http service.
            _httpServiceManager.Clear();
            foreach (Zenjector zenjector in _zenjectManager.ActiveZenjectors)
            {
                if (zenjector.HttpServiceType is not null)
                {
                    _httpServiceManager.AddService(zenjector.Metadata.Assembly);
                }
            }

            Container.Bind<IHttpService>().To<ContainerizedHttpService>().AsTransient().OnInstantiated<IHttpService>(HttpServiceCreated);

            // Takes every active zenjector and adds them to the SiraSync service.
            _siraSyncManager.Clear();
            foreach (Zenjector zenjector in _zenjectManager.ActiveZenjectors)
            {
                if (zenjector.SiraSyncServiceType is not null)
                {
                    _siraSyncManager.AddService(zenjector.Metadata.Assembly);
                }
            }

            Container.Bind<ISiraSyncService>().To<ContainerizedSiraSyncService>().AsTransient().OnInstantiated<ISiraSyncService>(SiraSyncServiceCreated);

            // Bind any global services
            Container.BindInterfacesTo<FinishEventDispatcher>().AsSingle();
            Container.Bind<SubmissionDataContainer>().AsSingle();
        }

        private void SiraLogCreated(InjectContext ctx, SiraLog siraLog)
        {
            // When a SiraLog is instantiated, add its backing logger and its default value for debug mode.
            SiraLogManager.LoggerContext loggerContext = _siraLogManager.LoggerFromAssembly(ctx.ObjectType.Assembly);
            siraLog.Setup(loggerContext.logger, ctx.ObjectType.Name, loggerContext.debugMode);
        }

        private Logger CreateIPAChildLogger(InjectContext ctx)
        {
            SiraLogManager.LoggerContext loggerContext = _siraLogManager.LoggerFromAssembly(ctx.ObjectType.Assembly);
            return loggerContext.logger.GetChildLogger(ctx.ObjectType.Name);
        }

        private void HttpServiceCreated(InjectContext ctx, IHttpService httpService)
        {
            // When a ContainerizedHttpService is instantied, add its backing service.
            IHttpService container = _httpServiceManager.ServiceFromAssembly(ctx.ObjectType.Assembly);
            (httpService as ContainerizedHttpService)!.Setup(container);
        }

        private void SiraSyncServiceCreated(InjectContext ctx, ISiraSyncService siraSyncService)
        {
            // When a ContainerizedSiraSyncService is instantiated, add its backing service.
            ISiraSyncService container = _siraSyncManager.ServiceFromAssembly(ctx.ObjectType.Assembly);
            (siraSyncService as ContainerizedSiraSyncService)!.Setup(container);
        }
    }
}