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
    class InitializableUnasserter
    {
        private static readonly MethodInfo _newFail = SymbolExtensions.GetMethodInfo(() => NewErrorBehavior(null!, null!, null!));
        private static readonly MethodInfo _rootMethod = typeof(ModestTree.Assert).GetMethod(nameof(ModestTree.Assert.CreateException), new Type[] { typeof(Exception), typeof(string), typeof(object[]) });

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            for (int i = 0; i < codes.Count; i++)
            {
                var code = codes[i];

                if (code.operand != null && code.Is(OpCodes.Call, _rootMethod))
                {
                    codes[i] = new CodeInstruction(OpCodes.Callvirt, _newFail);
                    codes.RemoveAt(i + 1);
                    break;
                }
            }
            return codes.AsEnumerable();
        }

        private static void NewErrorBehavior(Exception exception, string _, object[] parameters)
        {
            var failedType = (Type)parameters[0];
            string failText = $"Error occurred while initializing {nameof(IInitializable)} with type '{failedType.FullName}'";
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