using System;
using System.ComponentModel;

namespace SiraUtil.Tools.FPFC
{
    internal class NoFPFCSettings : IFPFCSettings
    {
        public float FOV => default;

        public bool Enabled
        {
            get => default;
            set { }
        }

        public bool Active => false;

        public float MoveSensitivity => default;

        public float MouseSensitivity => default;

        public bool Ignore => default;

        public bool LockViewOnDisable => false;

        public bool LimitFrameRate => false;

        public int VSyncCount => 0;

        public event Action<IFPFCSettings>? Changed { add { } remove { } }

        public event PropertyChangedEventHandler? PropertyChanged { add { } remove { } }
    }
}