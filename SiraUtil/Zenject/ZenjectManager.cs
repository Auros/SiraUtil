using Zenject;
using ModestTree;
using IPA.Loader;
using System.Linq;
using SiraUtil.Events;
using System.Collections.Generic;
using UnityEngine;
using IPA.Utilities;

namespace SiraUtil.Zenject
{
    internal class ZenjectManager
    {
        internal static bool ProjectContextWentOff { get; set; } = false;
        private readonly IDictionary<string, Zenjector> _allZenjectors = new Dictionary<string, Zenjector>();

        public ZenjectManager()
        {
            SiraEvents.ContextInstalling += SiraEvents_PreInstall;
            PluginManager.PluginEnabled += PluginManager_PluginEnabled;
            PluginManager.PluginDisabled += PluginManager_PluginDisabled;
        }

        private void PluginManager_PluginEnabled(PluginMetadata plugin, bool _)
        {
            if (_allZenjectors.TryGetValue(plugin.Id, out Zenjector zenjector) && zenjector.AutoControl)
            {
                zenjector.Enable();
            }
        }

        private void PluginManager_PluginDisabled(PluginMetadata plugin, bool _)
        {
            if (_allZenjectors.TryGetValue(plugin.Id, out Zenjector zenjector) && zenjector.AutoControl)
            {
                zenjector.Disable();
            }
        }

        internal void Add(Zenjector zenjector)
        {
            if (!_allZenjectors.ContainsKey(zenjector.Name))
            {
                _allZenjectors.Add(zenjector.Name, zenjector);
            }
        }

		#region Events

		private void SiraEvents_PreInstall(object sender, SiraEvents.SceneContextInstalledArgs e)
		{
			if (!ProjectContextWentOff)
			{
				if (e.Name == "AppCore") // AppCore is the first reported context.
				{
					ProjectContextWentOff = true;
				}
				else
				{
					return;
				}
			}
			var context = sender as SceneContext;
			var builders = _allZenjectors.Values.Where(x => x.Enabled).SelectMany(x => x.Builders).Where(x => x.Destination == e.Name && !x.Circuits.Contains(e.Name) && !x.Circuits.Contains(e.ModeInfo.Transition) && !x.Circuits.Contains(e.ModeInfo.Gamemode) && !x.Circuits.Contains(e.ModeInfo.MidScene)).ToList();

			builders.ForEach(x =>
			{
				x.Validate();

				x.Circuits.ForEach(y =>
				{
					Plugin.Log.Info(x.Type.FullName);
					Plugin.Log.Info(y);
					Plugin.Log.Info(e.Name);
					Plugin.Log.Info(e.ModeInfo.Gamemode);
					Plugin.Log.Info(e.ModeInfo.Transition);
					Plugin.Log.Info(e.ModeInfo.MidScene);
				});
			});

			// Expose injectables from decorators if requested. @Caeden
			var allInjectables = e.Decorators.SelectMany(x => x.GetField<List<MonoBehaviour>, SceneDecoratorContext>("_injectableMonoBehaviours"));

			for (int i = 0; i < builders.Count(); i++)
			{
				var toExpose = builders[i].Exposers;
				for (int d = 0; d < toExpose.Count; d++)
				{
					var exposableType = toExpose.ElementAt(d);
					var behaviour = allInjectables.FirstOrDefault(x => x.GetType() == exposableType);
					Assert.IsNotNull(behaviour, $"Could not find an object to expose in a decorator context. {Utilities.ASSERTHIT}", exposableType);
					if (!e.Container.HasBinding(behaviour.GetType()))
					{
						e.Container.Bind(exposableType).FromInstance(behaviour).AsSingle();
						break;
					}
				}
			}
			
            // Handle Parameters (Manually Installed)
            var parameterBased = builders.Where(x => x.Parameters != null && x.Parameters.Length > 0);
            var bases = context.NormalInstallers.ToList();
            for (int i = 0; i < parameterBased.Count(); i++)
            {
                var paramBuilder = parameterBased.ElementAt(i);

                // Configurable Mono Installers requires the Unity Inspector
                Assert.That(!paramBuilder.Type.DerivesFrom<MonoInstallerBase>(), $"MonoInstallers cannot have parameters due to Zenject limitations. {Utilities.ASSERTHIT}");

                bases.Add(e.Container.Instantiate(paramBuilder.Type, paramBuilder.Parameters) as InstallerBase);
            }
            context.NormalInstallers = bases;

            // Create Mono Installers
            var monoInstallers = context.Installers.ToList();
            var monos = builders.Where(x => x.Type.IsSubclassOf(typeof(MonoInstallerBase)));
            for (int i = 0; i < monos.Count(); i++)
            {
                monoInstallers.Add(context.gameObject.AddComponent(monos.ElementAt(i).Type) as MonoInstaller);
            }
            context.Installers = monoInstallers;

            // Add Normal Install Types
            builders.Where(x => x.Type.IsSubclassOf(typeof(InstallerBase)) && (x.Parameters == null || x.Parameters.Length == 0))
                .ToList().ForEach(x =>
                    context.AddNormalInstallerType(x.Type)
            );
        }

        #endregion

        ~ZenjectManager()
        {
            SiraEvents.ContextInstalling -= SiraEvents_PreInstall;
            PluginManager.PluginEnabled -= PluginManager_PluginEnabled;
            PluginManager.PluginDisabled -= PluginManager_PluginDisabled;
        }
    }
}