using System;

namespace SiraUtil.Affinity
{
    internal interface IAffinityPatcher : IDisposable
    {
        void Patch(IAffinity affinity);
        void Unpatch(IAffinity affinity);
    }
}