using System;
using Zenject;
using ModestTree;
using UnityEngine;
using System.Collections.Generic;

namespace SiraUtil.Zenject
{
    public class InstallBuilder
    {
        internal Type Type { get; private set; }
        internal string Destination { get; private set; }
        internal object[] Parameters { get; private set; }
        internal List<string> Circuits { get; } = new List<string>();
        internal HashSet<Type> Exposers { get; private set; } = new HashSet<Type>();
        internal HashSet<Tuple<Type, Action<MutationContext, MonoBehaviour>>> Mutators { get; private set; } = new HashSet<Tuple<Type, Action<MutationContext, MonoBehaviour>>>();

        internal InstallBuilder() { }
        internal InstallBuilder(Type type)
        {
            Type = type;
        }

        /// <summary>
        /// Add instance parameters to be injected into your installer.
        /// </summary>
        /// <param name="parameters">The parameters for your installer.</param>
        public InstallBuilder WithParameters(params object[] parameters)
        {
            Parameters = parameters;
            return this;
        }

        /// <summary>
        /// Installs the installer at a destination.
        /// </summary>
        /// <param name="destination">The name of the destination (scene name, installer type, scene context name, etc).</param>
        public InstallBuilder On(string destination)
        {
            Destination = destination;
            return this;
        }

        /// <summary>
        /// Installs the installer at a destination.
        /// </summary>
        /// <typeparam name="T">The type of the destination (installer type, etc).</typeparam>
        public InstallBuilder On<T>()
        {
            On(nameof(T));
            return this;
        }

        /// <summary>
        /// Registers an installer to be installed.
        /// </summary>
        /// <typeparam name="T">The type of the installer.</typeparam>
        public InstallBuilder Register<T>() where T : IInstaller
        {
            Type = typeof(T);
            return this;
        }

        /// <summary>
        /// Prevents the installer from being installed at a destination.
        /// </summary>
        /// <typeparam name="T">The type of the destination.</typeparam>
        public InstallBuilder ShortCircuitFor<T>()
        {
            return ShortCircuitFor(typeof(T).Name);
        }

        /// <summary>
        /// Prevents the installer from being installed on the standard game level.
        /// </summary>
        public InstallBuilder ShortCircuitForStandard()
        {
            return ShortCircuitFor(nameof(StandardLevelScenesTransitionSetupDataSO));
        }

        /// <summary>
        /// Prevents the installer from being installed on the campaign game level.
        /// </summary>
        public InstallBuilder ShortCircuitForCampaign()
        {
            return ShortCircuitFor(nameof(MissionLevelScenesTransitionSetupDataSO));
        }

        /// <summary>
        /// Prevents the installer from being installed on the tutorial game level.
        /// </summary>
        public InstallBuilder ShortCircuitForTutorial()
        {
            return ShortCircuitFor("Tutorial");
        }

        /// <summary>
        /// Prevents the installer from being installed on the multiplayer game level.
        /// </summary>
        public InstallBuilder ShortCircuitForMultiplayer()
        {
            return ShortCircuitFor(nameof(MultiplayerLevelScenesTransitionSetupDataSO));
        }

        /// <summary>
        /// Prevents the installer from being installed at a destination.
        /// </summary>
        public InstallBuilder ShortCircuitFor(string shortCircuiter)
        {
            Circuits.Add(shortCircuiter);
            return this;
        }

        [Obsolete("Use ShortCircuitFor<T>() instead.")]
        public InstallBuilder ShortCircuitOn<T>()
        {
            return ShortCircuitFor(typeof(T).Name);
        }

        [Obsolete("Use ShortCircuitForStandard() instead.")]
        public InstallBuilder ShortCircuitOnStandard()
        {
            return ShortCircuitFor(nameof(StandardLevelScenesTransitionSetupDataSO));
        }

        [Obsolete("Use ShortCircuitForCampaign() instead.")]
        public InstallBuilder ShortCircuitOnCampaign()
        {
            return ShortCircuitFor(nameof(MissionLevelScenesTransitionSetupDataSO));
        }

        [Obsolete("Use ShortCircuitForTutorial() instead.")]
        public InstallBuilder ShortCircuitOnTutorial()
        {
            return ShortCircuitFor("Tutorial");
        }

        [Obsolete("Use ShortCircuitForMultiplayer() instead.")]
        public InstallBuilder ShortCircuitOnMultiplayer()
        {
            return ShortCircuitFor(nameof(MultiplayerLevelScenesTransitionSetupDataSO));
        }

        /// <summary>
        /// Only install this installer on the standard game.
        /// </summary>
        public InstallBuilder OnlyForStandard()
        {
            ShortCircuitForCampaign();
            ShortCircuitForTutorial();
            ShortCircuitForMultiplayer();
            return this;
        }

        /// <summary>
        /// Exposes a <see cref="MonoBehaviour"/> in a <see cref="SceneDecoratorContext"/> to the <seealso cref="DiContainer"/> and binds it, thus making it available to be received in Zenject.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="MonoBehaviour"/>.</typeparam>
        /// <returns></returns>
        public InstallBuilder Expose<T>() where T : MonoBehaviour
        {
            Exposers.Add(typeof(T));
            return this;
        }

        /// <summary>
        /// Mutate a <see cref="MonoBehaviour"/> in a <see cref="SceneDecoratorContext"/> before it gets injected.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="MonoBehaviour"/>.</typeparam>
        /// <param name="action">The callback to handle mutations in.</param>
        /// <returns></returns>
        public InstallBuilder Mutate<T>(Action<MutationContext, MonoBehaviour> action) where T : MonoBehaviour
        {
            if (action == null)
            {
                return this;
            }
            Mutators.Add(new Tuple<Type, Action<MutationContext, MonoBehaviour>>(typeof(T), action));
            return this;
        }

        internal void Validate()
        {
            Assert.IsNotNull(Type, $"Zenject Registration must have a type. {Utilities.ASSERTHIT}");
            Assert.That(Type.DerivesFrom<IInstaller>(), $"Type must be an IInstaller {Utilities.ASSERTHIT}");
            if (string.IsNullOrEmpty(Destination))
            {
                throw new ArgumentNullException($"{nameof(Type)}:{nameof(Destination)}", "Installer registration needs a destination.");
            }
        }
    }
}