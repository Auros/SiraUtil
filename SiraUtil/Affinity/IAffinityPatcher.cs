using System;
using System.Reflection;

namespace SiraUtil.Affinity
{
    internal interface IAffinityPatcher : IDisposable
    {
        Guid? Patch(IAffinity affinity);
        void Unpatch(Guid contract, Assembly owner);
    }
}