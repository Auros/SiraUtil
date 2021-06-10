using HarmonyLib;
using System;
using System.Linq;
using UnityEngine;

namespace SiraUtil.Tweaks
{
    [HarmonyPatch(typeof(Debug), "LogWarning", argumentTypes: new Type[] { typeof(object) })]
    internal static class RemoveLocalizationWarningLog
    {
        private static readonly bool _contains = Environment.GetCommandLineArgs().Contains("--siralog");

        internal static bool Prefix(ref object message)
        {
            return !message.ToString().StartsWith("Could not find key") || _contains;
        }
    }
}