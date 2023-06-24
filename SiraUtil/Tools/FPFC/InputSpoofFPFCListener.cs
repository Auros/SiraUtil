using SiraUtil.Affinity;

namespace SiraUtil.Tools.FPFC
{
    internal class InputSpoofFPFCListener : IFPFCListener, IAffinity
    {
        private bool _active;
        private readonly DevicelessVRHelper _devicelessVRHelper = new();

        public void Enabled()
        {
            _active = true;
        }

        public void Disabled()
        {
            _active = false;
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(UnityXRHelper), nameof(IVRPlatformHelper.GetTriggerValue))]
        protected bool GetTriggerValueOverridePatch(ref float __result)
        {
            __result = _devicelessVRHelper.GetTriggerValue(default);
            return !_active;
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(OculusVRHelper), nameof(IVRPlatformHelper.GetTriggerValue))]
        protected bool GetOculusTriggerValueOverridePatch(ref float __result)
        {
            __result = _devicelessVRHelper.GetTriggerValue(default);
            return !_active;
        }
    }
}