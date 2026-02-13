using HarmonyLib;
using UnityEngine.SpatialTracking;

namespace SiraUtil.Tweaks
{
    [HarmonyPatch(typeof(TrackedPoseDriver), nameof(TrackedPoseDriver.OnEnable))]
    internal class ForceTrackedPoseDriverUpdateOnEnable
    {
        private static void Postfix(TrackedPoseDriver __instance)
        {
            __instance.PerformUpdate();
        }
    }
}
