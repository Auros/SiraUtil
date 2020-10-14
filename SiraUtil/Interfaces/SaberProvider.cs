using Zenject;

namespace SiraUtil.Interfaces
{
	public class SaberProvider
	{
		private DiContainer _container;
		internal SaberModelController ModelPrefab { get; set; }

		public SaberProvider(DiContainer container)
		{
			_container = container;
		}

		public bool IsSafe()
		{
			return ModelPrefab != null;
		}

		public SaberModelController GetModel()
		{
			if (ModelPrefab == null)
			{
				Plugin.Log.Info("NOT SAFE");
				Plugin.Log.Info("NOT SAFE");
				Plugin.Log.Info("NOT SAFE");
			}
			return _container.InstantiatePrefab(ModelPrefab).GetComponent<SaberModelController>();
		}
	}
}