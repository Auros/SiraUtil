using System;
using Zenject;
using ModestTree;
using UnityEngine;
using System.Collections.Generic;

namespace SiraUtil.Zenject
{
    /// <summary>
    /// A builder for constructing <seealso cref="Zenjector"/> bindings.
    /// </summary>
    public class InstallBuilder
    {
        internal Type Type { get; private set; }
        internal string Destination { get; private set; }
        internal object[] Parameters { get; private set; }
        internal Func<bool> WhenInstall { get; private set; } = null;
        internal List<string> Circuits { get; } = new List<string>();
        internal Action<DiContainer> Contextless { get; private set; } = null;
        internal HashSet<Type> Exposers { get; private set; } = new HashSet<Type>();
        internal Action<SceneContext, DiContainer> Resolved { get; private set; } = null;
        internal HashSet<Tuple<Type, DelegateWrapper>> Mutators { get; private set; } = new HashSet<Tuple<Type, DelegateWrapper>>();
        internal HashSet<Tuple<Type, Action<Context, DiContainer>>> Headers { get; private set; } = new HashSet<Tuple<Type, Action<Context, DiContainer>>>();

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
            On(typeof(T).Name);
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
            return ShortCircuitFor(typeof(StandardLevelScenesTransitionSetupDataSO).FullName);
        }

        /// <summary>
        /// Prevents the installer from being installed on the campaign game level.
        /// </summary>
        public InstallBuilder ShortCircuitForCampaign()
        {
            return ShortCircuitFor(typeof(MissionLevelScenesTransitionSetupDataSO).FullName);
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
            return ShortCircuitFor(typeof(MultiplayerLevelScenesTransitionSetupDataSO).FullName);
        }

        /// <summary>
        /// Prevents the installer from being installed at a destination.
        /// </summary>
        public InstallBuilder ShortCircuitFor(string shortCircuiter)
        {
            Circuits.Add(shortCircuiter);
            return this;
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
        /// Only install this installer in a multiplayer game.
        /// </summary>
        public InstallBuilder OnlyForMultiplayer()
        {
            ShortCircuitForCampaign();
            ShortCircuitForTutorial();
            ShortCircuitForStandard();
            return this;
        }

        /// <summary>
        /// Exposes a <see cref="MonoBehaviour"/> in a <see cref="SceneDecoratorContext"/> to the <seealso cref="DiContainer"/> and binds it, thus making it available to be received in Zenject.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="MonoBehaviour"/>.</typeparam>
        public InstallBuilder Expose<T>() where T : MonoBehaviour
        {
            Exposers.Add(typeof(T));
            return this;
        }

        /// <summary>
        /// Mutate a <see cref="MonoBehaviour"/> in a <see cref="Context"/> before it gets injected.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="MonoBehaviour"/>.</typeparam>
        /// <param name="action">The callback to handle mutations in.</param>
        public InstallBuilder Mutate<T>(Action<MutationContext, MonoBehaviour> action) where T : MonoBehaviour
        {
            if (action == null)
            {
                return this;
            }
            Mutators.Add(new Tuple<Type, DelegateWrapper>(typeof(T), new DelegateWrapper().Wrap(action)));
            return this;
        }

        /// <summary>
        /// Mutate a <see cref="MonoBehaviour"/> in a <see cref="Context"/> before it gets injected.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="MonoBehaviour"/>.</typeparam>
        /// <param name="action">The callback to handle mutations in.</param>
        public InstallBuilder Mutate<T>(Action<MutationContext, T> action) where T : MonoBehaviour
        {
            if (action == null)
            {
                return this;
            }
            Mutators.Add(new Tuple<Type, DelegateWrapper>(typeof(T), new DelegateWrapper().Wrap(action)));
            return this;
        }

        /// <summary>
        /// Install bindings with a pseudo action which acts like an installer. Not recommened for medium-large sized projects.
        /// </summary>
        /// <param name="action">The invoked action.</param>
        public InstallBuilder Pseudo(Action<DiContainer> action)
        {
            Contextless = action;
            return this;
        }

        /// <summary>
        /// Mainly for prototyping.
        /// </summary>
        /// <param name="onInit">An action that's invoked when the context has finished installing.</param>
        public InstallBuilder Initialized(Action<SceneContext, DiContainer> onInit)
        {
            Resolved = onInit;
            return this;
        }

        /// <summary>
        /// Conditionally install your installer.
        /// </summary>
        /// <param name="when">When to install.</param>
        /// <returns></returns>
        public InstallBuilder When(Func<bool> when)
        {
            WhenInstall = when;
            return this;
        }

        internal void Validate()
        {
            if (Contextless == null)
            {
                Assert.IsNotNull(Type, $"Contextful Zenject Registrations must have a type. {Utilities.ASSERTHIT}");
                Assert.That(Type.DerivesFrom<IInstaller>(), $"Type must implement IInstaller {Utilities.ASSERTHIT}");
            }
            if (string.IsNullOrEmpty(Destination))
            {
                throw new ArgumentNullException($"{nameof(Type)}:{nameof(Destination)}", "Installer registration needs a destination.");
            }
        }

        internal class DelegateWrapper
        {
            public Action<MutationContext, object> actionObj;

            public DelegateWrapper Wrap<T, U>(Action<T, U> callback) where T : MutationContext
            {
                actionObj = delegate (MutationContext context, object obj)
                {
                    callback((T)context, (U)obj);
                };
                return this;
            }
        }
    }
}