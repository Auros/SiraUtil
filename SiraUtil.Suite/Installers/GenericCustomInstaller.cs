using Zenject;

namespace SiraUtil.Suite.Installers
{
    internal class GenericCustomInstaller : Installer
    {
        public override void InstallBindings()
        {
            Plugin.Log.Notice($"Installing from {nameof(GenericCustomInstaller)}");
        }
    }
}