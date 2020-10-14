using SiraUtil.Interfaces;
using Zenject;

namespace SiraUtil.Sabers
{
    internal class SiraSaberInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
			Container.Bind<SaberProvider>().AsSingle();
			Container.BindFactory<SiraSaber, SiraSaber.Factory>().FromFactory<SiraSaber.SaberFactory>();
            Container.Bind<SiraSaberEffectManager>().FromNewComponentOnRoot().AsSingle().NonLazy();
        }
    }
}