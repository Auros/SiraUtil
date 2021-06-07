using SiraUtil.Services.Controllers;
using SiraUtil.Tools.FPFC;
using Zenject;

namespace SiraUtil.Installers
{
    internal class SiraPlayerInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameMenuControllerAccessor>().AsSingle();
            Container.BindInterfacesTo<GameTransformFPFCListener>().AsSingle();
        }
    }
}