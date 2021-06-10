using HarmonyLib;
using System;
using System.Linq;
using UnityEngine;

namespace SiraUtil.Tweaks
{
    internal class LocalizerUncomplainer
    {
        [HarmonyPatch(typeof(Debug), "LogWarning", argumentTypes: new Type[] { typeof(object) })]
        internal static class RemoveLocalizationWarningLog
        {
            internal static bool Prefix(ref object message)
            {
                return !message.ToString().StartsWith("Could not find key") || Environment.GetCommandLineArgs().Contains("--siralog");
            }
        }
    }
}