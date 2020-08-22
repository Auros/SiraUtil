using System;
using Zenject;
using UnityEngine;
using IPA.Utilities;
using System.Collections.Generic;

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

        internal static readonly PropertyAccessor<MonoInstallerBase, DiContainer>.Setter SetDiContainer = PropertyAccessor<MonoInstallerBase, DiContainer>.GetSetter("Container");
        internal static readonly PropertyAccessor<MonoInstallerBase, DiContainer>.Getter AccessDiContainer = PropertyAccessor<MonoInstallerBase, DiContainer>.GetGetter("Container");

        public static void RegisterAppInstaller<T>() where T : MonoInstaller
        {
            appInstallers.Add(typeof(T));
        }

        public static void UnregisterAppInstaller<T>() where T : MonoInstaller
        {
            appInstallers.Remove(typeof(T));
        }

        public static void RegisterMenuInstaller<T>() where T : MonoInstaller
        {
            menuInstallers.Add(typeof(T));
        }

        public static void UnregisterMenuInstaller<T>() where T : MonoInstaller
        {
            menuInstallers.Remove(typeof(T));
        }

        public static void RegisterGameCoreInstaller<T>() where T : MonoInstaller
        {
            gameCoreSceneSetupInstallers.Add(typeof(T));
        }

        public static void UnregisterGameCoreInstaller<T>() where T : MonoInstaller
        {
            gameCoreSceneSetupInstallers.Remove(typeof(T));
        }

        public static void RegisterGameplayCoreInstaller<T>() where T : MonoInstaller
        {
            gameplayCoreSceneSetupInstallers.Add(typeof(T));
        }

        public static void UnregisterGameplayCoreInstaller<T>() where T : MonoInstaller
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

        internal static void InstallFromBase(MonoBehaviour source, HashSet<Type> monoInstallers, HashSet<ISiraInstaller> siraInstallers)
        {
            // Convert the main installer to a MonoInstaller base 
            MonoInstallerBase monoInstaller = source as MonoInstallerBase;

            // Store its DiContainer reference
            DiContainer container = AccessDiContainer(ref monoInstaller);

            // Inject the mono installers
            foreach (Type t in monoInstallers)
            {
                // Create the mono installer's game object.
                MonoInstallerBase injectingInstallerBase = source.gameObject.AddComponent(t) as MonoInstallerBase;

                // Replace the container from the mod with the one from the source installer.
                SetDiContainer(ref injectingInstallerBase, container);

                // Force install their bindings with the source's DiContainer
                injectingInstallerBase.InstallBindings();
            }

            foreach (ISiraInstaller si in siraInstallers)
            {
                si.Install(container, source.gameObject);
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