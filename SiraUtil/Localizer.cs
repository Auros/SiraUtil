using System;
using Zenject;
using Polyglot;
using System.IO;
using HarmonyLib;
using UnityEngine;
using System.Linq;
using IPA.Utilities;
using System.Threading;
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
        private static readonly Dictionary<string, LocalizationAsset> _lockedAssetCache = new Dictionary<string, LocalizationAsset>();

        private readonly Config _config;
        private readonly WebClient _webClient;
        private readonly ILocalizer _localizer;

        public Localizer(Config config, WebClient webClient, [InjectOptional(Id = "SIRA.Localizer")] ILocalizer localizer)
        {
            _config = config;
            _webClient = webClient;
            _localizer = localizer;
        }

        public async void Initialize()
        {
            int successCount = 0;
            foreach (var source in _config.Localization.Sources.Where(s => s.Value.Enabled == true))
            {
                WebResponse response = await _webClient.GetAsync(source.Value.URL, CancellationToken.None);
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        if (!_lockedAssetCache.TryGetValue(source.Key, out LocalizationAsset asset))
                        {
                            using (var ms = new MemoryStream(response.ContentToBytes()))
                            {
                                using (var reader = new StreamReader(ms))
                                {
                                    asset = new LocalizationAsset
                                    {
                                        Format = source.Value.Format,
                                        TextAsset = new TextAsset(response.ContentToString())
                                    };
                                }
                            }
                            _lockedAssetCache.Add(source.Key, asset);
                            Localization.Instance.GetField<List<LocalizationAsset>, Localization>("inputFiles").Add(asset);
                            LocalizationImporter.Refresh();
                        }
                        successCount++;
                    }
                    catch
                    {
                        Plugin.Log.Warn($"Could not parse localization data from {source.Key}");
                        continue;
                    }
                }
                else
                {
                    Plugin.Log.Warn($"Could not fetch localization data from {source.Key}");
                }
            }
            /*List<string> keys = LocalizationImporter.GetKeys();

            string savePath = Path.Combine(UnityGame.UserDataPath, "SiraUtil", "Localization", "Dumps");
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            File.WriteAllLines(Path.Combine(savePath, "Keys.txt"), keys.ToArray());

            var english = new List<string>();
            foreach (var key in keys)
            {
                var contains = LocalizationImporter.GetLanguagesContains(key);
                english.Add(contains[key].First());
            }
            File.WriteAllLines(Path.Combine(savePath, "English.txt"), english.ToArray());*/
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