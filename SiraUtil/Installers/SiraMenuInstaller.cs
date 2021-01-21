using Zenject;
using UnityEngine;
using SiraUtil.Services;

namespace SiraUtil.Installers
{
    internal class SiraMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<Submission.Display>().AsSingle();
            Container.Bind<Submission.SiraSubmissionView>().FromNewComponentAsViewController().AsSingle();
            Container.Resolve<CanvasContainer>().CurvedCanvasTemplate = Container.Resolve<MainMenuViewController>().GetComponent<Canvas>();
        }
    }
}