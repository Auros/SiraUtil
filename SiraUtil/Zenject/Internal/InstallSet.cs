using BGLib.AppFlow.Initialization;
using SiraUtil.Zenject.Internal.Filters;
using System;

namespace SiraUtil.Zenject.Internal
{
    internal record InstallSet
    {
        public InstallSet(Type installerType, IInstallFilter installFilter, object[]? initialParameters = null)
        {
            InstallerType = installerType;
            IsAsyncInstaller = installerType.IsSubclassOf(typeof(AsyncInstaller)) && (initialParameters is null || initialParameters.Length == 0); // this is for backwards compatibility - future version should throw if parameters are given for an async installer
            InstallFilter = installFilter;
            InitialParameters = initialParameters;
        }

        public Type InstallerType { get; }

        public bool IsAsyncInstaller { get; }

        public IInstallFilter InstallFilter { get; }

        public object[]? InitialParameters { get; }
    }
}