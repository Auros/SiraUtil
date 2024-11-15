using HarmonyLib;
using IPA.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Zenject;

namespace SiraUtil.Tweaks
{
    [HarmonyPatch]
    class ZenjectLifecycleUnasserter
    {
        private static readonly MethodInfo _newFail = SymbolExtensions.GetMethodInfo(() => NewErrorBehavior(null!, null!, null!));
        private static readonly MethodInfo _rootMethod = typeof(ModestTree.Assert).GetMethod(nameof(ModestTree.Assert.CreateException), new Type[] { typeof(Exception), typeof(string), typeof(object[]) });

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            return new CodeMatcher(instructions, generator)
                .MatchForward(false, new CodeMatch(OpCodes.Call, _rootMethod), new CodeMatch(OpCodes.Throw))
                .ThrowIfInvalid($"Call to {nameof(ModestTree.Assert.CreateException)} & throw not found")
                .RemoveInstruction()
                .SetAndAdvance(OpCodes.Call, _newFail)
                .InstructionEnumeration();
        }

        public static IEnumerable<MethodInfo> TargetMethods()
        {
            yield return AccessTools.Method(typeof(InitializableManager), nameof(InitializableManager.Initialize));
            yield return AccessTools.Method(typeof(DisposableManager), nameof(DisposableManager.Dispose));
            yield return AccessTools.Method(typeof(DisposableManager), nameof(DisposableManager.LateDispose));
        }

        private static void NewErrorBehavior(Exception exception, string message, params object[] parameters)
        {
            var failedType = (Type)parameters[0];
            string failText = string.Format(message, failedType.FullName);
            if (failedType.Name != failedType.FullName)
            {
                Plugin.Log.Critical(failText);
                Plugin.Log.Critical(exception);

                var asm = failedType.Assembly;
                var plugin = PluginManager.EnabledPlugins.FirstOrDefault(pl => pl.Assembly == asm);

                if (plugin != null)
                {
                    string addressTo = string.IsNullOrEmpty(plugin.Author) ? $"The author of {plugin.Name}" : plugin.Author;
                    for (int i = 0; i < 10; i++)
                        Plugin.Log.Warn($"Please tell {addressTo} to fix this!");
                }
            }
            else
            {
                UnityEngine.Debug.LogError(failText);
                UnityEngine.Debug.LogError(exception);
            }
        }
    }
}