using SiraUtil.Services.Controllers;
using SiraUtil.Tools.FPFC;
using SiraUtil.Tools.SongControl;
using Zenject;

namespace SiraUtil.Installers
{
    // This class contains bindings that should work for EVERY song gamemode (Standard, Campaign, Tutorial, and Multiplayer)
    internal class SiraGameplayInstaller : Installer
    {
        public override void InstallBindings()
        {
            // FPFC stuff
            Container.BindInterfacesAndSelfTo<GameMenuControllerAccessor>().AsSingle();
            Container.BindInterfacesTo<GameTransformFPFCListener>().AsSingle();

            // SongControl stuff
            Container.BindInterfacesTo<SongControlManager>().AsSingle();
        }
    }
}