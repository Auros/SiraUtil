using System;
using Zenject;
using HarmonyLib;
using System.Linq;
using SiraUtil.Events;
using System.Collections.Generic;

namespace SiraUtil.Zenject.HarmonyPatches
{
    [HarmonyPatch(typeof(SceneContext), "InstallBindings")]
    internal class SiraContextDecorator
    {
        internal static void Prefix(ref SceneContext __instance, ref List<string> ____contractNames,
            ref List<MonoInstaller> ____installerPrefabs, ref List<MonoInstaller> ____monoInstallers,
            ref List<global::Zenject.Installer> ____normalInstallers, ref List<Type> ____normalInstallerTypes,
            ref List<ScriptableObjectInstaller> ____scriptableObjectInstallers)
        {
            var sourceNames =
                ____contractNames.Concat(
                ____installerPrefabs.Select(a => a.GetType().Name).Concat(
                ____monoInstallers.Select(b => b.GetType().Name).Concat(
                ____normalInstallers.Select(c => c.GetType().Name).Concat(
                ____normalInstallerTypes.Select(d => d.Name).Concat(
                ____scriptableObjectInstallers.Select(e => e.GetType().Name))))));

            for (int i = 0; i < sourceNames.Count(); i++)
            {
                SiraEvents.SendInstallEvent(sourceNames.ElementAt(i), __instance, __instance.Container);
            }
        }
    }
}