using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine.XR;

namespace SiraUtil.Tweaks
{
    [HarmonyPatch(typeof(SettingsApplicatorSO), nameof(SettingsApplicatorSO.ApplyGraphicSettings))]
    internal class DontCallSetTrackingSpaceType
    {
        private static readonly MethodInfo XRDeviceSetTrackingSpaceTypeMethod = AccessTools.DeclaredMethod(typeof(XRDevice), nameof(XRDevice.SetTrackingSpaceType));

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                .MatchForward(false, new CodeMatch(OpCodes.Ldc_I4_1), new CodeMatch(i => i.Calls(XRDeviceSetTrackingSpaceTypeMethod)), new CodeMatch(OpCodes.Pop))
                .ThrowIfInvalid($"Call to {nameof(XRDevice.SetTrackingSpaceType)} not found")
                .RemoveInstructions(3)
                .InstructionEnumeration();
        }
    }
}
