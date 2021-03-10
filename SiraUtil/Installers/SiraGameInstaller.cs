using Zenject;
using SiraUtil.Tools;
using SiraUtil.Services;
using System;

namespace SiraUtil.Zenject
{
    internal class SiraGameInstaller : Installer
    {
        private readonly Config _config;

        public SiraGameInstaller(Config config)
        {
            _config = config;
        }

        public override void InstallBindings()
        {
            if (_config.SongControl.Enabled)
            {
                Container.BindInterfacesTo<SongControl>().AsSingle();
            }
            Container.BindInterfacesAndSelfTo<SiraSaberEffectManager>().AsSingle();

            if (DateTime.Now.Month == 4 && DateTime.Now.Day == 1 && AprilFools.maxInSession >= AprilFools.sessionTrack)
            Container.BindInterfacesTo<AprilFools>().AsSingle();
        }
    }
}