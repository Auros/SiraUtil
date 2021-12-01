using SiraUtil.Affinity;

namespace SiraUtil.Tools.FPFC
{
    internal class OriginalFPFCDisabler : IAffinity
    {
        [AffinityPrefix]
        [AffinityPatch(typeof(FirstPersonFlyingController), nameof(FirstPersonFlyingController.Start))]
        protected bool FPFCStart(ref FirstPersonFlyingController __instance) => false;

        [AffinityPrefix]
        [AffinityPatch(typeof(FirstPersonFlyingController), nameof(FirstPersonFlyingController.OnEnable))]
        protected bool FPFCOnEnable() => false;

        [AffinityPrefix]
        [AffinityPatch(typeof(FirstPersonFlyingController), nameof(FirstPersonFlyingController.OnDisable))]
        protected bool FPFCOnDisable() => false;

        [AffinityPrefix]
        [AffinityPatch(typeof(FirstPersonFlyingController), nameof(FirstPersonFlyingController.Update))]
        protected bool FPFCOnUpdate() => false;
    }
}