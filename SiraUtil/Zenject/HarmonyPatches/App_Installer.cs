using HarmonyLib;

namespace SiraUtil.Zenject.HarmonyPatches
{
    [HarmonyPatch(typeof(AppCoreInstaller), "InstallBindings")]
    internal class App_Installer
    {
        internal static void Postfix(ref AppCoreInstaller __instance)
        {
            Installer.InstallFromBase(__instance, Installer.appInstallers);
        }
    }
}