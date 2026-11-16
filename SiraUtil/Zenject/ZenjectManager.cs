using BGLib.AppFlow.Initialization;
using HarmonyLib;
using IPA.Loader;
using SiraUtil.Zenject.Internal;
using SiraUtil.Zenject.Internal.Instructors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Zenject;

namespace SiraUtil.Zenject
{
    internal class ZenjectManager
    {
        private readonly HashSet<ZenjectorDatum> _zenjectors = [];
        private readonly InstructorManager _instructorManager = new();

        internal void Add(Zenjector zenjector) => _zenjectors.Add(new ZenjectorDatum(zenjector));
        internal IEnumerable<Zenjector> ActiveZenjectors => _zenjectors.Where(z => z.Enabled).Select(z => z.Zenjector);
        internal Zenjector? ZenjectorFromAssembly(Assembly assembly) => _zenjectors.FirstOrDefault(z => z.Zenjector.Metadata.Assembly == assembly)?.Zenjector;

        private void PluginManager_PluginEnabled(PluginMetadata plugin, bool _)
        {
            // Enables the zenjector of a plugin being enabled.
            ZenjectorDatum? datum = _zenjectors.FirstOrDefault(zen => zen.Zenjector.Metadata == plugin);
            datum?.Enabled = true;
        }

        private void PluginManager_PluginDisabled(PluginMetadata plugin, bool _)
        {
            // Disables the zenjector of a plugin being disabled.
            ZenjectorDatum? datum = _zenjectors.FirstOrDefault(zen => zen.Zenjector.Metadata == plugin);
            datum?.Enabled = false;
        }

        public void Enable()
        {
            PluginManager.PluginEnabled += PluginManager_PluginEnabled;
            PluginManager.PluginDisabled += PluginManager_PluginDisabled;
            Harmony.ContextDecorator.ContextInstalling += ContextDecorator_ContextInstalling;
            Harmony.ContextDecorator.SceneBindingsInstalled += ContextDecorator_SceneBindingsInstalled;
            Harmony.ContextDecorator.AsyncSceneContextInstallersLoading += ContextDecorator_AsyncSceneContextInstallersLoading;

            // This will set the default state for every Zenjector when SiraUtil enables.
            foreach (ZenjectorDatum zenDatum in _zenjectors)
            {
                zenDatum.Enabled = PluginManager.GetPluginFromId(zenDatum.Zenjector.Metadata.Id) != null;
            }
        }

        private void ContextDecorator_ContextInstalling(Context context, IEnumerable<Type> installerBindings)
        {
            foreach (ZenjectorDatum zenDatum in _zenjectors)
            {
                if (!zenDatum.Enabled)
                {
                    continue;
                }

                Zenjector zenjector = zenDatum.Zenjector;

                // Install every normal install set.
                // AsyncInstallers were already added in LoadInstallersAsync, so skip them here.
                foreach (InstallSet set in zenjector.InstallSets.Where(i => !i.IsAsyncInstaller))
                {
                    if (set.InstallFilter.ShouldInstall(context, installerBindings))
                    {
                        Plugin.Log.Debug($"Installing: {set.InstallerType.FullName} onto '{context.name}' ({context.GetType().FullDescription()})");

                        IInstructor? instructor = _instructorManager.InstructorForSet(set);
                        if (instructor is null)
                        {
                            Plugin.Log.Warn($"Could not find instatiation instructor for the type {set.InstallerType}");
                            continue;
                        }
                        instructor.Install(set, context);
                    }
                }

                // Install every installerless binding set.
                foreach (InstallInstruction instruction in zenjector.InstallInstructions)
                {
                    if (instruction.InstallFilter.ShouldInstall(context, installerBindings))
                    {
                        instruction.OnInstall(context.Container);
                    }
                }
            }
        }

        private void ContextDecorator_SceneBindingsInstalled(Context context, List<MonoBehaviour> injectableMonoBehaviours)
        {
            foreach (IInjectableMonoBehaviourInstruction instruction in _zenjectors.SelectMany(zd => zd.Zenjector.InjectableMonoBehaviourInstructions).OrderBy(i => i.Order))
            {
                foreach (MonoBehaviour monoBehaviour in injectableMonoBehaviours)
                {
                    instruction.Apply(context, monoBehaviour);
                }
            }
        }

        private void ContextDecorator_AsyncSceneContextInstallersLoading(AsyncSceneContext asyncSceneContext, AsyncSceneContextLoadersQueue queue, IEnumerable<Type> installerBindings)
        {
            foreach (ZenjectorDatum zenDatum in _zenjectors.Where(z => z.Enabled))
            {
                // Install only AsyncInstaller-typed installer sets.
                foreach (InstallSet set in zenDatum.Zenjector.InstallSets.Where(i => i.IsAsyncInstaller))
                {
                    if (set.InstallFilter.ShouldInstall(asyncSceneContext, installerBindings))
                    {
                        Plugin.Log.Debug($"Adding {set.InstallerType.FullName} to async installers on '{asyncSceneContext.name}' ({asyncSceneContext.GetType().FullDescription()})");

                        // AsyncSceneContext.InstallInstallers takes care of injecting the behaviour
                        AsyncInstaller asyncInstaller = (AsyncInstaller)asyncSceneContext.gameObject.AddComponent(set.InstallerType);
                        queue._loadersQueue[(int)AsyncLoader.SynchronizationStep.AfterAsyncInstallers]._asyncLoaders.Add(asyncInstaller);
                    }
                }
            }
        }

        public void Disable()
        {
            Harmony.ContextDecorator.SceneBindingsInstalled -= ContextDecorator_SceneBindingsInstalled;
            Harmony.ContextDecorator.ContextInstalling -= ContextDecorator_ContextInstalling;
            Harmony.ContextDecorator.AsyncSceneContextInstallersLoading -= ContextDecorator_AsyncSceneContextInstallersLoading;
            PluginManager.PluginDisabled -= PluginManager_PluginDisabled;
            PluginManager.PluginEnabled -= PluginManager_PluginEnabled;
        }

        private class ZenjectorDatum
        {
            public bool Enabled { get; set; }
            public Zenjector Zenjector { get; }

            public ZenjectorDatum(Zenjector zenjector)
            {
                Zenjector = zenjector;
            }
        }
    }
}