using HarmonyLib;
using System.Reflection;

namespace SiraUtil.Sabers
{
    internal static class BurnPatches
    {
        private static readonly MethodInfo _areaOriginalStart = typeof(SaberBurnMarkArea).GetMethod("Start"); 
        private static readonly MethodInfo _areaOriginalOnDestroy = typeof(SaberBurnMarkArea).GetMethod("OnDestroy");
        private static readonly MethodInfo _areaOriginalLateUpdate = typeof(SaberBurnMarkArea).GetMethod("LateUpdate");
        private static readonly MethodInfo _burnAreaPostfixStart = typeof(BurnAreaPatch).GetMethod("PostfixStart");
        private static readonly MethodInfo _burnAreaPrefixOnDestroy = typeof(BurnAreaPatch).GetMethod("PrefixCancelMethod");
        private static readonly MethodInfo _burnAreaPrefixLateUpdate = typeof(BurnAreaPatch).GetMethod("PrefixCancelMethod");

        private static readonly MethodInfo _sparkOriginalStart = typeof(SaberBurnMarkSparkles).GetMethod("Start");
        private static readonly MethodInfo _sparkOriginalOnDestroy = typeof(SaberBurnMarkSparkles).GetMethod("OnDestroy");
        private static readonly MethodInfo _sparkOriginalLateUpdate = typeof(SaberBurnMarkSparkles).GetMethod("LateUpdate");
        private static readonly MethodInfo _burnClashPostfixStart = typeof(BurnSparkPatch).GetMethod("PostfixStart");
        private static readonly MethodInfo _burnClashPrefixOnDestroy = typeof(BurnSparkPatch).GetMethod("PrefixCancelMethod");
        private static readonly MethodInfo _burnClashPrefixLateUpdate = typeof(BurnSparkPatch).GetMethod("PrefixCancelMethod");

        private static readonly MethodInfo _clashCheckOriginalStart = typeof(SaberClashChecker).GetMethod("Start");
        private static readonly MethodInfo _clashCheckOriginalUpdate = typeof(SaberClashChecker).GetMethod("Update");
        private static readonly MethodInfo _clashCheckPostfixStart = typeof(ClashPatch).GetMethod("PostfixStart");
        private static readonly MethodInfo _clashCheckPrefixUpdate = typeof(ClashPatch).GetMethod("PrefixCancelMethod");

        private static readonly MethodInfo _obstacleOriginalStart = typeof(ObstacleSaberSparkleEffectManager).GetMethod("Start");
        private static readonly MethodInfo _obstacleOriginalOnDisable = typeof(ObstacleSaberSparkleEffectManager).GetMethod("OnDisable");
        private static readonly MethodInfo _obstacleOriginalUpdate = typeof(ObstacleSaberSparkleEffectManager).GetMethod("Update");
        private static readonly MethodInfo _obstaclePostfixStart = typeof(ObstaclePatch).GetMethod("PostfixStart");
        private static readonly MethodInfo _obstaclePrefixOnDisable = typeof(ObstaclePatch).GetMethod("PrefixCancelMethod");
        private static readonly MethodInfo _obstaclePrefixUpdate = typeof(ObstaclePatch).GetMethod("PrefixCancelMethod");

        internal static void Patch(Harmony harmony)
        {
            harmony.Patch(_areaOriginalStart, null, new HarmonyMethod(_burnAreaPostfixStart));
            harmony.Patch(_areaOriginalOnDestroy, new HarmonyMethod(_burnAreaPrefixOnDestroy));
            harmony.Patch(_areaOriginalLateUpdate, new HarmonyMethod(_burnAreaPrefixLateUpdate));

            harmony.Patch(_sparkOriginalStart, null, new HarmonyMethod(_burnClashPostfixStart));
            harmony.Patch(_sparkOriginalOnDestroy, new HarmonyMethod(_burnClashPrefixOnDestroy));
            harmony.Patch(_sparkOriginalLateUpdate, new HarmonyMethod(_burnClashPrefixLateUpdate));

            harmony.Patch(_clashCheckOriginalStart, null, new HarmonyMethod(_clashCheckPostfixStart));
            harmony.Patch(_clashCheckOriginalUpdate, new HarmonyMethod(_clashCheckPrefixUpdate));

            harmony.Patch(_obstacleOriginalStart, null, new HarmonyMethod(_obstaclePostfixStart));
            harmony.Patch(_obstacleOriginalOnDisable, new HarmonyMethod(_obstaclePrefixOnDisable));
            harmony.Patch(_obstacleOriginalUpdate, new HarmonyMethod(_obstaclePrefixUpdate));
        }

        internal static void Unpatch(Harmony harmony)
        {
            harmony.Unpatch(_areaOriginalStart, _burnAreaPostfixStart);
            harmony.Unpatch(_areaOriginalOnDestroy, _burnAreaPrefixOnDestroy);
            harmony.Unpatch(_areaOriginalLateUpdate, _burnAreaPrefixLateUpdate);

            harmony.Unpatch(_sparkOriginalStart, _burnClashPostfixStart);
            harmony.Unpatch(_sparkOriginalOnDestroy, _burnClashPrefixOnDestroy);
            harmony.Unpatch(_sparkOriginalLateUpdate, _burnClashPrefixLateUpdate);

            harmony.Unpatch(_clashCheckOriginalStart, _clashCheckPostfixStart);
            harmony.Unpatch(_clashCheckOriginalUpdate, _clashCheckPrefixUpdate);

            harmony.Unpatch(_obstacleOriginalStart, _obstaclePostfixStart);
            harmony.Unpatch(_obstacleOriginalOnDisable, _obstaclePrefixOnDisable);
            harmony.Unpatch(_obstacleOriginalUpdate, _obstaclePrefixUpdate);
        }

        internal class BurnAreaPatch
        {
            public static void PostfixStart(ref SaberBurnMarkArea __instance)
            {
                if (!(__instance is SiraSaberBurnMarkArea))
                {
                    __instance.gameObject.AddComponent<SiraSaberBurnMarkArea>();
                }
            }

            public static bool PrefixCancelMethod(ref SaberBurnMarkArea __instance)
            {
				return __instance is SiraSaberBurnMarkArea;
			}
		}

        internal class BurnSparkPatch
        {
            public static void PostfixStart(ref SaberBurnMarkSparkles __instance)
            {
                if (!(__instance is SiraSaberBurnMarkSparkles))
                {
                    __instance.gameObject.AddComponent<SiraSaberBurnMarkSparkles>();
                }
            }

            public static bool PrefixCancelMethod(ref SaberBurnMarkSparkles __instance)
            {
				return __instance is SiraSaberBurnMarkSparkles;
			}
		}

        internal class ClashPatch
        {
            public static void PostfixStart(ref SaberClashChecker __instance)
            {
                if (!(__instance is SiraSaberClashChecker))
                {
                    __instance.gameObject.AddComponent<SiraSaberClashChecker>();
                }
            }

            public static bool PrefixCancelMethod(ref SaberClashChecker __instance)
            {
				return __instance is SiraSaberClashChecker;
			}
		}

        internal class ObstaclePatch
        {
            public static void PostfixStart(ref ObstacleSaberSparkleEffectManager __instance)
            {
                if (!(__instance is SiraObstacleSaberSparkleEffectManager))
                {
                    __instance.gameObject.AddComponent<SiraObstacleSaberSparkleEffectManager>();
                }
            }

            public static bool PrefixCancelMethod(ref ObstacleSaberSparkleEffectManager __instance)
            {
				return __instance is SiraObstacleSaberSparkleEffectManager;
			}
		}
    }
}