using SiraUtil.Zenject.Internal.Filters;
using System;

namespace SiraUtil.Zenject.Internal
{
    internal record InstallSet
    {
        public InstallSet(Type installerType, IInstallFilter installFilter, object[]? initialParameters = null, bool isAsyncInstaller = false)
        {
            InstallerType = installerType;
            InstallFilter = installFilter;
            InitialParameters = initialParameters;
            IsAsyncInstaller = isAsyncInstaller;
        }

        public Type InstallerType { get; }

        public IInstallFilter InstallFilter { get; }

        public object[]? InitialParameters { get; }

        public bool IsAsyncInstaller { get; }
    }
}