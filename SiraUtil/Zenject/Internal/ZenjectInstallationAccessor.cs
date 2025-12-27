using System;
using System.Collections.Generic;
using Zenject;

namespace SiraUtil.Zenject.Internal
{
    // This is used to store strong references to the lists used to install zenject installers. This means we don't have to
    // directly access the Context variables for updating the install list, thus centralizing the injection point.
    internal class ZenjectInstallationAccessor
    {
        public readonly List<MonoInstaller> installers;
        public readonly List<InstallerBase> normalInstallers;
        public readonly List<Type> normalInstallerTypes;

        public ZenjectInstallationAccessor(List<InstallerBase> normalInstallers, List<Type> normalInstallerTypes, List<MonoInstaller> installers)
        {
            this.installers = installers;
            this.normalInstallers = normalInstallers;
            this.normalInstallerTypes = normalInstallerTypes;
        }
    }
}