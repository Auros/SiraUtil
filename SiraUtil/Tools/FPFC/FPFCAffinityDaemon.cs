using SiraUtil.Affinity;

namespace SiraUtil.Tools.FPFC
{
    internal class FPFCAffinityDaemon : IAffinity
    {
        private readonly IFPFCSettings _fpfcSettings;

        public FPFCAffinityDaemon(IFPFCSettings fpfcSettings)
        {
            _fpfcSettings = fpfcSettings;
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(FirstPersonFlyingController), nameof(FirstPersonFlyingController.OnEnable))]
        protected bool FPFCOnEnable() => false;

        [AffinityPrefix]
        [AffinityPatch(typeof(FirstPersonFlyingController), nameof(FirstPersonFlyingController.OnDisable))]
        protected bool FPFCOnDisable() => false;

        [AffinityPrefix]
        [AffinityPatch(typeof(FirstPersonFlyingController), nameof(FirstPersonFlyingController.Update))]
        protected bool FPFCOnUpdate() => false;

        [AffinityPrefix]
        [AffinityPatch(typeof(SmoothCameraController), nameof(SmoothCameraController.ActivateSmoothCameraIfNeeded))]
        protected bool OnlyEnableSmoothCameraIfNeeded()
        {
            if (_fpfcSettings.Enabled)
                return false;
            return true;
        }

    }
}