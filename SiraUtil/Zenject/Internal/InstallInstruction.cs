using SiraUtil.Zenject.Internal.Filters;
using System;
using Zenject;

namespace SiraUtil.Zenject.Internal
{
    internal readonly struct InstallInstruction
    {
        public readonly IInstallFilter installFilter;
        public readonly Action<DiContainer> onInstall;

        public InstallInstruction(IInstallFilter installFilter, Action<DiContainer> onInstall)
        {
            this.onInstall = onInstall;
            this.installFilter = installFilter;
        }
    }
}