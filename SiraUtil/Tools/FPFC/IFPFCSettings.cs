using System;

namespace SiraUtil.Tools.FPFC
{
    internal interface IFPFCSettings
    {
        float FOV { get; }
        bool Enabled { get; }
        event Action<IFPFCSettings> Changed;
    }
}