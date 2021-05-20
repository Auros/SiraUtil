using SiraUtil.Tools.FPFC;
using Zenject;

namespace SiraUtil.Installers
{
    internal class SiraSettingsInstaller : Installer
    {
        public override void InstallBindings()
        {
            Plugin.Log.Info("SIRA SETTINGS INSTALLLING HELLO HELLO");
            Container.Bind(typeof(ITickable), typeof(IFPFCSettings)).To<SimpleFPFCSettings>().AsSingle();
        }
    }
}