using SiraUtil.Logging;
using SiraUtil.Zenject;
using Zenject;

namespace SiraUtil.Installers
{
    internal class SiraInitializationInstaller : Installer
    {
        private readonly SiraLogManager _siraLogManager;
        private readonly ZenjectManager _zenjectManager;

        public SiraInitializationInstaller(ZenjectManager zenjectManager)
        {
            _zenjectManager = zenjectManager;
            _siraLogManager = new SiraLogManager(zenjectManager);
        }

        public override void InstallBindings()
        {
            foreach (var zenjector in _zenjectManager.ActiveZenjectors)
                if (zenjector.Logger is not null)
                    _siraLogManager.AddLogger(zenjector.Metadata.Assembly, zenjector.Logger);

            Container.Bind<SiraLog>().AsTransient().OnInstantiated<SiraLog>(SiraLogCreated);
        }

        private void SiraLogCreated(InjectContext ctx, SiraLog siraLog)
        {
            SiraLogManager.LoggerContext loggerContext = _siraLogManager.LoggerFromAssembly(ctx.ObjectType.Assembly);
            siraLog.Setup(loggerContext.logger, ctx.ObjectType.Name, loggerContext.debugMode);
        }
    }
}