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
            // conservatively apply this patch only to the main camera
            if (__instance.TryGetComponent(out MainCamera _))
            {
                __instance.PerformUpdate();
            }
        }

        // conservatively only apply this patch if the camera controller has been disabled before the OnDisable() that calls this method
        [HarmonyPatch(nameof(TrackedPoseDriver.ResetToCachedLocalPosition))]
        [HarmonyPrefix]
        private static bool Prefix(TrackedPoseDriver __instance) => !__instance.TryGetComponent(out CameraController cameraController) || !cameraController.enabled;
    }
}
