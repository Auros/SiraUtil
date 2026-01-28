using HarmonyLib;
using SiraUtil.Tools.FPFC;
using UnityEngine.SpatialTracking;

namespace SiraUtil.Tweaks
{
    [HarmonyPatch(typeof(TrackedPoseDriver))]
    internal class FixTrackedPoseDriverEnableDisablePosition
    {
        [HarmonyPatch(nameof(TrackedPoseDriver.OnEnable))]
        [HarmonyPostfix]
        private static void Postfix(TrackedPoseDriver __instance)
        {
            if (ShouldApplyPatch(__instance))
            {
                __instance.PerformUpdate();
            }
        }

        [HarmonyPatch(nameof(TrackedPoseDriver.ResetToCachedLocalPosition))]
        [HarmonyPrefix]
        private static bool Prefix(TrackedPoseDriver __instance) => !ShouldApplyPatch(__instance);

        private static bool ShouldApplyPatch(TrackedPoseDriver trackedPoseDriver) => trackedPoseDriver.TryGetComponent(out CameraController cameraController) && cameraController.enabled;
    }
}
