using SiraUtil.Services.Events;
using SiraUtil.Submissions;
using SiraUtil.Tools.FPFC;
using Zenject;

namespace SiraUtil.Installers
{
    internal class SiraGlobalServicesInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<FPFCFixDaemon>().AsSingle();
            Container.BindInterfacesTo<InputSpoofFPFCListener>().AsSingle();
            Container.BindInterfacesTo<FinishEventDispatcher>().AsSingle();
            Container.Bind<SubmissionDataContainer>().AsSingle();
        }
    }
}