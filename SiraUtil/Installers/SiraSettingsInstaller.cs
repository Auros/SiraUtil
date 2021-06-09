using SiraUtil.Tools.FPFC;
using System;
using System.Linq;
using Zenject;

namespace SiraUtil.Installers
{
    internal class SiraSettingsInstaller : Installer
    {
        private readonly Config _config;

        public SiraSettingsInstaller(Config config)
        {
            _config = config;
        }

        public override void InstallBindings()
        {
            Container.BindInstance(_config.FPFCToggle).AsSingle();
            Container.BindInstance(_config.SongControl).AsSingle();
            Container.BindInterfacesTo<FPFCSettingsController>().AsSingle();

            if (Environment.GetCommandLineArgs().Any(a => a.ToLower() == FPFCToggle.Argument))
                Container.BindInterfacesTo<OriginalFPFCDisabler>().AsSingle().NonLazy();
        }
    }
}