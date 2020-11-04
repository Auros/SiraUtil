using Zenject;
using HarmonyLib;
using IPA.Utilities;
using SiraUtil.Services;

namespace SiraUtil.Zenject.HarmonyPatches
{
    [HarmonyPatch(typeof(GameplayCoreInstaller), "InstallBindings")]
    internal class GameplaySubmissionDecorator
    {
        internal static void Prefix(GameplayCoreInstaller __instance, ref GameplayCoreSceneSetupData ____sceneSetupData)
        {
            var mib = __instance as MonoInstallerBase;
            var Container = Accessors.GetDiContainer(ref mib);
            Container.BindInterfacesAndSelfTo<Submission>().AsSingle();
        } 
    }

    [HarmonyPatch(typeof(SinglePlayerLevelSelectionFlowCoordinator), "HandleStandardLevelDidFinish")]
    internal class ConvertPractice
    {
        internal static void Prefix(ref StandardLevelScenesTransitionSetupDataSO standardLevelScenesTransitionSetupData, out PracticeSettings __state)
        {
            var setupData = standardLevelScenesTransitionSetupData.Get<GameplayCoreSceneSetupData>();
            if (setupData.practiceSettings is Submission.SiraPracticeSettings siraPracticeSettings)
            {
                __state = siraPracticeSettings.normalPracticeSettings;
                return;
            }
            __state = setupData.practiceSettings;
        }

        internal static void Postfix(ref StandardLevelScenesTransitionSetupDataSO standardLevelScenesTransitionSetupData, PracticeSettings __state)
        {
            standardLevelScenesTransitionSetupData.Get<GameplayCoreSceneSetupData>().SetField("practiceSettings", __state);
        }
    }
}