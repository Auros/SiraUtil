using HarmonyLib;
using SiraUtil.Zenject.Internal;
using System;
using System.Collections.Generic;
using Zenject;

namespace SiraUtil.Zenject.Harmony
{
    [HarmonyPatch(typeof(Context))]
    [HarmonyPatch("InstallInstallers")]
    [HarmonyPatch(new Type[] { typeof(List<InstallerBase>), typeof(List<Type>), typeof(List<ScriptableObjectInstaller>), typeof(List<MonoInstaller>), typeof(List<MonoInstaller>) })]
    internal class ContextDecorator
    {
        // This set is used to catch any late installing decorators.
        private static readonly HashSet<Context> _recentlyInstalledDecorators = new();
        internal static Action<Context, IEnumerable<ContextBinding>>? ContextInstalling;

        internal static void Prefix(ref Context __instance, ref List<InstallerBase> normalInstallers, ref List<Type> normalInstallerTypes, ref List<ScriptableObjectInstaller> scriptableObjectInstallers, ref List<MonoInstaller> installers, ref List<MonoInstaller> installerPrefabs)
        {
            // Check if this is a late bound decorator installation.
            if (_recentlyInstalledDecorators.Contains(__instance))
            {
                _recentlyInstalledDecorators.Remove(__instance);
                return;
            }

            ZenjectInstallationAccessor accessor = new(ref normalInstallers, ref normalInstallerTypes, ref installers);

            // Adds every installer that's being installed to the type registrator.
            HashSet<ContextBinding> bindings = new();
            foreach (var normalInstaller in normalInstallers)
                bindings.Add(new ContextBinding(__instance, normalInstaller.GetType(), accessor));
            foreach (var normalInstallerType in normalInstallerTypes)
                bindings.Add(new ContextBinding(__instance, normalInstallerType, accessor));
            foreach (var scriptableObjectInstaller in scriptableObjectInstallers)
                bindings.Add(new ContextBinding(__instance, scriptableObjectInstaller.GetType(), accessor));
            foreach (var installer in installers)
                bindings.Add(new ContextBinding(__instance, installer.GetType(), accessor));
            foreach (var installerPrefab in installerPrefabs)
                bindings.Add(new ContextBinding(__instance, installerPrefab.GetType(), accessor));

            if (__instance is SceneDecoratorContext decorator)
                _recentlyInstalledDecorators.Add(decorator);

            ContextInstalling?.Invoke(__instance, bindings);
        }
    }
}