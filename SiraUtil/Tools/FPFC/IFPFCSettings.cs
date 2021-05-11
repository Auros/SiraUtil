using System;

namespace SiraUtil.Tools.FPFC
{
    internal interface IFPFCSettings
    {
        bool Enabled { get; }
        event Action<bool> Changed;
    }
}