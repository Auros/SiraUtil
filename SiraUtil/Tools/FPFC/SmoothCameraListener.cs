using SiraUtil.Affinity;

namespace SiraUtil.Tools.FPFC
{
    internal class SmoothCameraListener : IFPFCListener, IAffinity
    {
        private SmoothCamera? _smoothCamera;
        private SmoothCameraController? _smoothCameraController;

        [AffinityPatch(typeof(SmoothCameraController), nameof(SmoothCameraController.Start))]
        protected void AcquireSmoothCamera(SmoothCameraController __instance, SmoothCamera ____smoothCamera)
        {
            if (_smoothCamera != null)
                return;

            _smoothCamera = ____smoothCamera;
            _smoothCameraController = __instance;
        }

        public void Enabled()
        {
            if (_smoothCamera != null && _smoothCamera.enabled)
                _smoothCamera.enabled = false;
        }

        public void Disabled()
        {
            if (_smoothCameraController != null)
            {
                _smoothCameraController.ActivateSmoothCameraIfNeeded();
            }
        }
    }
}