using System;
using Zenject;
using Polyglot;
using HarmonyLib;
using UnityEngine;
using System.Linq;
using SiraUtil.Interfaces;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace SiraUtil
{
    [HarmonyPatch(typeof(LocalizationImporter), "ImportTextFile")]
    internal static class RemoveLocalizationLog
    {
        private static readonly List<OpCode> _logOpCodes = new List<OpCode>()
        {
             OpCodes.Ldstr,
             OpCodes.Ldloc_S,
             OpCodes.Ldstr,
             OpCodes.Call,
             OpCodes.Call
        };

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();

            for (int i = 0; i < codes.Count; i++)
            {
                if (Utilities.OpCodeSequence(codes, _logOpCodes, i))
                {
                    codes.RemoveRange(i, _logOpCodes.Count);
                    break;
                }
            }

            return codes.AsEnumerable();
        }
    }

    [HarmonyPatch(typeof(Debug), "LogWarning", argumentTypes: new Type[] { typeof(object) })]
    internal static class RemoveLocalizationWarningLog
    {
        internal static bool Prefix(ref object message)
        {
            return !message.ToString().StartsWith("Could not find key") || Environment.GetCommandLineArgs().Contains("--siralog");
        }
    }

    public class Localizer
    {
        private readonly ILocalizer _localizer;

        public Localizer([InjectOptional(Id = "SIRA.Localizer")] ILocalizer localizer)
        {
            _localizer = localizer;
        }

        public void AddLocalizationSheet(LocalizationAsset localizationAsset)
        {
            _localizer?.AddLocalizationSheet(localizationAsset);
        }

        public void RemoveLocalizationSheet(LocalizationAsset localizationAsset)
        {
            _localizer?.RemoveLocalizationSheet(localizationAsset);
        }

        public void RemoveLocalizationSheet(string key)
        {
            _localizer?.RemoveLocalizationSheet(key);
        }

        public LocalizationAsset AddLocalizationSheet(string localizationAsset, GoogleDriveDownloadFormat type, string id, bool shadow = false)
        {
            return _localizer?.AddLocalizationSheet(localizationAsset, type, id, shadow);
        }

        public LocalizationAsset AddLocalizationSheetFromAssembly(string assemblyPath, GoogleDriveDownloadFormat type, bool shadow = false)
        {
            return _localizer?.AddLocalizationSheetFromAssembly(assemblyPath, type, shadow);
        }
    }
}