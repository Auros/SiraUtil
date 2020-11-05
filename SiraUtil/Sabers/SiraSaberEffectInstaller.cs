using Zenject;
using SiraUtil.Services;

namespace SiraUtil.Sabers
{
    public class SiraSaberEffectInstaller : Installer
    {
        public override void InstallBindings()
        {
            //Container.BindInterfacesAndSelfTo<SiraSaberEffectManager>().AsSingle();
        }
    }
}