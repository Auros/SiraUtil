using System;

namespace SiraUtil.Tools.FPFC
{
    internal interface IFPFCSettings
    {
        float FOV { get; }
        bool Enabled { get; }
        float MouseSensitivity { get; }

        event Action<IFPFCSettings> Changed;
    }
}