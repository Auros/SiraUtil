using SiraUtil.Zenject.Internal.Filters;
using System;

namespace SiraUtil.Zenject.Internal
{
    internal readonly struct InstallSet
    {
        public readonly Type installerType;
        public readonly IInstallFilter installFilter;

        public readonly object[]? initialParameters;

        public InstallSet(Type installerType, IInstallFilter installFilter, object[]? initialParameters = null)
        {
            this.installerType = installerType;
            this.installFilter = installFilter;
            this.initialParameters = initialParameters;
        }
    }
}