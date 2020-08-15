using SiraUtil.Zenject;
using System.Reflection;
using System.Collections.Generic;

namespace SiraUtil.Sabers
{
    public static class ExtraSabers
    {
        private static readonly HashSet<Assembly> _registeredAssemblies = new HashSet<Assembly>();

        public static void Touch()
        {
            _registeredAssemblies.Add(Assembly.GetCallingAssembly());
        
            if (_registeredAssemblies.Count == 1)
            {
                Installer.RegisterGameCoreInstaller<SiraSaberInstaller>();
                BurnPatches.Patch(Plugin.Instance.Harmony);
            }
        }

        public static void Untouch()
        {
            _registeredAssemblies.Remove(Assembly.GetCallingAssembly());

            if (_registeredAssemblies.Count == 0)
            {
                Installer.UnregisterGameCoreInstaller<SiraSaberInstaller>();
                try
                {
                    BurnPatches.Unpatch(Plugin.Instance.Harmony);
                }
                catch { }
            }
        }
    }
}