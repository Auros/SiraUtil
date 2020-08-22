using HarmonyLib;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace SiraUtil.Zenject.HarmonyPatches
{
    [HarmonyPatch(typeof(AppCoreInstaller), "InstallBindings")]
    internal class App_Installer
    {
        private static readonly MethodInfo _containerPatch = SymbolExtensions.GetMethodInfo(() => PatchContainer(null));

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();
            codes.InsertRange(codes.Count - 1, new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, _containerPatch)
            });
            return codes.AsEnumerable();
        }

        private static void PatchContainer(AppCoreInstaller installer)
        {
            Installer.InstallFromBase(installer, Installer.appInstallers, Installer.appSiraInstallers);
        }
    }
}