using SiraUtil.Affinity;

namespace SiraUtil.Tools.FPFC
{
    internal class FPFCFixDaemon : IAffinity
    {
        private readonly bool _isOculus;
        private readonly IFPFCSettings _fpfcSettings;
        private readonly IVRPlatformHelper _vrPlatformHelper;

        public FPFCFixDaemon(IFPFCSettings fpfcSettings, IVRPlatformHelper vrPlatformHelper)
        {
            _fpfcSettings = fpfcSettings;
            _vrPlatformHelper = vrPlatformHelper;
            _isOculus = _vrPlatformHelper is OculusVRHelper;
        }

        [AffinityPatch(typeof(OculusVRHelper), nameof(OculusVRHelper.hasInputFocus), AffinityMethodType.Getter)]
        protected void ForceInputFocus(ref bool __result)
        {
            if (_isOculus && _fpfcSettings.Enabled)
                __result = true;
        }
    }
}