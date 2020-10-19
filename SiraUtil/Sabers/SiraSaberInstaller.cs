using Zenject;
using SiraUtil.Services;

namespace SiraUtil.Sabers
{
	internal class SiraSaberInstaller : Installer
    {
        public override void InstallBindings()
        {
			Container.Bind<SaberProvider>().AsSingle();
			Container.BindFactory<SiraSaber, SiraSaber.Factory>().FromFactory<SiraSaber.SaberFactory>();
        }
    }
}