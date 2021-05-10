using System;
using Zenject;

namespace SiraUtil.Zenject.Internal
{
    internal struct InstallInstruction
    {
        public readonly Type baseInstaller;
        public readonly Action<DiContainer> onInstall;

        public InstallInstruction(Type baseInstaller, Action<DiContainer> onInstall)
        {
            this.onInstall = onInstall;
            this.baseInstaller = baseInstaller;
        }
    }
}