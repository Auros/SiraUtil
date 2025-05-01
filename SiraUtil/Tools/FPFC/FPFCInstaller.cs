using Zenject;

namespace SiraUtil.Tools.FPFC
{
    internal class SiraFullFPFCInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<FPFCToggle>().AsSingle();
            Container.BindInterfacesTo<SmoothCameraListener>().AsSingle();
        }
    }
}