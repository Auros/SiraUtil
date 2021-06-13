using SiraUtil.Submissions;
using Zenject;

namespace SiraUtil.Installers
{
    internal class SiraStandardCampaignInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<Submission>().AsSingle();
            Container.BindInterfacesTo<SubmissionCompletionInjector>().AsSingle();
        }
    }
}