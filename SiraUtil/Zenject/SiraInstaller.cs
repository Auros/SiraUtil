using System;
using Zenject;
using UnityEngine;
using SiraUtil.Tools;
using SiraUtil.Interfaces;

namespace SiraUtil.Zenject
{
	internal class SiraInstaller : Installer<Config, SiraInstaller>
	{

		private readonly Config _config;

		public SiraInstaller(Config config)
		{
			_config = config;
		}

		public override void InstallBindings()
		{
			Container.BindInstance(_config).AsSingle().NonLazy();
			Container.BindInstance(_config.FPFCToggle.Enabled).WithId("FPFCEnabled").WhenInjectedInto<FPFCToggle>();
			Container.BindInstance(_config.FPFCToggle.CameraFOV).WithId("CameraFOV").WhenInjectedInto<FPFCToggle>();
			Container.BindInstance(_config.FPFCToggle.ToggleKeyCode).WithId("ToggleCode").WhenInjectedInto<FPFCToggle>();
			Container.BindInstance(_config.FPFCToggle.MoveSensitivity).WithId("MoveSensitivity").WhenInjectedInto<FPFCToggle>();
			Container.Bind<FPFCToggle>().FromNewComponentOn(new GameObject("FPFCToggle")).AsSingle().NonLazy();
			if (_config.Localization.Enabled)
			{
				Container.Bind(typeof(IInitializable), typeof(IDisposable), typeof(WebClient)).To<WebClient>().AsSingle();
				Container.BindInterfacesAndSelfTo<Localizer>().AsSingle().NonLazy();
			}

			// Make Zenject know this is a list
			Container.Bind<IModelProvider>().To<DummyProviderA>().AsSingle();
			Container.Bind<IModelProvider>().To<DummyProviderB>().AsSingle();
		}

		private class DummyProviderA : IModelProvider { public int Priority => -9999; }
		private class DummyProviderB : IModelProvider { public int Priority => -9999; }
	}

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
				Container.BindInstance(_config.SongControl.ExitKeyCode).WithId("ExitCode");
				Container.BindInstance(_config.SongControl.RestartKeyCode).WithId("RestartCode");
				Container.BindInstance(_config.SongControl.PauseToggleKeyCode).WithId("PauseToggleCode");
				Container.BindInterfacesTo<SongControl>().AsSingle().NonLazy();
			}
		}
	}
}
