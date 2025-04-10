using HarmonyLib;
using System.Threading.Tasks;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;
using Zenject;

namespace SiraUtil.Tweaks
{
    /// <summary>
    /// This patch fixes an issue with SteamVR where Unity will decide to not use SteamVR's room center
    /// and instead try to invent its own origin, often resulting in the headset being under the floor.
    /// This tends to happen when using FPFCToggle.
    /// </summary>
    /// <remarks>
    /// This post explains the various possible OpenXR tracking spaces: <see href="https://discussions.unity.com/t/how-to-recenter-in-openxr/935209/9"/>
    /// </remarks>
    [HarmonyPatch]
    internal static class DisableOpenXRRecentering
    {
        private static bool ShouldDisableRecentering => OpenXRRuntime.name == "SteamVR/OpenXR" && OpenXRSettings.AllowRecentering;

        [HarmonyPatch(typeof(SceneContext), nameof(SceneContext.Awake))]
        [HarmonyPostfix]
        internal static async void RestartXRLoaderIfNecessary()
        {
            if (XRGeneralSettings.Instance.Manager.activeLoader is not OpenXRLoaderBase || !ShouldDisableRecentering)
            {
                return;
            }

            Plugin.Log.Notice("Restarting XR loader");

            XRManagerSettings manager = XRGeneralSettings.Instance.Manager;

            if (manager.activeLoader != null)
            {
                Plugin.Log.Info($"Deinitializing XR loader '{manager.activeLoader.name}'");
                manager.DeinitializeLoader();

                await Task.Yield();
            }

            manager.InitializeLoaderSync();
            manager.StartSubsystems();

            if (manager.activeLoader != null)
            {
                Plugin.Log.Info($"Initialized XR loader '{manager.activeLoader.name}'");
            }
            else
            {
                Plugin.Log.Error("Failed to initialize any XR loader");
            }
        }

        [HarmonyPatch(typeof(OpenXRLoaderBase), nameof(OpenXRLoaderBase.Initialize))]
        [HarmonyPostfix]
        private static void OpenXRLoaderBase_Initialize(ref bool __result)
        {
            if (!__result || !ShouldDisableRecentering)
            {
                return;
            }

            Plugin.Log.Info("Disabling recentering");
            OpenXRSettings.SetAllowRecentering(false, 0);
        }
    }
}
