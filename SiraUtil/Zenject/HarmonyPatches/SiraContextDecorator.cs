using System;
using Zenject;
using HarmonyLib;
using System.Linq;
using IPA.Utilities;
using SiraUtil.Events;
using System.Collections.Generic;

namespace SiraUtil.Zenject.HarmonyPatches
{

    [HarmonyPatch(typeof(Context), "InstallInstallers", argumentTypes: new Type[] { })]
    internal class SiraContextDecorator
    {
        private static readonly FieldAccessor<SceneContext, List<SceneDecoratorContext>>.Accessor Decorators = FieldAccessor<SceneContext, List<SceneDecoratorContext>>.GetAccessor("_decoratorContexts");

        internal static string LastTransitionSetupName { get; set; }
        internal static string LastGamemodeSetupName { get; set; }
        internal static string LastMidSceneName { get; set; }

        internal static void Prefix(ref Context __instance,
            ref List<MonoInstaller> ____installerPrefabs, ref List<MonoInstaller> ____monoInstallers,
            ref List<Installer> ____normalInstallers, ref List<Type> ____normalInstallerTypes,
            ref List<ScriptableObjectInstaller> ____scriptableObjectInstallers)
        {
            var ____contractNames = new List<string>();
            var ____decoratorContexts = new List<SceneDecoratorContext>();
            if (__instance is SceneContext sceneContext)
            {
                ____contractNames.AddRange(sceneContext.ContractNames);
                ____decoratorContexts.AddRange(Decorators(ref sceneContext));
            }
            if (__instance is SceneDecoratorContext sceneDecoratorContext)
            {
                ____decoratorContexts.Add(sceneDecoratorContext);
            }

            var sourceNames =
                ____contractNames.Concat(
                ____installerPrefabs.Select(a => a.GetType().FullName).Concat(
                ____monoInstallers.Select(b => b.GetType().FullName).Concat(
                ____normalInstallers.Select(c => c.GetType().FullName).Concat(
                ____normalInstallerTypes.Select(d => d.FullName).Concat(
                ____scriptableObjectInstallers.Select(e => e.GetType().FullName).Concat(
                ____decoratorContexts.Select(f => f.gameObject.scene.name)))))));

            SiraEvents.SendInstallEvent(sourceNames.ToArray(), __instance, __instance.Container, ____decoratorContexts, LastGamemodeSetupName, LastTransitionSetupName, LastMidSceneName);
        }
    }
}