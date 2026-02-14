using BGLib.AppFlow.Initialization;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SiraUtil.Zenject.Harmony
{
    [HarmonyPatch(typeof(Context))]
    internal class ContextDecorator
    {
        // This set is used to catch any late installing decorators.
        private static readonly HashSet<Context> _recentlyInstalledDecorators = [];
        internal static Action<Context, IEnumerable<Type>>? ContextInstalling;
        internal static Action<Context, List<MonoBehaviour>>? InstalledSceneBindings;

        [HarmonyPatch(nameof(Context.InstallInstallers))]
        [HarmonyPatch([typeof(List<InstallerBase>), typeof(List<Type>), typeof(List<ScriptableObjectInstaller>), typeof(List<MonoInstaller>), typeof(List<MonoInstaller>)])]
        [HarmonyPrefix]
        internal static void InstallInstallers(Context __instance, List<InstallerBase> normalInstallers, List<Type> normalInstallerTypes, List<ScriptableObjectInstaller> scriptableObjectInstallers, List<MonoInstaller> installers, List<MonoInstaller> installerPrefabs)
        {
            // Check if this is a late bound decorator installation.
            if (_recentlyInstalledDecorators.Contains(__instance))
            {
                _recentlyInstalledDecorators.Remove(__instance);
                return;
            }

            // Adds every installer that's being installed to the type registrator.
            HashSet<Type> installerBindings = [];
            foreach (InstallerBase normalInstaller in normalInstallers)
            {
                installerBindings.Add(normalInstaller.GetType());
            }

            foreach (Type normalInstallerType in normalInstallerTypes)
            {
                installerBindings.Add(normalInstallerType);
            }

            foreach (ScriptableObjectInstaller scriptableObjectInstaller in scriptableObjectInstallers)
            {
                installerBindings.Add(scriptableObjectInstaller.GetType());
            }

            foreach (MonoInstaller installer in installers)
            {
                installerBindings.Add(installer.GetType());
            }

            foreach (MonoInstaller installerPrefab in installerPrefabs)
            {
                installerBindings.Add(installerPrefab.GetType());
            }

            if (__instance is AsyncSceneContext asyncSceneContext)
            {
                foreach (AsyncInstaller? asyncInstaller in asyncSceneContext._asyncInstallers)
                {
                    installerBindings.Add(asyncInstaller.GetType());
                }
            }

            if (__instance is SceneDecoratorContext decorator)
            {
                _recentlyInstalledDecorators.Add(decorator);
            }

            ContextInstalling?.Invoke(__instance, installerBindings);
        }

        [HarmonyPatch(nameof(Context.InstallSceneBindings))]
        [HarmonyPostfix]
        internal static void InstallSceneBindings(Context __instance, List<MonoBehaviour> injectableMonoBehaviours)
        {
            InstalledSceneBindings?.Invoke(__instance, injectableMonoBehaviours);
        }
    }
}