using SiraUtil.Suite.Tests.Affinity;
using Zenject;

namespace SiraUtil.Suite.Tests.Installers
{
    internal class AffinityTestInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<MainMenuViewControllerPatchTest>().AsSingle();
        }
    }
}