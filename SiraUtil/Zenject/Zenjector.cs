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
using UnityEngine;
using Zenject;
using Logger = IPA.Logging.Logger;

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
        internal IEnumerable<InstallSet> InstallSets => _installSets;
        internal IEnumerable<InstallInstruction> InstallInstructions => _installInstructions;
        internal IEnumerable<IInjectableMonoBehaviourInstruction> InjectableMonoBehaviourInstructions => _injectableMonoBehaviourInstructions;

        private bool _autoBinded;
        private readonly HashSet<InstallSet> _installSets = [];
        private readonly HashSet<InstallInstruction> _installInstructions = [];
        private readonly HashSet<IInjectableMonoBehaviourInstruction> _injectableMonoBehaviourInstructions = [];

        internal Zenjector(PluginMetadata metadata)
        {
            Metadata = metadata;
            SiraSyncID = metadata.Id;
            SiraSyncOwner = metadata.Author;
            Slog = metadata.PluginType?.CustomAttributes.Any(ca => ca.AttributeType.FullName == typeof(SlogAttribute).FullName) ?? false;
        }

        /// <summary>
        /// Installs a custom installer to a location.
        /// </summary>
        /// <typeparam name="T">The type of your custom installer.</typeparam>
        /// <param name="location">The location to install it to.</param>
        /// <param name="parameters">
        /// Parameters for the constructor of the installer. This will override Zenject's constructor injection on this installer,
        /// and the installer type cannot be a <see cref="MonoInstaller" /> if using this.
        /// </param>
        public void Install<T>(Location location, params object[] parameters) where T : IInstaller
        {
            IEnumerable<IInstallFilter> filters = FiltersForLocation(location);

            foreach (IInstallFilter filter in filters)
            {
                _installSets.Add(new InstallSet(typeof(T), filter, parameters.Length != 0 ? parameters : null));
            }
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
        /// Install bindings to a custom location.
        /// </summary>
        /// <param name="location">The location to install it to.</param>
        /// <param name="installCallback">The callback which is used to install custom bindings into the container.</param>
        public void Install(Location location, Action<DiContainer> installCallback)
        {
            foreach (IInstallFilter filter in FiltersForLocation(location))
            {
                _installInstructions.Add(new InstallInstruction(filter, installCallback));
            }
        }

        /// <summary>
        /// Install bindings alongsise another installer without a custom installer.
        /// </summary>
        /// <typeparam name="TBaseInstaller">The installer to install your bindings with.</typeparam>
        /// <param name="installCallback">The callback which is used to install custom bindings into the container.</param>
        public void Install<TBaseInstaller>(Action<DiContainer> installCallback) where TBaseInstaller : IInstaller
        {
            _installInstructions.Add(new InstallInstruction(new TypedInstallFilter(typeof(TBaseInstaller)), installCallback));
        }

        private IEnumerable<IInstallFilter> FiltersForLocation(Location location)
        {
            if (location.HasFlag(Location.App))
            {
                yield return new TypedInstallFilter(typeof(BeatSaberInit));
            }

            if (location.HasFlag(Location.Menu))
            {
                yield return new TypedInstallFilter(typeof(MainSettingsMenuViewControllersInstaller));
            }

            if (location.HasFlag(Location.StandardPlayer))
            {
                yield return new TypedInstallFilter(typeof(StandardGameplayInstaller));
            }

            if (location.HasFlag(Location.CampaignPlayer))
            {
                yield return new TypedInstallFilter(typeof(MissionGameplayInstaller));
            }

            if (location.HasFlag(Location.MultiPlayer))
            {
                yield return new TypedInstallFilter(typeof(MultiplayerLocalActivePlayerInstaller));
            }

            if (location.HasFlag(Location.Tutorial))
            {
                yield return new TypedInstallFilter(typeof(TutorialInstaller));
            }

            if (location.HasFlag(Location.GameCore))
            {
                yield return new TypedInstallFilter(typeof(GameCoreSceneSetup));
            }

            if (location.HasFlag(Location.MultiplayerCore))
            {
                yield return new TypedInstallFilter(typeof(MultiplayerCoreInstaller));
            }

            if (location.HasFlag(Location.ConnectedPlayer))
            {
                yield return new TypedInstallFilter(typeof(MultiplayerConnectedPlayerInstaller));
            }

            if (location.HasFlag(Location.AlwaysMultiPlayer))
            {
                yield return new TypedInstallFilter(typeof(MultiplayerLocalPlayerInstaller));
            }

            if (location.HasFlag(Location.InactiveMultiPlayer))
            {
                yield return new TypedInstallFilter(typeof(MultiplayerLocalInactivePlayerInstaller));
            }

            if (location.HasFlag(Location.HealthWarning))
            {
                yield return new ContextedNamedSceneInstallFilter<SceneContext>("HealthWarning");
            }

            if (location.HasFlag(Location.Credits))
            {
                yield return new ContextedNamedSceneInstallFilter<SceneContext>("Credits");
            }

            if (location.HasFlag(Location.StartupError))
            {
                yield return new ContextedNamedSceneInstallFilter<SceneContext>("StartupError");
            }
        }

        /// <summary>
        /// Finds all types that match <typeparamref name="TMonoBehaviour"/> in a context and binds them to the context's container if they are not yet bound.
        /// </summary>
        /// <remarks>
        /// This is similar to having a <see cref="ZenjectBinding"/> that specifies <typeparamref name="TMonoBehaviour"/> wherever it exists.
        /// </remarks>
        /// <typeparam name="TMonoBehaviour">The type of <see cref="MonoBehaviour"/> to expose.</typeparam>
        /// <param name="identifier">The identifier to use when binding the type.</param>
        /// <param name="useSceneContext">Whether or not to bind to the parent <see cref="SceneContext"/>. This is useful when the component you are targeting is inside a <see cref="GameObjectContext"/>.</param>
        /// <param name="ifNotBound">Only bind this type if it is not already bound. Zenject will throw an error when trying to resolve a single instance if more than one instance is bound.</param>
        /// <param name="condition">An optional callback that returns a value indicating whether or not the type should be exposed given the <typeparamref name="TMonoBehaviour"/> and its associated <see cref="Context"/>.</param>
        /// <param name="bindTypes">An optional enumerable of types against which <typeparamref name="TMonoBehaviour"/> should be bound. If <see langword="null"/>, defaults to just <typeparamref name="TMonoBehaviour"/>.</param>
        public void Expose<TMonoBehaviour>(object? identifier = null, bool useSceneContext = false, bool ifNotBound = false, Func<Context, TMonoBehaviour, bool>? condition = null, IEnumerable<Type>? bindTypes = null)
            where TMonoBehaviour : MonoBehaviour
        {
            bindTypes ??= [typeof(TMonoBehaviour)];

            if (!bindTypes.Any())
            {
                throw new ArgumentException("Must bind to at least one type", nameof(bindTypes));
            }

            _injectableMonoBehaviourInstructions.Add(new ExposeInstruction<TMonoBehaviour>(identifier, useSceneContext, ifNotBound, condition, bindTypes));
        }

        /// <summary>
        /// Finds all types that match <typeparamref name="TExposeType"/> on the <see cref="SceneDecoratorContext"/> with the given <paramref name="contractName"/> and binds them to the context's container if they are not yet bound.
        /// </summary>
        /// <typeparam name="TExposeType">The type to expose.</typeparam>
        /// <param name="contractName">The contract name of the <see cref="SceneDecoratorContext"/> to search on.</param>
        [Obsolete("Use Expose<TMonoBehaviour>() with a condition instead")]
        public void Expose<TExposeType>(string contractName)
        {
            if (contractName is null)
            {
                throw new ArgumentNullException(nameof(contractName));
            }

            _injectableMonoBehaviourInstructions.Add(new SceneDecoratorExposeInstruction<TExposeType>(contractName));
        }

        /// <summary>
        /// Calls <paramref name="action"/> on all instances of <typeparamref name="TMonoBehaviour"/> in a context.
        /// </summary>
        /// <typeparam name="TMonoBehaviour">The <see cref="MonoBehaviour"/> to match.</typeparam>
        /// <param name="action">The callback to invoke when <typeparamref name="TMonoBehaviour"/> is encountered.</param>
        public void Mutate<TMonoBehaviour>(Action<Context, TMonoBehaviour> action) where TMonoBehaviour : MonoBehaviour
        {
            _injectableMonoBehaviourInstructions.Add(new MutateInstruction<TMonoBehaviour>(action));
        }

        /// <summary>
        /// Instantiates a <typeparamref name="TNewComponent"/> alongside any existing <typeparamref name="TMonoBehaviour"/> on a context.
        /// </summary>
        /// <typeparam name="TMonoBehaviour">The <see cref="MonoBehaviour"/> to match.</typeparam>
        /// <typeparam name="TNewComponent">The new <see cref="Component"/> to instantiate.</typeparam>
        /// <param name="action">An optional callback to invoke with the new component after it's created.</param>
        /// <param name="gameObjectGetter">An optional function to specify on which <see cref="GameObject"/> the <typeparamref name="TNewComponent"/> should be added. If <see langword="null"/>, the new component is added to the same <see cref="GameObject"/> as the <typeparamref name="TMonoBehaviour"/>.</param>
        /// <param name="condition">An optional callback that returns a value indicating whether or not <typeparamref name="TNewComponent"/> should be added given the <typeparamref name="TMonoBehaviour"/> and its associated <see cref="Context"/>.</param>
        /// <param name="bindTypes">An optional enumerable of types against which <typeparamref name="TMonoBehaviour"/> should be bound. If <see langword="null"/>, nothing is bound.</param>
        public void Mutate<TMonoBehaviour, TNewComponent>(
            Action<Context, TMonoBehaviour, TNewComponent>? action = null,
            Func<Context, TMonoBehaviour, GameObject>? gameObjectGetter = null,
            Func<Context, TMonoBehaviour, bool>? condition = null,
            IEnumerable<Type>? bindTypes = null)
            where TMonoBehaviour : MonoBehaviour where TNewComponent : Component
        {
            if (bindTypes != null && !bindTypes.Any())
            {
                throw new ArgumentException("Must bind to at least one type if not null", nameof(bindTypes));
            }

            _injectableMonoBehaviourInstructions.Add(new MutateInstruction<TMonoBehaviour, TNewComponent>(action, gameObjectGetter, condition, bindTypes));
        }

        /// <summary>
        /// Calls <paramref name="mutationCallback"/> on all instances of <typeparamref name="TMutateType"/> in a <see cref="SceneDecoratorContext"/>.
        /// </summary>
        /// <typeparam name="TMutateType">The type to mutate.</typeparam>
        /// <param name="contractName">The contract name of the <see cref="SceneDecoratorContext" /> to search on.</param>
        /// <param name="mutationCallback">The callback used to mutate the object instance.</param>
        [Obsolete("Use Mutate<TMonoBehaviour>(Action<Context, TMonoBehaviour>) instead")]
        public void Mutate<TMutateType>(string contractName, Action<SceneDecoratorContext, TMutateType> mutationCallback)
        {
            _injectableMonoBehaviourInstructions.Add(new SceneDecoratorMutateInstruction<TMutateType>(contractName, mutationCallback));
        }

        /// <summary>
        /// Sets up a logger to be used in Zenject.
        /// </summary>
        /// <param name="logger">The logger to use as a source. If nothing is put in here, a logger is generated automatically.</param>
        public void UseLogger(Logger? logger = null)
        {
            if (logger == null)
            {
                Plugin.Log.Warn($"`UseLogger` called by '{Metadata.Name}' without passing a logger instance. Calling `UseLogger` without passing a logger is deprecated and will raise an exception in future releases.");
            }

            // Creates a new logger if no logger is specified.
            // TODO: Get rid of the StandardLogger instantiation in v4
            Logger = logger ?? AccessTools.Constructor(typeof(StandardLogger), [typeof(string)]).Invoke([Metadata.Name]) as StandardLogger;
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
            {
                SiraSyncID = modID!;
            }

            if (!string.IsNullOrWhiteSpace(userID))
            {
                SiraSyncOwner = userID!;
            }

            if (HttpServiceType is null)
            {
                UseHttpService();
            }
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
            {
                return;
            }

            _autoBinded = true;
            foreach (Type? type in Metadata.Assembly.GetTypes())
            {
                BindAttribute? bind = type.GetCustomAttribute<BindAttribute>();
                if (bind is not null)
                {
                    Plugin.Log.Debug($"Found bind attribute in type '{type.FullName}'");
                    AutobindInstruction instruction = new(type, bind);
                    Install(instruction.Location, instruction.Bind);
                }
            }
        }
    }
}