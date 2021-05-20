using Zenject;

namespace SiraUtil.Tools.FPFC
{
    internal class FPFCInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<IInitializable>().To<FPFCToggle>().AsSingle();
        }
    }
}