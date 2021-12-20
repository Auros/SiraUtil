using SiraUtil.Services;
using SiraUtil.Services.Controllers;
using SiraUtil.Submissions;
using Zenject;

namespace SiraUtil.Installers
{
    internal class SiraMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<IMenuControllerAccessor>().To<MenuMenuControllerAccessor>().AsSingle();

            // Score Submission
            Container.Bind<SiraSubmissionViewController>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesTo<StandardDisplayer>().AsSingle();
            Container.BindInterfacesTo<MissionDisplayer>().AsSingle();
        }
    }
}