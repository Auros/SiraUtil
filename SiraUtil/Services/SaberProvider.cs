using System;
using Zenject;

namespace SiraUtil.Services
{
	public class SaberProvider
	{
		private readonly DiContainer _container;

		private SaberModelController _saberModelController;
		internal SaberModelController ModelPrefab
		{
			get => _saberModelController;
			set
			{
				_saberModelController = value;
				ControllerReady?.Invoke();
			}
		}
		public event Action ControllerReady;

		public SaberProvider(DiContainer container)
		{
			_container = container;
		}

		public bool IsSafe()
		{
			var safe =  ModelPrefab != null;
			if (safe)
			{
				ControllerReady?.Invoke();
			}
			return safe;
		}

		public SaberModelController GetModel()
		{
			return ModelPrefab == null ? null : _container.InstantiatePrefab(ModelPrefab).GetComponent<SaberModelController>();
		}
	}
}