using SiraUtil.Affinity;

namespace SiraUtil.Tools.FPFC
{
    internal class OriginalFPFCDisabler : IAffinity
    {
        [AffinityPrefix]
        [AffinityPatch(typeof(FirstPersonFlyingController), nameof(FirstPersonFlyingController.Start))]
        protected bool FPFCStart(ref FirstPersonFlyingController __instance)
        {
            Plugin.Log.Info("q");
            __instance.enabled = false;
            return false;
        }
        
        [AffinityPrefix]
        [AffinityPatch(typeof(FirstPersonFlyingController), nameof(FirstPersonFlyingController.OnEnable))]
        protected bool FPFCOnEnable()
        {
            Plugin.Log.Info("x");
            return false;
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(FirstPersonFlyingController), nameof(FirstPersonFlyingController.OnDisable))]
        protected bool FPFCOnDisable()
        {
            Plugin.Log.Info("e");
            return false;
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(FirstPersonFlyingController), nameof(FirstPersonFlyingController.Update))]
        protected bool FPFCUpdate()
        {
            Plugin.Log.Info("g");
            return false;
        }
    }
}