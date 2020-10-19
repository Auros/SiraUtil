using System;
using Zenject;
using ModestTree;
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
		internal HashSet<Tuple<Type, Action<DiContainer, object>>> Mutators { get; private set; } = new HashSet<Tuple<Type, Action<DiContainer, object>>>();

		internal InstallBuilder() { }
        internal InstallBuilder(Type type)
        {
            Type = type;
        }

        public InstallBuilder WithParameters(params object[] parameters)
        {
            Parameters = parameters;
            return this;
        }

        public InstallBuilder On(string destination)
        {
            Destination = destination;
            return this;
        }

        public InstallBuilder On<T>()
        {
            On(nameof(T));
            return this;
        }

        public InstallBuilder Register<T>() where T : IInstaller
        {
            Type = typeof(T);
            return this;
        }

		public InstallBuilder ShortCircuitOn(string shortCircuiter)
		{
			Circuits.Add(shortCircuiter);
			return this;
		}

		public InstallBuilder ShortCircuitFor<T>()
		{
			return ShortCircuitOn(typeof(T).Name);
		}

		public InstallBuilder ShortCircuitForStandard()
		{
			return ShortCircuitOn(nameof(StandardLevelScenesTransitionSetupDataSO));
		}

		public InstallBuilder ShortCircuitForCampaign()
		{
			return ShortCircuitOn(nameof(MissionLevelScenesTransitionSetupDataSO));
		}

		public InstallBuilder ShortCircuitForTutorial()
		{
			return ShortCircuitOn("Tutorial");
		}

		public InstallBuilder ShortCircuitForMultiplayer()
		{
			return ShortCircuitOn(nameof(MultiplayerLevelScenesTransitionSetupDataSO));
		}

		[Obsolete("Use ShortCircuitFor<T>() instead.")]
		public InstallBuilder ShortCircuitOn<T>()
		{
			return ShortCircuitOn(typeof(T).Name);
		}

		[Obsolete("Use ShortCircuitForStandard() instead.")]
		public InstallBuilder ShortCircuitOnStandard()
		{
			return ShortCircuitOn(nameof(StandardLevelScenesTransitionSetupDataSO));
		}

		[Obsolete("Use ShortCircuitForCampaign() instead.")]
		public InstallBuilder ShortCircuitOnCampaign()
		{
			return ShortCircuitOn(nameof(MissionLevelScenesTransitionSetupDataSO));
		}

		[Obsolete("Use ShortCircuitForTutorial() instead.")]
		public InstallBuilder ShortCircuitOnTutorial()
		{
			return ShortCircuitOn("Tutorial");
		}

		[Obsolete("Use ShortCircuitForMultiplayer() instead.")]
		public InstallBuilder ShortCircuitOnMultiplayer()
		{
			return ShortCircuitOn(nameof(MultiplayerLevelScenesTransitionSetupDataSO));
		}

		public InstallBuilder OnlyForStandard()
		{
			ShortCircuitForCampaign();
			ShortCircuitForTutorial();
			ShortCircuitForMultiplayer();
			return this;
		}

		public InstallBuilder Expose<T>()
		{
			Exposers.Add(typeof(T));
			return this;
		}

		public InstallBuilder Mutate<T>(Action<DiContainer, object> action)
		{
			if (action == null)
			{
				return this;
			}
			Mutators.Add(new Tuple<Type, Action<DiContainer, object>>(typeof(T), action));
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