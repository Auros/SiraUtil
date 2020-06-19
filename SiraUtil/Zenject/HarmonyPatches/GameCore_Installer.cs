using HarmonyLib;

namespace SiraUtil.Zenject.HarmonyPatches
{
    [HarmonyPatch(typeof(GameCoreSceneSetup), "InstallBindings")]
    internal class GameCore_Installer
    {
        internal static void Postfix(ref MenuInstaller __instance)
        {
            Installer.InstallFromBase(__instance, Installer.gameCoreSceneSetupInstallers);
        }
    }
}