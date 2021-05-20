using UnityEngine;

namespace SiraUtil.Tools.FPFC
{
    internal class FPFCState
    {
        public float CameraFOV { get; set; } = 90f;
        public StereoTargetEyeMask StereroTarget { get; set; } = StereoTargetEyeMask.None;
        public float Aspect { get; set; } = 1f;
    }
}