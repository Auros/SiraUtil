using HarmonyLib;
using Zenject;

namespace SiraUtil.Affinity
{
    internal class AffinityInstaller : Installer<AffinityInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<AffinityKernel>().AsSingle();
        }
    }
}