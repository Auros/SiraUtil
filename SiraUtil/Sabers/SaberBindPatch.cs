using Zenject;
using HarmonyLib;
using ModestTree;
using System.Linq;
using UnityEngine;
using SiraUtil.Interfaces;
using System.Collections.Generic;

namespace SiraUtil.Sabers
{
	[HarmonyPatch(typeof(SaberModelContainer), nameof(SaberModelContainer.Start))]
	internal class SaberBindPatch
	{
		internal static bool Prefix(ref Saber ____saber, ref DiContainer ____container, ref SaberModelContainer __instance, SaberModelController ____saberModelControllerPrefab)
		{
			var providers = ____container.Resolve<List<IModelProvider>>().Where(x => x.Type.DerivesFrom(typeof(SaberModelController)));
			if (providers.Count() == 0)
			{
				var provider = ____container.Resolve<SaberProvider>();
				if (!provider.IsSafe())
				{
					provider.ModelPrefab = ____saberModelControllerPrefab;
				}
				return true;
			}
			var baseProvider = providers.First();
			SaberModelController saberModelController = null;
			var providerX = ____container.Resolve<SaberProvider>();
			if (!providerX.IsSafe())
			{
				providerX.ModelPrefab = new GameObject(baseProvider.GetType().FullName).AddComponent(baseProvider.Type) as SaberModelController;
			}
			____container.Inject(saberModelController);
			Accessors.SaberTrail(ref saberModelController) = Accessors.SaberTrail(ref ____saberModelControllerPrefab);
			Accessors.SaberGlowColor(ref saberModelController) = Accessors.SaberGlowColor(ref ____saberModelControllerPrefab);
			var glowColors = Accessors.SaberGlowColor(ref saberModelController);
			for (int i = 0; i < glowColors.Length; i++)
			{
				____container.Inject(glowColors[i]);
			}
			Accessors.FakeSaberGlowColor(ref saberModelController) = Accessors.FakeSaberGlowColor(ref ____saberModelControllerPrefab);
			var fakeGlowColors = Accessors.FakeSaberGlowColor(ref saberModelController);
			for (int i = 0; i < fakeGlowColors.Length; i++)
			{
				____container.Inject(fakeGlowColors[i]);
			}
			Accessors.SaberLight(ref saberModelController) = Accessors.SaberLight(ref ____saberModelControllerPrefab);
			saberModelController.gameObject.transform.SetParent(__instance.transform, false);
			saberModelController.Init(__instance.transform, ____saber);
			return false;
		}

		private static void SetupSaberParent(InjectContext _, object source)
		{
			if (source is SaberModelController saberController)
			{
				Plugin.Log.Info("Moving controller");
				var go = new GameObject(saberController.GetType().FullName);
				
				saberController.gameObject.transform.SetParent(go.transform, false);
			}
		}
	}
}