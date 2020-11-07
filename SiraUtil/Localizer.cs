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

    /// <summary>
    /// A wrapper around the optional localizer mod called SiraLocalizer. Nothing will actually localize if the mod isn't installed. This class only exists to avoid breaking any API changes.
    /// </summary>
    public class Localizer
    {
        private readonly ILocalizer _localizer;

        internal Localizer([InjectOptional(Id = "SIRA.Localizer")] ILocalizer localizer)
        {
            _localizer = localizer;
        }

        /// <summary>
        /// Adds a localization sheet.
        /// </summary>
        /// <param name="localizationAsset"></param>
        public void AddLocalizationSheet(LocalizationAsset localizationAsset)
        {
            _localizer?.AddLocalizationSheet(localizationAsset);
        }

        /// <summary>
        /// Removes a localization sheet.
        /// </summary>
        /// <param name="localizationAsset"></param>
        public void RemoveLocalizationSheet(LocalizationAsset localizationAsset)
        {
            _localizer?.RemoveLocalizationSheet(localizationAsset);
        }

        /// <summary>
        /// Removes a localization sheet by its key.
        /// </summary>
        /// <param name="key"></param>
        public void RemoveLocalizationSheet(string key)
        {
            _localizer?.RemoveLocalizationSheet(key);
        }

        /// <summary>
        /// Adds a localization sheet by text.
        /// </summary>
        /// <param name="localizationAsset">The stringy asset of the sheet.</param>
        /// <param name="type">The type of the asset.</param>
        /// <param name="id">The ID of the asset.</param>
        /// <param name="shadow">Is it a shadow localization? This means it will not appear unless another source NOT marked as a shadow localization uses a language present. This is to avoid a mod implementing a lot of languages that aren't used anywhere else and them appearing on the list.</param>
        /// <returns></returns>
        public LocalizationAsset AddLocalizationSheet(string localizationAsset, GoogleDriveDownloadFormat type, string id, bool shadow = false)
        {
            return _localizer?.AddLocalizationSheet(localizationAsset, type, id, shadow);
        }

        /// <summary>
        /// Adds a localization sheet from an assembly rsource.
        /// </summary>
        /// <param name="assemblyPath">The path to the resource.</param>
        /// <param name="type">The type of the asset.</param>
        /// <param name="shadow">Is it a shadow localization? This means it will not appear unless another source NOT marked as a shadow localization uses a language present. This is to avoid a mod implementing a lot of languages that aren't used anywhere else and them appearing on the list.</param>
        /// <returns></returns>
        public LocalizationAsset AddLocalizationSheetFromAssembly(string assemblyPath, GoogleDriveDownloadFormat type, bool shadow = false)
        {
            return _localizer?.AddLocalizationSheetFromAssembly(assemblyPath, type, shadow);
        }
    }
}