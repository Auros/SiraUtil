using HarmonyLib;
using SiraUtil.Tools.FPFC;
using UnityEngine.XR.OpenXR;
using Zenject;

namespace SiraUtil.Tweaks
{
    /// <summary>
    /// Patches <see cref="BeatSaberInit.PreInitializeAsync(DiContainer)"/> so we always attempt to enable the XR loader (if any) before the game checks <see cref="OpenXRRuntime.name"/>.
    /// This prevents the startup error screen from showing up if the game is started normally but an internal restart is triggered while in FPFC.
    /// </summary>
    [HarmonyPatch]
    internal class EnableXRLoaderOnRestart
    {
        [HarmonyPatch(typeof(BeatSaberInit), nameof(BeatSaberInit.PreInitializeAsync))]
        [HarmonyPrefix]
        private static void PreInitializeAsync(DiContainer container)
        {
            // Unity automatically start up the XR loader very early during game load, so this is a no-op on first start
            container.Resolve<XRHelper>().InitializeXRLoader();
        }
    }
}
