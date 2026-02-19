using SiraUtil.Zenject.Internal.Filters;
using System;
using Zenject;

namespace SiraUtil.Zenject.Internal
{
    internal record InstallInstruction
    {
        public InstallInstruction(IInstallFilter installFilter, Action<DiContainer> onInstall)
        {
            OnInstall = onInstall;
            InstallFilter = installFilter;
        }

        public IInstallFilter InstallFilter { get; }

        public Action<DiContainer> OnInstall { get; }
    }
}