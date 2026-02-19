using SiraUtil.Zenject.Internal.Filters;
using System;

namespace SiraUtil.Zenject.Internal
{
    internal record InstallSet
    {
        public InstallSet(Type installerType, IInstallFilter installFilter, object[]? initialParameters = null)
        {
            InstallerType = installerType;
            InstallFilter = installFilter;
            InitialParameters = initialParameters;
        }

        public Type InstallerType { get; }

        public IInstallFilter InstallFilter { get; }

        public object[]? InitialParameters { get; }
    }
}