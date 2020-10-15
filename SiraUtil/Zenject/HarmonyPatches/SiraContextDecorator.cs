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
		internal static string LastTransitionSetupName { get; set; }
		internal static string LastGamemodeSetupName { get; set; }

        internal static void Prefix(ref SceneContext __instance, ref List<string> ____contractNames,
            ref List<MonoInstaller> ____installerPrefabs, ref List<MonoInstaller> ____monoInstallers,
            ref List<Installer> ____normalInstallers, ref List<Type> ____normalInstallerTypes,
            ref List<ScriptableObjectInstaller> ____scriptableObjectInstallers, ref List<SceneDecoratorContext> ____decoratorContexts)
        {
            var sourceNames =
                ____contractNames.Concat(
                ____installerPrefabs.Select(a => a.GetType().FullName).Concat(
                ____monoInstallers.Select(b => b.GetType().FullName).Concat(
                ____normalInstallers.Select(c => c.GetType().FullName).Concat(
                ____normalInstallerTypes.Select(d => d.FullName).Concat(
                ____scriptableObjectInstallers.Select(e => e.GetType().FullName).Concat(
				____decoratorContexts.Select(f => f.gameObject.scene.name)))))));
            for (int i = 0; i < sourceNames.Count(); i++)
            {
                SiraEvents.SendInstallEvent(sourceNames.ElementAt(i), __instance, __instance.Container, ____decoratorContexts, LastGamemodeSetupName, LastTransitionSetupName ?? "");
            }
        }
    }
}