using UnityEngine;
using Zenject;

namespace SiraUtil.Sabers
{
    internal class SiraSaberInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<SaberTest>().FromNewComponentOnRoot().AsSingle().NonLazy();
            Container.BindFactory<SiraSaber, SiraSaber.Factory>().FromFactory<SiraSaber.SaberFactory>();
        }
    }
}