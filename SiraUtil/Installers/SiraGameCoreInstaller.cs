using SiraUtil.Objects;
using Zenject;

namespace SiraUtil.Installers
{
    internal class SiraGameCoreInstaller : Installer
    {
        public override void InstallBindings()
        {
            // Object API
            Container.BindInterfacesTo<ContractRecycler>().AsSingle();
        }
    }
}