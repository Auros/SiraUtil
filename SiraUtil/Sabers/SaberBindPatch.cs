using Zenject;
using HarmonyLib;
using ModestTree;
using System.Linq;
using UnityEngine;
using System.Reflection;
using SiraUtil.Services;
using SiraUtil.Interfaces;
using System.Reflection.Emit;
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
			var baseProvider = providers.OrderByDescending(x => x.Priority).First();
			SaberModelController saberModelController = null;
			var providerX = ____container.Resolve<SaberProvider>();
			if (!providerX.IsSafe())
			{
				saberModelController = providerX.ModelPrefab = new GameObject(baseProvider.GetType().FullName).AddComponent(baseProvider.Type) as SaberModelController;
			}
			else
			{
				saberModelController = providerX.ModelPrefab;
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
	}

	[HarmonyPatch(typeof(GameplayCoreInstaller), "InstallBindings")]
	public class GameplayCoreClashCheckerSwap
	{
		private static readonly MethodInfo _clashAttacher = SymbolExtensions.GetMethodInfo(() => ClashAttacher(null));

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var codes = instructions.ToList();
			int runIns = 0;
			for (int i = codes.Count - 1; i > 15; i--)
			{
				if (codes[i].opcode == OpCodes.Pop && codes[i - 1].opcode == OpCodes.Callvirt &&
					codes[i - 2].opcode == OpCodes.Callvirt && codes[i - 3].opcode == OpCodes.Call &&
					codes[i - 4].opcode == OpCodes.Ldarg_0)
				{
					runIns++;
					if (runIns == 3)
					{
						codes.Insert(i - 1, new CodeInstruction(OpCodes.Callvirt, _clashAttacher));
						break;
					}
				}
			}
			return codes.AsEnumerable();
		}

		private static FromBinderGeneric<SiraSaberClashChecker> ClashAttacher(ConcreteIdBinderGeneric<SaberClashChecker> contract)
		{
			return contract.To<SiraSaberClashChecker>();
		}
	}
}