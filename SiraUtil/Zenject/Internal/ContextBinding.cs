using ModestTree;
using System;
using Zenject;

namespace SiraUtil.Zenject.Internal
{
    // We are not supporting adding MonoInstaller Prefabs or ScriptableObject installers since realistically
    // they don't make sense in the context of modding. They are primarily used for the editor.
    internal struct ContextBinding
    {
        public readonly Context context;
        public readonly Type installerType;
        private readonly ZenjectInstallationAccessor accessor;

        public ContextBinding(Context context, Type installerType, ZenjectInstallationAccessor accessor)
        {
            this.context = context;
            this.accessor = accessor;
            this.installerType = installerType;
        }

        public void AddInstaller(InstallerBase installerBase)
            => accessor.normalInstallers.Add(installerBase);

        public void AddInstaller(MonoInstaller monoInstaller)
            => accessor.installers.Add(monoInstaller);

        public void AddInstaller(Type type)
        {
            if (type.DerivesFrom<InstallerBase>())
               accessor.normalInstallerTypes.Add(type);
        }
    }
}