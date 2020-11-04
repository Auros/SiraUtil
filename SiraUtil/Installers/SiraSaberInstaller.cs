using Zenject;
using SiraUtil.Sabers;
using SiraUtil.Services;

namespace SiraUtil.Installers
{
    internal class SiraSaberInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<SaberProvider>().AsSingle().NonLazy();
            Container.BindFactory<SiraSaber, SiraSaber.Factory>().FromFactory<SiraSaber.SaberFactory>();
        }
    }
}