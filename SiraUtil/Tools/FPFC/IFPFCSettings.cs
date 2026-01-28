using System;
using System.ComponentModel;

namespace SiraUtil.Tools.FPFC
{
    /// <summary>
    /// Represents settings for FPFC. This is subject to changes in the future, so don't be surprised if I update it and you inherit this object.
    /// </summary>
    public interface IFPFCSettings : INotifyPropertyChanged
    {
        /// <summary>
        /// The FOV for the camera. This doesn't work when a VR headset is active.
        /// </summary>
        [Obsolete("To be removed with no alternative.")]
        float FOV { get; }

        /// <summary>
        /// Whether or not the controller is enabled.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// The arbituary move sensitivity.
        /// </summary>
        [Obsolete("To be removed with no alternative.")]
        float MoveSensitivity { get; }

        /// <summary>
        /// The arbituary mouse sensitivity.
        /// </summary>
        [Obsolete("To be removed with no alternative.")]
        float MouseSensitivity { get; }

        /// <summary>
        /// Ignores and inverts the default state of FPFC when changed.
        /// </summary>
        [Obsolete("This value is no longer set by the user. Do not rely on this value to figure out if the game was started with FPFC.")]
        bool Ignore { get; }

        /// <summary>
        /// Doesn't restore the camera back to the user when they toggle out of FPFC. Requested by Mawntee.
        /// </summary>
        [Obsolete("To be removed with no alternative.")]
        bool LockViewOnDisable { get; }

        /// <summary>
        /// Limit the frame rate to the current screen's refresh rate when FPFC is enabled. Can help prevent GPU coil whine.
        /// </summary>
        [Obsolete("To be removed with no alternative.")]
        bool LimitFrameRate { get; }

        /// <summary>
        /// Enable VSync when FPFC is enabled. Can help prevent GPU coil whine.
        /// </summary>
        [Obsolete("To be removed with no alternative.")]
        int VSyncCount { get; }

        /// <summary>
        /// Called when the object is changed.
        /// </summary>
        [Obsolete("Use the PropertyChanged event instead.")]
        event Action<IFPFCSettings> Changed;
    }
}