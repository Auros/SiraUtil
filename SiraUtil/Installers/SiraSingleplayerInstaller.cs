using SiraUtil.Tools.SongControl;
using Zenject;

namespace SiraUtil.Installers
{
    internal class SiraSingleplayerInstaller : Installer
    {
        public override void InstallBindings()
        {
            // Binds ISongControl
            Container.BindInterfacesTo<StandardSongControl>().AsSingle();
        }
    }
}