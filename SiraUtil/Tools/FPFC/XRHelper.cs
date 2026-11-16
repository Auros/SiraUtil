using SiraUtil.Logging;
using System.Linq;
using UnityEngine.XR.Management;

namespace SiraUtil.Tools.FPFC
{
    internal class XRHelper
    {
        private readonly SiraLog _siraLog;

        public XRHelper(SiraLog siraLog)
        {
            _siraLog = siraLog;
        }

        // we unfortunately need to fully deinitialize/initialize the XR loader since OpenXR doesn't simply stop/start properly
        internal void InitializeXRLoader()
        {
            XRManagerSettings manager = XRGeneralSettings.Instance.Manager;

            if (manager.activeLoader != null || !manager.activeLoaders.Any(l => l != null))
            {
                return;
            }

            _siraLog.Notice("Enabling XR Loader");
            manager.InitializeLoaderSync();

            if (!manager.isInitializationComplete)
            {
                _siraLog.Error("Failed to initialize XR loader");
                return;
            }

            manager.StartSubsystems();
        }

        internal void DeinitializeXRLoader()
        {
            XRManagerSettings manager = XRGeneralSettings.Instance.Manager;

            if (manager.activeLoader == null)
            {
                return;
            }

            _siraLog.Notice("Disabling XR Loader");
            manager.DeinitializeLoader();
        }
    }
}
