using HarmonyLib;
using UnityEngine.XR.OpenXR;

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
    [HarmonyPatch(typeof(OpenXRLoaderBase), nameof(OpenXRLoaderBase.Initialize))]
    internal static class DisableOpenXRRecentering
    {
        private static void Postfix(ref bool __result)
        {
            if (__result && OpenXRRuntime.name == "SteamVR/OpenXR")
            {
                Plugin.Log.Info("Disabling recentering");
                OpenXRSettings.SetAllowRecentering(false, 0);
            }
        }
    }
}
