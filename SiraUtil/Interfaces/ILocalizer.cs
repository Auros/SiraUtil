using Polyglot;

namespace SiraUtil.Interfaces
{
    public interface ILocalizer
    {
        /// <summary>
        /// Creates a localization asset.
        /// </summary>
        /// <param name="localizationAsset">The text to generate it from.</param>
        /// <param name="type">The format of the localization data.</param>
        /// <param name="id">The ID of the localization data.</param>
        /// <param name="shadow">Only show a language set if another localization asset is using it.</param>
        /// <returns>The LocalizationAsset of the sheet.</returns>
        LocalizationAsset AddLocalizationSheet(string localizationAsset, GoogleDriveDownloadFormat type, string id, bool shadow = false);

        /// <summary>
        /// Adds a localization sheet from an assembly path.
        /// </summary>
        /// <param name="assemblyPath">The assembly path to the localization asset file.</param>
        /// <param name="id">The ID of the localization data.</param>
        /// <param name="shadow">Only show a language set if another localization asset is using it.</param>
        /// <returns>The LocalizationAsset of the sheet.</returns>
        LocalizationAsset AddLocalizationSheetFromAssembly(string assemblyPath, GoogleDriveDownloadFormat type, bool shadow = false);

        /// <summary>
        /// Adds a localization asset to Polyglot.
        /// </summary>
        /// <param name="localizationAsset"></param>
        /// <param name="shadow">Only show a language set if another localization asset is using it.</param>
        void AddLocalizationSheet(LocalizationAsset localizationAsset, bool shadow = false);

        /// <summary>
        /// Removes a localization asset from Polyglot.
        /// </summary>
        /// <param name="localizationAsset"></param>
        void RemoveLocalizationSheet(LocalizationAsset localizationAsset);

        /// <summary>
        /// Removes a localization asset from Polyglot.
        /// </summary>
        /// <param name="key">The name or source of the asset.</param>
        void RemoveLocalizationSheet(string key);

        /// <summary>
        /// Recalculate the supported languages table.
        /// </summary>
        void RecalculateLanguages();
    }
}