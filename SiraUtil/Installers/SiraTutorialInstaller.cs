using SiraUtil.Services.Controllers;
using SiraUtil.Tools.FPFC;
using SiraUtil.Tools.SongControl;
using Zenject;

namespace SiraUtil.Installers
{
    internal class SiraTutorialInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameMenuControllerAccessor>().AsSingle();
            Container.BindInterfacesTo<GameTransformFPFCListener>().AsSingle();
            Container.BindInterfacesTo<SongControlManager>().AsSingle();
        }
    }
}