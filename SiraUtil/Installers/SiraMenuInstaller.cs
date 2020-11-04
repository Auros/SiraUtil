using Zenject;
using SiraUtil.Services;

namespace SiraUtil.Installers
{
    internal class SiraMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<Submission.Display>().AsSingle();
        }
    }
}