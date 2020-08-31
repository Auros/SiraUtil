using SiraUtil.Zenject;
using System.Reflection;
using System.Collections.Generic;

namespace SiraUtil.Sabers
{
    public static class ExtraSabers
    {
        private static readonly HashSet<Assembly> _registeredAssemblies = new HashSet<Assembly>();

        /// <summary>
        /// Registers your assembly for using ExtraSabers. This enables the ability to request the SiraSaber.Factory in Zenject.
        /// </summary>
        public static void Touch()
        {
            _registeredAssemblies.Add(Assembly.GetCallingAssembly());
        
            if (_registeredAssemblies.Count == 1)
            {
                Installer.RegisterGameCoreInstaller<SiraSaberInstaller>();
                BurnPatches.Patch(Plugin.Harmony);
            }
        }

        /// <summary>
        /// Unregisters your assembly for using ExtraSabers. Make sure to do this when you disable your mod.
        /// </summary>
        public static void Untouch()
        {
            _registeredAssemblies.Remove(Assembly.GetCallingAssembly());

            if (_registeredAssemblies.Count == 0)
            {
                Installer.UnregisterGameCoreInstaller<SiraSaberInstaller>();
                try
                {
                    BurnPatches.Unpatch(Plugin.Harmony);
                }
                catch { }
            }
        }
    }
}