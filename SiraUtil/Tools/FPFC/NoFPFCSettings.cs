using System;

namespace SiraUtil.Tools.FPFC
{
    internal class NoFPFCSettings : IFPFCSettings
    {
        public float FOV => default;

        public bool Enabled
        {
            get => default;
            set => _ = false;
        }

        public float MoveSensitivity => default;

        public float MouseSensitivity => default;

        public bool Ignore => default;

        public event Action<IFPFCSettings>? Changed;

        public void CallEvent()
        {
            Changed?.Invoke(this);
        }
    }
}