﻿using SiraUtil.Events;
using SiraUtil.Services;
using SiraUtil.Services.Controllers;
using SiraUtil.Submissions;
using Zenject;

namespace SiraUtil.Installers
{
    internal class SiraMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<IMenuControllerAccessor>().To<MenuMenuControllerAccessor>().AsSingle();
            Container.BindInterfacesAndSelfTo<MenuLevelEvents>().AsSingle();

            // Score Submission
            Container.Bind<SiraSubmissionViewController>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesTo<StandardDislayer>().AsSingle();
            Container.BindInterfacesTo<MissionDisplayer>().AsSingle();
        }
    }
}