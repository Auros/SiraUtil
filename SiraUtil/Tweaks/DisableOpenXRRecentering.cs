using HarmonyLib;
using System;
using System.Linq;
using UnityEngine.XR.Management;
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
        // XR_EXT_local_floor is built into the 1.1 spec: https://www.khronos.org/blog/stepping-up-the-floor-is-yours-with-promotion-of-the-xr-ext-local-floor-extension-to-openxr-1.1-core
        private static readonly Version OpenXRApiLocalFloorMinVersion = new(1, 1, 0);

        internal static void DisableIfLoaded()
        {
            if (XRGeneralSettings.Instance.Manager.activeLoader is OpenXRLoaderBase)
            {
                DisableIfNecessary();
            }
        }

        private static void Postfix(ref bool __result)
        {
            if (__result)
            {
                DisableIfNecessary();
            }
        }

        private static void DisableIfNecessary()
        {
            if (new Version(OpenXRRuntime.apiVersion) < OpenXRApiLocalFloorMinVersion && !OpenXRRuntime.GetEnabledExtensions().Contains("XR_EXT_local_floor"))
            {
                Plugin.Log.Info("Disabling recentering");
                OpenXRSettings.SetAllowRecentering(false, 0);
            }
        }
    }
}
