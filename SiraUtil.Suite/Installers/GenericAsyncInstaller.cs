using BGLib.AppFlow.Initialization;
using System.Threading.Tasks;
using Zenject;

namespace SiraUtil.Suite.Installers
{
    internal class GenericAsyncInstaller : AsyncInstaller
    {
        public override void InstallBindings()
        {
            Plugin.Log.Info("GenericAsyncInstaller.InstallBindings");
        }

        protected override Task LoadResourcesBeforeInstallAsync(IInstallerRegistry registry, DiContainer container)
        {
            Plugin.Log.Info("GenericAsyncInstaller.LoadResourcesBeforeInstallAsync");
            return Task.CompletedTask;
        }
    }
}
