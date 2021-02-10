using HarmonyLib;
using Zenject;

namespace SiraUtil.Objects
{
    [HarmonyPatch(typeof(EffectPoolsManualInstaller), nameof(EffectPoolsManualInstaller.ManualInstallBindings))]
    internal class EffectObjectRedecorator
    {
        private static FlyingTextEffect _staticFlyingTextEffectPrefab;

        internal static void Prefix(DiContainer container, ref FlyingTextEffect ____flyingTextEffectPrefab)
        {
            _staticFlyingTextEffectPrefab = ____flyingTextEffectPrefab;

            if (_staticFlyingTextEffectPrefab != null)
            {
                ____flyingTextEffectPrefab = _staticFlyingTextEffectPrefab;
            }

            var flyingText = BeatmapObjectRedecorator.InstallModelProviderSystem(container, ____flyingTextEffectPrefab);
            if (flyingText != null)
            {
                flyingText.gameObject.SetActive(false);
                ____flyingTextEffectPrefab.gameObject.SetActive(false);
                ____flyingTextEffectPrefab = flyingText;
            }
        }
    }
}