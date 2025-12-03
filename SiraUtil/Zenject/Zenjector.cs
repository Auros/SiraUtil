using HarmonyLib;
using IPA.Loader;
using IPA.Logging;
using SiraUtil.Attributes;
using SiraUtil.Web;
using SiraUtil.Web.SiraSync;
using SiraUtil.Zenject.Internal;
using SiraUtil.Zenject.Internal.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Zenject;

namespace SiraUtil.Zenject
{
    /// <summary>
    /// A constructor class for building Zenject installer registration events.
    /// </summary>
    public class Zenjector
    {
        internal bool Slog { get; }
        internal Logger? Logger { get; set; }
        internal PluginMetadata Metadata { get; }
        internal Type? UBinderType { get; private set; }
        internal string SiraSyncID { get; private set; }
        internal string SiraSyncOwner { get; private set; }
        internal object? UBinderValue { get; private set; }
        internal HttpServiceType? HttpServiceType { get; private set; }
        internal SiraSyncServiceType? SiraSyncServiceType { get; private set; }
        internal IEnumerable<ExposeSet> ExposeSets => _exposeSets;
        internal IEnumerable<MutateSet> MutateSets => _mutateSets;
        internal IEnumerable<InstallSet> InstallSets => _installSets;
        internal IEnumerable<InstallInstruction> InstallInstructions => _installInstructions;
        internal IEnumerable<AutobindInstruction> AutobindInstructions => _autobindInstructions;

        private bool _autoBinded;
        private readonly HashSet<ExposeSet> _exposeSets = new();
        private readonly HashSet<MutateSet> _mutateSets = new();
        private readonly HashSet<InstallSet> _installSets = new();
        private readonly HashSet<InstallInstruction> _installInstructions = new();
        private readonly HashSet<AutobindInstruction> _autobindInstructions = new();

        internal Zenjector(PluginMetadata metadata)
        {
            Metadata = metadata;
            SiraSyncID = metadata.Id;
            SiraSyncOwner = metadata.Author;
            Slog = metadata.PluginType?.CustomAttributes.Any(ca => ca.AttributeType.FullName == typeof(SlogAttribute).FullName) ?? false;
        }

        /// <summary>
        /// Installs a custom installer to a location with a backing installer(s).
        /// </summary>
        /// <typeparam name="T">The type of your custom installer.</typeparam>
        /// <param name="location">The location to install it to.</param>
        /// <param name="parameters">
        /// Parameters for the constructor of the installer. This will override Zenject's constructor injection on this installer,
        /// and the installer type cannot be a <see cref="MonoInstaller" /> if using this.
        /// </param>
        public void Install<T>(Location location, params object[] parameters) where T : IInstaller
        {
            IEnumerable<Type> installerTypes = InstallerForLocation(location);
            IInstallFilter filter = new MultiTypedInstallFilter(installerTypes);
            _installSets.Add(new InstallSet(typeof(T), filter, parameters.Length != 0 ? parameters : null));
        }

        /// <summary>
        /// Installs a custom installer alongside another installer.
        /// </summary>
        /// <typeparam name="TCustomInstaller">The new installer being installed.</typeparam>
        /// <typeparam name="TBaseInstaller">The installer to install <typeparamref name="TCustomInstaller"/> with.</typeparam>
        /// <param name="parameters">
        /// Parameters for the constructor of the installer. This will override Zenject's constructor injection on this installer,
        /// and the installer type cannot be a <see cref="MonoInstaller" /> if using this.
        /// </param>
        public void Install<TCustomInstaller, TBaseInstaller>(params object[] parameters) where TCustomInstaller : IInstaller where TBaseInstaller : IInstaller
        {
            IInstallFilter filter = new TypedInstallFilter(typeof(TBaseInstaller));
            _installSets.Add(new InstallSet(typeof(TCustomInstaller), filter, parameters.Length != 0 ? parameters : null));
        }

        /// <summary>
        /// Install bindings to a custom location with a backing installer(s).
        /// </summary>
        /// <param name="location">The location to install it to.</param>
        /// <param name="installCallback">The callback which is used to install custom bindings into the container.</param>
        public void Install(Location location, Action<DiContainer> installCallback)
        {
            foreach (var installlerType in InstallerForLocation(location))
                _installInstructions.Add(new InstallInstruction(installlerType, installCallback));
        }

        /// <summary>
        /// Install bindings alongsise another installer without a custom installer.
        /// </summary>
        /// <typeparam name="TBaseInstaller">The installer to install your bindings with.</typeparam>
        /// <param name="installCallback">The callback which is used to install custom bindings into the container.</param>
        public void Install<TBaseInstaller>(Action<DiContainer> installCallback) where TBaseInstaller : IInstaller
        {
            _installInstructions.Add(new InstallInstruction(typeof(TBaseInstaller), installCallback));
        }

        internal void Install<T, TContext>(string sceneName, params object[] parameters) where T : IInstaller where TContext : Context
        {
            IInstallFilter filter = new ContextedNamedSceneInstallFilter<TContext>(sceneName);
            _installSets.Add(new InstallSet(typeof(T), filter, parameters.Length != 0 ? parameters : null));
        } 

        // This converts the Location to an actual installer type usable by the Zenjector system.
        // These need to be updated if some game update drastically changes, renames, or deletes one of these installers.
        private IEnumerable<Type> InstallerForLocation(Location location)
        {
            HashSet<Type> installerTypes = new();
            if (location.HasFlag(Location.App))
                installerTypes.Add(typeof(BeatSaberInit));
            if (location.HasFlag(Location.Menu))
                installerTypes.Add(typeof(MainSettingsMenuViewControllersInstaller));
            if (location.HasFlag(Location.StandardPlayer))
                installerTypes.Add(typeof(StandardGameplayInstaller));
            if (location.HasFlag(Location.CampaignPlayer))
                installerTypes.Add(typeof(MissionGameplayInstaller));
            if (location.HasFlag(Location.MultiPlayer))
                installerTypes.Add(typeof(MultiplayerLocalActivePlayerInstaller));
            if (location.HasFlag(Location.Tutorial))
                installerTypes.Add(typeof(TutorialInstaller));
            if (location.HasFlag(Location.GameCore))
                installerTypes.Add(typeof(GameCoreSceneSetup));
            if (location.HasFlag(Location.MultiplayerCore))
                installerTypes.Add(typeof(MultiplayerCoreInstaller));
            if (location.HasFlag(Location.ConnectedPlayer))
                installerTypes.Add(typeof(MultiplayerConnectedPlayerInstaller));
            if (location.HasFlag(Location.AlwaysMultiPlayer))
                installerTypes.Add(typeof(MultiplayerLocalPlayerInstaller));
            if (location.HasFlag(Location.InactiveMultiPlayer))
                installerTypes.Add(typeof(MultiplayerLocalInactivePlayerInstaller));
            return installerTypes;
        }

        /// <summary>
        /// Searches a decorator context for the first instance that matches a type, then automatically binds them the the active container.
        /// </summary>
        /// <typeparam name="TExposeType">The type to expose.</typeparam>
        /// <param name="contractName">The contract name of the <see cref="SceneDecoratorContext"/> to search on.</param>
        public void Expose<TExposeType>(string contractName)
        {
            if (contractName is null)
                throw new ArgumentNullException(contractName);

            _exposeSets.Add(new ExposeSet(typeof(TExposeType), contractName));
        }

        /// <summary>
        /// Searches a decorator context for the first instance that matches a type, then invokes a callback with that instance for it to be modified or mutated.
        /// </summary>
        /// <typeparam name="TMutateType">The type to mutate.</typeparam>
        /// <param name="contractName">The contract name of the <see cref="SceneDecoratorContext" /> to search on.</param>
        /// <param name="mutationCallback">The callback used to mutate the object instance.</param>
        public void Mutate<TMutateType>(string contractName, Action<SceneDecoratorContext, TMutateType> mutationCallback)
        {
            // Wraps the action into a class so it can be invoked without a generic.
            DelegateWrapper wrapper = new();
            wrapper.Wrap(mutationCallback);

            _mutateSets.Add(new MutateSet(typeof(TMutateType), contractName, wrapper));
        }

        /// <summary>
        /// Sets up a logger to be used in Zenject.
        /// </summary>
        /// <param name="logger">The logger to use as a source. If nothing is put in here, a logger is generated automatically.</param>
        public void UseLogger(Logger? logger = null)
        {
            // Creates a new logger if no logger is specified.
            Logger = logger ?? AccessTools.Constructor(typeof(StandardLogger), new Type[] { typeof(string) }).Invoke(new object[] { Metadata.Name }) as StandardLogger;
        }

        /// <summary>
        /// Registers your metadata under a UBinder
        /// </summary>
        /// <typeparam name="TKey">The key to retrive it by. Make this a type that is in your assembly.</typeparam>
        /// <remarks>
        /// This allows you to retrieve your PluginMetadata through the container by requesting UBinder|TKey, PluginMetadata| and accessing .Value 
        /// </remarks>
        public void UseMetadataBinder<TKey>()
        {
            object ubinder = new UBinder<TKey, PluginMetadata>(Metadata);
            UBinderType = ubinder.GetType();
            UBinderValue = ubinder;
        }

        /// <summary>
        /// Allows you to use SiraUtil's HTTP service system.
        /// </summary>
        /// <param name="type"></param>
        public void UseHttpService(HttpServiceType type = Web.HttpServiceType.UnityWebRequests)
        {
            HttpServiceType = type;
        }

        /// <summary>
        /// SiraSync allows you to get generic info about your mod from the internet like the latest version and changelog.
        /// </summary>
        /// <param name="type">The type of service to use under the hood.</param>
        /// <param name="userID">The username/ID of the owner of the mod/repo in the SiraSync service.</param>
        /// <param name="modID">The ID of your mod/repo in the SiraSync service.</param>
        /// <remarks>
        /// This will register this Zenjector into the Sira HttpService if it already isn't so.
        /// </remarks>
        public void UseSiraSync(SiraSyncServiceType type = Web.SiraSync.SiraSyncServiceType.GitHub, string? userID = null!, string? modID = null!)
        {
            SiraSyncServiceType = type;
            if (!string.IsNullOrWhiteSpace(modID))
                SiraSyncID = modID!;
            if (!string.IsNullOrWhiteSpace(userID))
                SiraSyncOwner = userID!;

            if (HttpServiceType is null)
                UseHttpService();
        }

        /// <summary>
        /// Registers autobinding support, which provides easy zenject binding installations through the <see cref="BindAttribute"/> attribute.
        /// </summary>
        /// <remarks>
        /// This will scan all the types in your assembly, so if you're using strict optional dependencies, warnings will probably be thrown.
        /// </remarks>
        public void UseAutoBinder()
        {
            if (_autoBinded)
                return;

            _autoBinded = true;
            foreach (var type in Metadata.Assembly.GetTypes())
            {
                BindAttribute? bind = type.GetCustomAttribute<BindAttribute>();
                if (bind is not null)
                {
                    Plugin.Log.Debug($"Found bind attribute in type '{type.FullName}'");
                    var instruction = new AutobindInstruction(type, bind);
                    Install(instruction.Location, Container => instruction.Bind(Container));
                    _autobindInstructions.Add(instruction);
                }
            }
        }
    }
}