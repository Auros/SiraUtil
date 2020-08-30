using System;
using Zenject;
using UnityEngine;
using IPA.Utilities;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace SiraUtil.Zenject
{
    public static class Installer
    {
        internal readonly static HashSet<Type> appInstallers = new HashSet<Type>();
        internal readonly static HashSet<Type> menuInstallers = new HashSet<Type>();
        internal readonly static HashSet<Type> gameCoreSceneSetupInstallers = new HashSet<Type>();
        internal readonly static HashSet<Type> gameplayCoreSceneSetupInstallers = new HashSet<Type>();

        internal readonly static HashSet<ISiraInstaller> appSiraInstallers = new HashSet<ISiraInstaller>();
        internal readonly static HashSet<ISiraInstaller> menuSiraInstallers = new HashSet<ISiraInstaller>();
        internal readonly static HashSet<ISiraInstaller> gameCoreSiraInstallers = new HashSet<ISiraInstaller>();
        internal readonly static HashSet<ISiraInstaller> gameplayCoreSiraInstallers = new HashSet<ISiraInstaller>();

        private readonly static HashSet<Type> _installedInstallers = new HashSet<Type>();

        internal static readonly PropertyAccessor<MonoInstallerBase, DiContainer>.Setter SetDiContainer = PropertyAccessor<MonoInstallerBase, DiContainer>.GetSetter("Container");
        internal static readonly PropertyAccessor<MonoInstallerBase, DiContainer>.Getter AccessDiContainer = PropertyAccessor<MonoInstallerBase, DiContainer>.GetGetter("Container");
        private static readonly MethodInfo _installMethod = typeof(DiContainer).GetMethod("Install", BindingFlags.Public | BindingFlags.Instance, null, new Type[0], null);

        internal static bool NotAllAppInstallersAreInstalled => appInstallers.Count + appSiraInstallers.Count != _installedInstallers.Count;

        public static void RegisterAppInstaller<T>() where T : IInstaller
        {
            appInstallers.Add(typeof(T));
        }

        public static void UnregisterAppInstaller<T>() where T : IInstaller
        {
            appInstallers.Remove(typeof(T));
            _installedInstallers.Remove(typeof(T));
        }

        public static void RegisterMenuInstaller<T>() where T : IInstaller
        {
            menuInstallers.Add(typeof(T));
        }

        public static void UnregisterMenuInstaller<T>() where T : IInstaller
        {
            menuInstallers.Remove(typeof(T));
        }

        public static void RegisterGameCoreInstaller<T>() where T : IInstaller
        {
            gameCoreSceneSetupInstallers.Add(typeof(T));
        }

        public static void UnregisterGameCoreInstaller<T>() where T : IInstaller
        {
            gameCoreSceneSetupInstallers.Remove(typeof(T));
        }

        public static void RegisterGameplayCoreInstaller<T>() where T : IInstaller
        {
            gameplayCoreSceneSetupInstallers.Add(typeof(T));
        }

        public static void UnregisterGameplayCoreInstaller<T>() where T : IInstaller
        {
            gameplayCoreSceneSetupInstallers.Remove(typeof(T));
        }

        public static void RegisterAppInstaller(ISiraInstaller installer)
        {
            appSiraInstallers.Add(installer);
        }

        public static void UnregisterAppInstaller(ISiraInstaller installer)
        {
            appSiraInstallers.Remove(installer);
            _installedInstallers.Remove(installer.GetType());
        }

        public static void RegisterMenuInstaller(ISiraInstaller installer)
        {
            menuSiraInstallers.Add(installer);
        }

        public static void UnregisterMenuInstaller(ISiraInstaller installer)
        {
            menuSiraInstallers.Remove(installer);
        }

        public static void RegisterGameCoreInstaller(ISiraInstaller installer)
        {
            gameCoreSiraInstallers.Add(installer);
        }

        public static void UnregisterGameCoreInstaller(ISiraInstaller installer)
        {
            gameCoreSiraInstallers.Remove(installer);
        }

        public static void RegisterGameplayCoreInstaller(ISiraInstaller installer)
        {
            gameplayCoreSiraInstallers.Add(installer);
        }

        public static void UnregisterGameplayCoreInstaller(ISiraInstaller installer)
        {
            gameplayCoreSiraInstallers.Remove(installer);
        }

        internal static void InstallFromBase(MonoBehaviour source, HashSet<Type> installers, HashSet<ISiraInstaller> siraInstallers, bool isApp = false)
        {
            // Convert the main installer to a MonoInstaller base 
            MonoInstallerBase monoInstaller = source as MonoInstallerBase;

            // Store its DiContainer reference
            DiContainer container = AccessDiContainer(ref monoInstaller);

            // Inject the mono installers
            foreach (Type t in installers)
            {
                var attr = t.GetCustomAttribute<RequiresInstallerAttribute>();
                if (attr != null && attr is RequiresInstallerAttribute requireInstallerAttr)
                {
                    if (!_installedInstallers.Contains(requireInstallerAttr.RequiredInstaller))
                    {
                        return;
                    }
                }
                if (t.IsSubclassOf(typeof(MonoBehaviour)))
                {
                    // Create the mono installer's game object.
                    MonoInstallerBase injectingInstallerBase = source.gameObject.AddComponent(t) as MonoInstallerBase;

                    // Replace the container from the mod with the one from the source installer.
                    SetDiContainer(ref injectingInstallerBase, container);

                    // Force install their bindings with the source's DiContainer
                    injectingInstallerBase.InstallBindings();

                }
                else
                {
                    _installMethod.MakeGenericMethod(t).Invoke(container, null);
                }
                if (isApp && !_installedInstallers.Contains(t))
                {
                    _installedInstallers.Add(t);
                }
            }

            foreach (ISiraInstaller si in siraInstallers)
            {
                var attr = si.GetType().GetCustomAttribute<RequiresInstallerAttribute>();
                if (attr != null && attr is RequiresInstallerAttribute requireInstallerAttr)
                {
                    if (!_installedInstallers.Contains(requireInstallerAttr.RequiredInstaller))
                    {
                        return;
                    }
                }
                si.Install(container, source.gameObject);
                if (isApp)
                {
                    _installedInstallers.Add(si.GetType());
                }
            }
        }

        public static void InjectSpecialInstance<T>(this DiContainer Container, Component controller)
        {
            Container.BindInstance((T)(object)controller).AsSingle().NonLazy();
            Container.InjectGameObject(controller.gameObject);
        }

        public static void ForceBindComponent<T>(this DiContainer Container, Component controller)
            => InjectSpecialInstance<T>(Container, controller);
    }
}