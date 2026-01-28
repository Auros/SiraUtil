using SiraUtil.Tools.FPFC;
using System;
using System.Linq;
using Zenject;

namespace SiraUtil.Installers
{
    internal class SiraSettingsInstaller : Installer
    {
        private const string FpfcDisableArgument = "--no-sirautil-fpfc";

        private readonly Config _config;

        public SiraSettingsInstaller(Config config)
        {
            _config = config;
        }

        public override void InstallBindings()
        {
            Container.BindInstance(_config.FPFCToggle).AsSingle();
            Container.BindInstance(_config.SongControl).AsSingle();

            string[] args = Environment.GetCommandLineArgs();
            if (!args.Contains(FpfcDisableArgument, StringComparer.OrdinalIgnoreCase))
            {
                Container.BindInterfacesTo<FPFCSettingsController>().AsSingle();
                Container.BindInterfacesTo<FPFCAffinityDaemon>().AsSingle().NonLazy();
            }
            else
            {
                Container.Bind<IFPFCSettings>().To<NoFPFCSettings>().AsSingle();
            }
        }
    }
}