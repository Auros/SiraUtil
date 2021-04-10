using IPA.Loader;
using SiraUtil.Zenject.Internal;
using SiraUtil.Zenject.Internal.Filters;
using System;
using System.Collections.Generic;
using Zenject;

namespace SiraUtil.Zenject
{
    /// <summary>
    /// A constructor class for building Zenject installer registration events.
    /// </summary>
    public class Zenjector
    {
        internal PluginMetadata Metadata { get; }
        internal IEnumerable<InstallSet> Sets => _installSets;
        private readonly HashSet<InstallSet> _installSets = new();

        internal Zenjector(PluginMetadata metadata)
        {
            Metadata = metadata;
        }

        /// <summary>
        /// Installs a custom installer to a location with a backing installer.
        /// </summary>
        /// <typeparam name="T">The type of your custom installer.</typeparam>
        /// <param name="location">The location to install it to.</param>
        /// <param name="parameters">
        /// Parameters for the constructor of the installer. This will override Zenject's constructor injection on this installer,
        /// and the installer type cannot be a <see cref="MonoInstaller" />.
        /// </param>
        public void Install<T>(Location location, params object[] parameters) where T : IInstaller
        {
            HashSet<Type> installerTypes = new();
            if (location.HasFlag(Location.App))
                installerTypes.Add(typeof(PCAppInit));
            if (location.HasFlag(Location.Menu))
                installerTypes.Add(typeof(MenuInstaller));
            if (location.HasFlag(Location.StandardPlayer))
                installerTypes.Add(typeof(StandardGameplayInstaller));
            if (location.HasFlag(Location.CampaignPlayer))
                installerTypes.Add(typeof(MissionGameplayInstaller));
            if (location.HasFlag(Location.MultiPlayer))
                installerTypes.Add(typeof(MultiplayerLocalPlayerInstaller));
            if (location.HasFlag(Location.Tutorial))
                installerTypes.Add(typeof(TutorialInstaller));
            if (location.HasFlag(Location.GameCore))
                installerTypes.Add(typeof(GameCoreSceneSetup));
            if (location.HasFlag(Location.MultiplayerCore))
                installerTypes.Add(typeof(MultiplayerCoreInstaller));

            IInstallFilter filters = new MultiTypedInstallFilter(installerTypes);
            _installSets.Add(new InstallSet(typeof(T), filters, parameters.Length != 0 ? parameters : null));
        }
    }
}