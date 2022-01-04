using SiraUtil.Services;
using SiraUtil.Services.Controllers;
using Zenject;

namespace SiraUtil.Installers
{
    internal class SiraCreditsInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<IMenuControllerAccessor>().To<MenuMenuControllerAccessor>().AsSingle();
        }
    }
}