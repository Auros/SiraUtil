using HarmonyLib;

namespace SiraUtil.Zenject.HarmonyPatches
{
    [HarmonyPatch(typeof(MenuInstaller), "InstallBindings")]
    internal class Menu_Installer
    {
        internal static void Postfix(ref MenuInstaller __instance)
        {
            Installer.InstallFromBase(__instance, Installer.menuInstallers, Installer.menuSiraInstallers);
        }
    }
}