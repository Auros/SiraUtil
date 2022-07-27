using SiraUtil.Sabers;
using SiraUtil.Sabers.Effects;
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
            Container.BindInterfacesTo<GameTransformFPFCListener>().AsSingle();

            // SongControl stuff
            Container.BindInterfacesTo<SongControlManager>().AsSingle();

            // Saber API
            Container.BindInterfacesTo<ObstacleSaberSparkleEffectManagerLatch>().AsSingle();
            Container.BindInterfacesTo<SaberBurnMarkSparklesLatch>().AsSingle();
            Container.BindInterfacesAndSelfTo<SaberModelProvider>().AsSingle();
            Container.BindInterfacesAndSelfTo<SaberModelManager>().AsSingle();
            Container.BindInterfacesTo<SaberClashEffectAdjuster>().AsSingle();
            Container.BindInterfacesTo<SaberBurnMarkAreaLatch>().AsSingle();
            Container.Bind<SiraSaberFactory>().AsSingle();
        }
    }
}