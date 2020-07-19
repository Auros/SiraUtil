using System;
using Zenject;
using System.Collections.Generic;
using IPA.Utilities;
using UnityEngine;

namespace SiraUtil.Zenject
{
    public static class Installer
    {
        internal readonly static HashSet<Type> menuInstallers = new HashSet<Type>();
        internal readonly static HashSet<Type> gameCoreSceneSetupInstallers = new HashSet<Type>();
        internal readonly static HashSet<Type> gameplayCoreSceneSetupInstallers = new HashSet<Type>();
        internal static readonly PropertyAccessor<MonoInstallerBase, DiContainer>.Setter SetDiContainer = PropertyAccessor<MonoInstallerBase, DiContainer>.GetSetter("Container");
        internal static readonly PropertyAccessor<MonoInstallerBase, DiContainer>.Getter AccessDiContainer = PropertyAccessor<MonoInstallerBase, DiContainer>.GetGetter("Container");

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

        public static void InstallFromBase(MonoBehaviour source, HashSet<Type> monoInstallers)
        {
            // Convert the main installer to a MonoInstaller base 
            MonoInstallerBase monoInstaller = source as MonoInstallerBase;

            // Inject the mono installers
            foreach (Type t in monoInstallers)
            {
                // Create the mono installer's game object.
                MonoInstallerBase injectingInstallerBase = source.gameObject.AddComponent(t) as MonoInstallerBase;

                // Replace the container from the mod with the one from the source installer.
                SetDiContainer(ref injectingInstallerBase, AccessDiContainer(ref monoInstaller));

                // Force install their bindings with the source's DiContainer
                injectingInstallerBase.InstallBindings();
            }
        }

        /*internal readonly static HashSet<Type> globalInstallers = new HashSet<Type>();
        /// <summary>
        /// Installs a MonoInstaller globally. This MUST be done before the MenuViewControllers scene is loaded! (or before menu reloads when the player presses Ok in the settings)
        /// </summary>
        /// <typeparam name="T">The MonoInstaller that (you have created and) you want installed.</typeparam>
        public static void RegisterGlobally<T>() where T : MonoBehaviour
        {
            globalInstallers.Add(typeof(T));
        }

        /// <summary>
        /// Unregistes a global MonoInstaller that was installed using RegisterGlobally<T>(). The unregister comes in effect the next time the MenuViewControllers scene is loaded.
        /// </summary>
        /// <typeparam name="T">The MonoInstaller that (has already been installed and) needs to be uninstalled.</typeparam>
        public static void UnregisterGlobally<T>() where T : MonoBehaviour
        {
            globalInstallers.Remove(typeof(T));
        }
        */
    }
}