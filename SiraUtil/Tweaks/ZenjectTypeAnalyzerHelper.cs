using HarmonyLib;
using System;
using Zenject;

namespace SiraUtil.Tweaks
{
    [HarmonyPatch]
    internal class ZenjectTypeAnalyzerHelper
    {
        [HarmonyPatch(typeof(TypeAnalyzer), nameof(TypeAnalyzer.TryGetInfo), [typeof(Type)])]
        [HarmonyFinalizer]
        private static Exception? TypeAnalyzer_TryGetInfo(Type type, Exception __exception)
        {
            return __exception != null
                ? new TypeAnalyzerException($"Failed to get type info for type {type.FullDescription()}", __exception)
                : __exception;
        }
    }

    /// <summary>
    /// Exception thrown when an error occurs within <see cref="TypeAnalyzer"/>.
    /// </summary>
    public class TypeAnalyzerException : Exception
    {
        internal TypeAnalyzerException(string message, Exception innerException) : base(message, innerException) { }
    }
}
