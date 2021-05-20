using Zenject;

namespace SiraUtil.Tools.FPFC
{
    internal class FPFCInstaller : Installer
    {
        public override void InstallBindings()
        {
            Plugin.Log.Info("INSTALLING");
            Container.Bind<IInitializable>().To<FPFCToggle>().AsSingle();
        }
    }
}