using SiraUtil.Tools.FPFC;
using System;
using System.Linq;
using Zenject;

namespace SiraUtil.Installers
{
    internal class SiraSettingsInstaller : Installer
    {
        private const string _fpfc = "fpfc";

        public override void InstallBindings()
        {
            Plugin.Log.Info("sgdfgsdfrg");
            Container.Bind(typeof(ITickable), typeof(IFPFCSettings)).To<SimpleFPFCSettings>().AsSingle();

            //if (Environment.GetCommandLineArgs().Any(a => a.ToLower() == _fpfc))
            Container.BindInterfacesTo<OriginalFPFCDisabler>().AsSingle().NonLazy();
        }
    }
}