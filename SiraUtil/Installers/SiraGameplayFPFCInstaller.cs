using SiraUtil.Services.Controllers;
using Zenject;

namespace SiraUtil.Installers
{
    internal class SiraAllPlayersFPFCInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameMenuControllerAccessor>().AsSingle();
        }
    }
}