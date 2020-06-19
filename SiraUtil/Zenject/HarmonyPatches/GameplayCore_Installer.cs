using HarmonyLib;

namespace SiraUtil.Zenject.HarmonyPatches
{
    [HarmonyPatch(typeof(GameplayCoreSceneSetup), "InstallBindings")]
    internal class GameplayCore_Installer
    {
        internal static void Postfix(ref MenuInstaller __instance)
        {
            Installer.InstallFromBase(__instance, Installer.gameplayCoreSceneSetupInstallers);
        }
    }
}