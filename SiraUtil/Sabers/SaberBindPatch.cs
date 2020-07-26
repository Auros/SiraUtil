using Xft;
using Zenject;
using HarmonyLib;
using System.Linq;
using IPA.Utilities;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SiraUtil.Sabers
{
    internal class SaberBindPatch
    {
        private static MethodInfo _original = typeof(GameCoreSceneSetup).GetMethod("InstallBindings");
        private static MethodInfo _transpile = typeof(GameCorePatch).GetMethod("Transpiler");

        internal static void Patch(Harmony harmony)
        {
            harmony.Patch(_original, null, null, new HarmonyMethod(_transpile));
        }

        internal static void Unpatch(Harmony harmony)
        {
            harmony.Unpatch(_original, _transpile);
        }

        [HarmonyPatch(typeof(GameCoreSceneSetup), "InstallBindings")]
        internal class GameCorePatch
        {
            private static List<OpCode> _basicModelBind = new List<OpCode>
            {
                OpCodes.Ldarg_0,
                OpCodes.Call,
                OpCodes.Callvirt,
                OpCodes.Ldarg_0,
                OpCodes.Ldfld,
                OpCodes.Callvirt,
                OpCodes.Callvirt,
                OpCodes.Pop
            };

            private static readonly MethodInfo _conditionalBind = SymbolExtensions.GetMethodInfo(() => ConditionalModelControllerBind(null));

            public static void Prefix(ref GameCoreSceneSetup __instance, ref BasicSaberModelController ____basicSaberModelControllerPrefab)
            {
                // Borrow the XWeaponTrail and expose it to Zenject ;)

                var trail = ____basicSaberModelControllerPrefab.GetField<XWeaponTrail, BasicSaberModelController>("_saberWeaponTrail");
                var container = GetContainer(__instance);

                container.Bind<XWeaponTrail>().FromComponentOn(trail.gameObject).AsTransient();

                Plugin.Log.Info("Binded weapon trail to zenject as transient");
            }

            public static void Postfix(ref GameCoreSceneSetup __instance)
            {
                var container = GetContainer(__instance);

                var topProvider = SaberModelProvider.providers.OrderByDescending(q => q.Priority).FirstOrDefault();
                if (topProvider != null)
                {
                    container.Bind<ISaberModelController>().FromComponentInNewPrefab(topProvider.ModelController).AsTransient();
                }
                Plugin.Log.Info("ran postfix");
            }

            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                // Remove the BasicSaberModelController bind
                List<CodeInstruction> codes = instructions.ToList();
                for (int i = 0; i < codes.Count(); i++)
                {
                    if (Utilities.OpCodeSequence(codes, _basicModelBind, i))
                    {
                        for (int f = _basicModelBind.Count - 2; f > 0; f--)
                        {
                            codes.RemoveAt(i + f);
                        }
                        codes.InsertRange(i + 2, new List<CodeInstruction>
                        {
                            new CodeInstruction(OpCodes.Ldarg_0),
                            new CodeInstruction(OpCodes.Call, _conditionalBind)
                        });
                        break;
                    }
                }
                return codes.AsEnumerable();
            }

            private static void ConditionalModelControllerBind(GameCoreSceneSetup gameCoreSceneSetup)
            {
                Plugin.Log.Info(SaberModelProvider.providers.Count == 0 ? "Rebinding model controller as default" : "Using custom model controller");
                if (SaberModelProvider.providers.Count == 0)
                {
                    var modelController = gameCoreSceneSetup.GetField<BasicSaberModelController, GameCoreSceneSetup>("_basicSaberModelControllerPrefab");
                    if (modelController != null)
                    {
                        var container = GetContainer(gameCoreSceneSetup);
                        container.Bind<ISaberModelController>().FromComponentInNewPrefab(modelController).AsTransient();
                    }
                }
            }

            private static DiContainer GetContainer(GameCoreSceneSetup gameCoreSceneSetup)
            {
                MonoInstallerBase monoInstaller = gameCoreSceneSetup;
                DiContainer container = Zenject.Installer.AccessDiContainer(ref monoInstaller);
                return container;
            }
        }
    }
}
