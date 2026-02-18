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
        public static bool InitialSceneConstructionRegistered { get; private set; }
        private const string _initialContextName = "AppCoreSceneContext";

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
            Harmony.ContextDecorator.InstalledSceneBindings += ContextDecorator_InstalledSceneBindings;

            // This will set the default state for every Zenjector when SiraUtil enables.
            foreach (ZenjectorDatum zenDatum in _zenjectors)
            {
                zenDatum.Enabled = PluginManager.GetPluginFromId(zenDatum.Zenjector.Metadata.Id) != null;
            }
        }

        private void ContextDecorator_ContextInstalling(Context context, IEnumerable<Type> installerBindings)
        {
            if (context.name == _initialContextName)
            {
                InitialSceneConstructionRegistered = true;
            }

            if (!InitialSceneConstructionRegistered)
            {
                return;
            }

            foreach (ZenjectorDatum zenDatum in _zenjectors)
            {
                if (!zenDatum.Enabled)
                {
                    continue;
                }

                Zenjector zenjector = zenDatum.Zenjector;

                // Install every normal install set.
                foreach (InstallSet set in zenjector.InstallSets)
                {
                    if (set.installFilter.ShouldInstall(context, installerBindings))
                    {
                        Plugin.Log.Debug($"Installing: {set.installerType.FullName} onto '{context.name}' ({context.GetType().FullDescription()})");
                        IInstructor? instructor = _instructorManager.InstructorForSet(set);
                        if (instructor is null)
                        {
                            Plugin.Log.Warn($"Could not find instatiation instructor for the type {set.installerType}");
                            continue;
                        }
                        instructor.Install(set, context);
                    }
                }

                // Install every installerless binding set.
                foreach (InstallInstruction instruction in zenjector.InstallInstructions)
                {
                    if (instruction.installFilter.ShouldInstall(context, installerBindings))
                    {
                        instruction.onInstall(context.Container);
                    }
                }
            }
        }

        private void ContextDecorator_InstalledSceneBindings(Context context, List<MonoBehaviour> injectableMonoBehaviours)
        {
            foreach (IInjectableMonoBehaviourInstruction instruction in _zenjectors.SelectMany(zd => zd.Zenjector.InjectableMonoBehaviourInstructions).OrderBy(i => i.Order))
            {
                foreach (MonoBehaviour monoBehaviour in injectableMonoBehaviours)
                {
                    instruction.Apply(context, monoBehaviour);
                }
            }
        }

        public void Disable()
        {
            Harmony.ContextDecorator.InstalledSceneBindings -= ContextDecorator_InstalledSceneBindings;
            Harmony.ContextDecorator.ContextInstalling -= ContextDecorator_ContextInstalling;
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