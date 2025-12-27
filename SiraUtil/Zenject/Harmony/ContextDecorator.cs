using BGLib.AppFlow.Initialization;
using HarmonyLib;
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
        internal static Action<Context, IEnumerable<Type>>? ContextInstalling;

        internal static void Prefix(Context __instance, List<InstallerBase> normalInstallers, List<Type> normalInstallerTypes, List<ScriptableObjectInstaller> scriptableObjectInstallers, List<MonoInstaller> installers, List<MonoInstaller> installerPrefabs)
        {
            // Check if this is a late bound decorator installation.
            if (_recentlyInstalledDecorators.Contains(__instance))
            {
                _recentlyInstalledDecorators.Remove(__instance);
                return;
            }

            // Adds every installer that's being installed to the type registrator.
            HashSet<Type> installerBindings = new();
            foreach (var normalInstaller in normalInstallers)
                installerBindings.Add(normalInstaller.GetType());
            foreach (var normalInstallerType in normalInstallerTypes)
                installerBindings.Add(normalInstallerType);
            foreach (var scriptableObjectInstaller in scriptableObjectInstallers)
                installerBindings.Add(scriptableObjectInstaller.GetType());
            foreach (var installer in installers)
                installerBindings.Add(installer.GetType());
            foreach (var installerPrefab in installerPrefabs)
                installerBindings.Add(installerPrefab.GetType());

            if (__instance is AsyncSceneContext asyncSceneContext)
                foreach (var asyncInstaller in asyncSceneContext._asyncInstallers)
                    installerBindings.Add(asyncInstaller.GetType());

            if (__instance is SceneDecoratorContext decorator)
                _recentlyInstalledDecorators.Add(decorator);

            ContextInstalling?.Invoke(__instance, installerBindings);
        }
    }
}