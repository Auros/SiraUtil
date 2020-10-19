using System;
using Zenject;
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
			Container.BindInstance(_config.FPFCToggle).AsSingle();
			if (_config.FPFCToggle.Enabled)
			{
				Container.Bind<FPFCToggle>().FromNewComponentOnNewGameObject(nameof(FPFCToggle)).AsSingle().NonLazy();

			}
            if (_config.Localization.Enabled)
            {
                Container.Bind(typeof(IInitializable), typeof(IDisposable), typeof(WebClient)).To<WebClient>().AsSingle();
                Container.BindInterfacesAndSelfTo<Localizer>().AsSingle().NonLazy();
            }

            // Make Zenject know this is a list
            Container.Bind<IModelProvider>().To<DummyProviderA>().AsSingle();
            Container.Bind<IModelProvider>().To<DummyProviderB>().AsSingle();
        }

        private class DummyProviderA : IModelProvider { public int Priority => -9999; public Type Type => typeof(DummyProviderB); }
		private class DummyProviderB : IModelProvider { public int Priority => -9999; public Type Type => typeof(DummyProviderA); }
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

			//Container.Bind<IModelProvider>().To<GamerSaberProvider>().AsSingle();
			//Container.Bind<IModelProvider>().To<GamerSaberProvider2>().AsSingle();
		}
    }

	internal class SiraMultiGameInstaller : Installer
	{
		private readonly Config _config;

		public SiraMultiGameInstaller(Config config)
		{
			_config = config;
		}

		public override void InstallBindings()
		{
			if (_config.SongControl.Enabled)
			{
				Container.BindInstance(_config.SongControl.ExitKeyCode).WithId("ExitCode");
				Container.BindInstance(_config.SongControl.PauseToggleKeyCode).WithId("PauseToggleCode");
				Container.BindInterfacesTo<MultiSongControl>().AsSingle().NonLazy();
			}
		}
	}

	/*
	public class GamerSaberController : SaberModelController
	{
		public override void Init(Transform transform, Saber saber)
		{
			Plugin.Log.Info("bruh moment");
		}
	}

	public class GamerSaberProvider : IModelProvider
	{
		public Type Type => typeof(GamerSaberController);
		public int Priority => 200;
	}
	public class TheInstaller : Installer
	{
		public override void InstallBindings()
		{
			Container.Bind<IModelProvider>().To<GamerSaberProvider>().AsSingle();
		}
	}

	public class GamerSaberController2 : SaberModelController
	{
		public override void Init(Transform transform, Saber saber)
		{
			Plugin.Log.Info("bruh moment 2");
		}
	}

	public class GamerSaberProvider2 : IModelProvider
	{
		public Type Type => typeof(GamerSaberController2);
		public int Priority => 201;
	}
	public class TheInstaller2 : Installer
	{
		public override void InstallBindings()
		{
			Container.Bind<IModelProvider>().To<GamerSaberProvider>().AsSingle();
		}
	}*/
}