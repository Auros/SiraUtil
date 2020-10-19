using System;
using Zenject;
using Polyglot;
using System.IO;
using HarmonyLib;
using UnityEngine;
using System.Linq;
using IPA.Utilities;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
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

    [HarmonyPatch(typeof(UnityEngine.Debug), "LogWarning", argumentTypes: new Type[] { typeof(object) })]
    internal static class RemoveLocalizationWarningLog
    {
        internal static bool Prefix(ref object message)
        {
            return !message.ToString().StartsWith("Could not find key") || Environment.GetCommandLineArgs().Contains("--siralog");
        }
    }


    public class Localizer : IInitializable, ILateDisposable
    {
        private static readonly Dictionary<string, LocalizationAsset> _lockedAssetCache = new Dictionary<string, LocalizationAsset>();

        private readonly Config _config;
        private readonly WebClient _webClient;
        private LocalizationAsset _siraLocalizationAsset;

        public Localizer(Config config, WebClient webClient)
        {
            _config = config;
            _webClient = webClient;
        }

        public async void Initialize()
        {
            _siraLocalizationAsset = AddLocalizationSheetFromAssembly("SiraUtil.Resources.master_oct7-2020.tsv", GoogleDriveDownloadFormat.TSV, true);
            var stopwatch = Stopwatch.StartNew();
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
            stopwatch.Stop();
            Plugin.Log.Sira($"Took {stopwatch.Elapsed.TotalSeconds} seconds to download, parse, and load {successCount} localization sheets.");
            CheckLanguages();
			/*
            List<string> keys = LocalizationImporter.GetKeys();

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
            File.WriteAllLines(Path.Combine(savePath, "English.txt"), english.ToArray());
            */
            
            /*Localization.Instance.GetField<List<Language>, Localization>("supportedLanguages").Add(Language.French);
            Localization.Instance.SelectedLanguage = Language.French;*/
        }

        public void CheckLanguages()
        {
#if DEBUG
            var stopwatch = Stopwatch.StartNew();
#endif
            var supported = Localization.Instance.GetField<List<Language>, Localization>("supportedLanguages");
            FieldInfo field = typeof(LocalizationImporter).GetField("languageStrings", BindingFlags.NonPublic | BindingFlags.Static);
            var locTable = (Dictionary<string, List<string>>)field.GetValue(null);
            ISet<int> validLanguages = new HashSet<int>();
            foreach (var value in locTable.Values)
            {
                for (int i = 0; i < value.Count; i++)
                {
                    if (!string.IsNullOrEmpty(value.ElementAtOrDefault(i)))
                    {
                        validLanguages.Add(i);
                    }
                }
            }

            supported.Clear();
            for (int i = 0; i < validLanguages.Count; i++)
            {
                supported.Add((Language)validLanguages.ElementAt(i));
                Plugin.Log.Sira($"Language Detected: {(Language)validLanguages.ElementAt(i)}");
            }
            stopwatch.Stop();
            Plugin.Log.Sira($"Took {stopwatch.Elapsed:c} to recalculate languages.");
            Localization.Instance.InvokeOnLocalize();
        }

        public void AddLocalizationSheet(LocalizationAsset localizationAsset)
        {
            var loc = _lockedAssetCache.Where(x => x.Value == localizationAsset || x.Value.TextAsset.text == localizationAsset.TextAsset.text).FirstOrDefault();
            if (loc.Equals(default(KeyValuePair<string, LocalizationAsset>)))
            {
                return;
            }
            Localization.Instance.GetField<List<LocalizationAsset>, Localization>("inputFiles").Add(localizationAsset);
            LocalizationImporter.Refresh();
        }

        public void RemoveLocalizationSheet(LocalizationAsset localizationAsset)
        {
            var loc = _lockedAssetCache.Where(x => x.Value == localizationAsset || x.Value.TextAsset.text == localizationAsset.TextAsset.text).FirstOrDefault();
            if (!loc.Equals(default(KeyValuePair<string, LocalizationAsset>)))
            {
                _lockedAssetCache.Remove(loc.Key);
            }
        }

        public void RemoveLocalizationSheet(string key)
        {
            _lockedAssetCache.Remove(key);
        }

        public LocalizationAsset AddLocalizationSheet(string localizationAsset, GoogleDriveDownloadFormat type, string id, bool addToPolyglot = true)
        {
            var asset = new LocalizationAsset
            {
                Format = type,
                TextAsset = new TextAsset(localizationAsset)
            };
            if (!_lockedAssetCache.ContainsKey(id))
            {
                _lockedAssetCache.Add(id, asset);
            }
            if (addToPolyglot)
            {
                AddLocalizationSheet(asset);
            }
            return asset;
        }

        public LocalizationAsset AddLocalizationSheetFromAssembly(string assemblyPath, GoogleDriveDownloadFormat type, bool addToPolyglot = true)
        {
            Utilities.AssemblyFromPath(assemblyPath, out Assembly assembly, out string path);
            string content = Utilities.GetResourceContent(assembly, path);
            var locSheet = AddLocalizationSheet(content, type, path, addToPolyglot);
            if (!_lockedAssetCache.ContainsKey(path))
            {
                _lockedAssetCache.Add(path, locSheet);
            }
            return locSheet;
        }

        public void LateDispose()
        {
            RemoveLocalizationSheet(_siraLocalizationAsset);
        }
    }
}