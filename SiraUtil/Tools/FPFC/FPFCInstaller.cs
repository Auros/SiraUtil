using System;
using System.Linq;
using Zenject;

namespace SiraUtil.Tools.FPFC
{
    internal class SiraFullFPFCInstaller : Installer
    {
        public override void InstallBindings()
        {
            var args = Environment.GetCommandLineArgs();
            if (!args.Any(a => a.Equals(FPFCToggle.EnableArgument, StringComparison.OrdinalIgnoreCase) || args.Any(a => a.Equals(FPFCToggle.DisableArgument, StringComparison.OrdinalIgnoreCase))))
            {
                Container.Bind<IFPFCSettings>().To<NoFPFCSettings>().AsSingle();
                return;
            }
            Container.BindInterfacesTo<FPFCToggle>().AsSingle();
            Container.BindInterfacesTo<MenuFPFCListener>().AsSingle();
            Container.BindInterfacesTo<SmoothCameraListener>().AsSingle();
        }
    }
}