using SiraUtil.Tools.SongControl;
using Zenject;

namespace SiraUtil.Installers
{
    internal class SiraMultiplayerInstaller : Installer
    {
        public override void InstallBindings()
        {
            // Binds ISongControl
            Container.BindInterfacesTo<MultiplayerSongControl>().AsSingle();
        }
    }
}