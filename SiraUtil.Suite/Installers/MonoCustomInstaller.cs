using Zenject;

namespace SiraUtil.Suite.Installers
{
    internal class MonoCustomInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Plugin.Log.Notice($"Installing from {nameof(MonoCustomInstaller)}");
        }
    }
}
