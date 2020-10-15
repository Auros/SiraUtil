using HarmonyLib;
using System.Linq;

namespace SiraUtil.Zenject.HarmonyPatches
{
	[HarmonyPatch(typeof(GameScenesManager), "PushScenes")]
	internal class GamemodeDecorator
	{
		internal static void Prefix(ScenesTransitionSetupDataSO scenesTransitionSetupData)
		{
			SiraContextDecorator.LastTransitionSetupName = scenesTransitionSetupData.GetType().Name;
			scenesTransitionSetupData.scenes.ToList().ForEach(x => Plugin.Log.Info(x.sceneName));
			// Let's check if it has a gamemode associated with it.
			var gameModeProperty = scenesTransitionSetupData.GetType().GetProperty("gameMode");
			if (gameModeProperty != null)
			{
				SiraContextDecorator.LastGamemodeSetupName = gameModeProperty.GetValue(scenesTransitionSetupData) as string;
			}
			if (scenesTransitionSetupData.scenes.Length == 3)
			{
				SiraContextDecorator.LastMidSceneName = scenesTransitionSetupData.scenes[1].sceneName;
			}
		}
	}
}