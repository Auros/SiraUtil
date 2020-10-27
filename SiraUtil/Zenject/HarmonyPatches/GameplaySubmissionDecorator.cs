using Zenject;
using HarmonyLib;
using SiraUtil.Services;

namespace SiraUtil.Zenject.HarmonyPatches
{
    [HarmonyPatch(typeof(GameplayCoreInstaller), "InstallBindings")]
    internal class GameplaySubmissionDecorator
    {
        internal static void Prefix(GameplayCoreInstaller __instance)
        {
            var mib = __instance as MonoInstallerBase;
            var Container = Accessors.GetDiContainer(ref mib);
            Container.BindInterfacesAndSelfTo<Submission>().AsSingle();
        } 
    }
}