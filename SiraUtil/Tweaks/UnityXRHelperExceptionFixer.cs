using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.XR;

namespace SiraUtil.Tweaks
{
    [HarmonyPatch(typeof(UnityXRHelper), nameof(UnityXRHelper.Start))]
    internal static class UnityXRHelperExceptionFixer
    {
        private static readonly MethodInfo XRInputSubsystemListIndexer = AccessTools.DeclaredMethod(typeof(List<XRInputSubsystem>), "get_Item");
        private static readonly MethodInfo XRInputSubsystemListCountGetter = AccessTools.DeclaredPropertyGetter(typeof(List<XRInputSubsystem>), nameof(List<XRInputSubsystem>.Count));
        private static readonly MethodInfo AddTrackingOriginUpdatedMethod = typeof(XRInputSubsystem).GetEvent(nameof(XRInputSubsystem.trackingOriginUpdated)).GetAddMethod();
        private static readonly MethodInfo SubsystemManagerGetInstancesMethod = AccessTools.DeclaredMethod(typeof(SubsystemManager), nameof(SubsystemManager.GetInstances), generics: new Type[] { typeof(XRInputSubsystem) });

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilGenerator)
        {
            // Create a new local so we can store the XRInputSubsystem
            LocalBuilder localBuilder = ilGenerator.DeclareLocal(typeof(XRInputSubsystem));

            return new CodeMatcher(instructions, ilGenerator)
                // Create a label after `xrInputSubsystem.trackingOriginUpdated += OnTrackingOriginUpdated;` (last use of xrInputSubsystem)
                .MatchForward(
                    true,
                    new CodeMatch(i => i.Calls(AddTrackingOriginUpdatedMethod)))
                .ThrowIfInvalid("`trackingOriginUpdated +=` not found")
                .CreateLabelWithOffsets(1, out Label doneUsingInputSystemLabel)
                // Backtrack to right after `new List<XRInputSubsystem>()`
                .MatchBack(
                    false,
                    new CodeMatch(OpCodes.Dup),
                    new CodeMatch(i => i.Calls(SubsystemManagerGetInstancesMethod)))
                .ThrowIfInvalid("`SubsystemManager.GetInstances` not found")
                // Replace `dup` with storing to & loading from our local variable
                .SetAndAdvance(OpCodes.Stloc, localBuilder)
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc, localBuilder))
                // Move to after `SubsystemManager.GetInstances` call, right before `XRInputSubsystem xrInputSubsystem = list[0]`
                .Advance(1)
                // Insert the branching logic `if (list.Count > 0) { ... }`
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldloc, localBuilder),
                    new CodeInstruction(OpCodes.Callvirt, XRInputSubsystemListCountGetter),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Ble, doneUsingInputSystemLabel),
                    new CodeInstruction(OpCodes.Ldloc, localBuilder)) // This last ldloc be used by `XRInputSubsystem xrInputSubsystem = list[0]`
                .InstructionEnumeration();
        }
    }
}
