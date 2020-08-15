using System;
using Zenject;
using System.Linq;
using UnityEngine;
using IPA.Utilities;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace SiraUtil.Zenject
{
    public static class Installer
    {
        internal readonly static HashSet<Type> appInstallers = new HashSet<Type>();
        internal readonly static HashSet<Type> menuInstallers = new HashSet<Type>();
        internal readonly static HashSet<Type> gameCoreSceneSetupInstallers = new HashSet<Type>();
        internal readonly static HashSet<Type> gameplayCoreSceneSetupInstallers = new HashSet<Type>();
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

        /*public static void ContextInject(object obj, string sceneName = "PCInit", bool bind = false)
        {
            GetSceneContextAsync(ContextAction, sceneName, obj, bind);
        }

        private static void ContextAction(SceneContext context, object obj, bool bind)
        {
            if (bind)
            {
                context.Container.BindInstance(obj);
            }
            context.Container.Inject(obj);
        }*/

        internal static void InstallFromBase(MonoBehaviour source, HashSet<Type> monoInstallers)
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

        public static void GetSceneContextAsync(Action<SceneContext> contextInstalled, string sceneName)
        {
            if (contextInstalled == null) throw new ArgumentNullException(nameof(contextInstalled));
            if (string.IsNullOrEmpty(sceneName)) throw new ArgumentNullException(nameof(sceneName));
            if (!SceneManager.GetSceneByName(sceneName).isLoaded) throw new Exception($"Scene '{sceneName}' is not loaded");
            List<SceneContext> sceneContexts = Resources.FindObjectsOfTypeAll<SceneContext>().Where(sc => sc.gameObject.scene.name == sceneName).ToList();
            if (sceneContexts.Count == 0)
            {
                throw new Exception($"Scene context not found in scene '{sceneName}'");
            }
            if (sceneContexts.Count > 1)
            {
                throw new Exception($"More than one scene context found in scene '{sceneName}'");
            }
            SceneContext sceneContext = sceneContexts[0];
            if (sceneContext.HasInstalled)
            {
                contextInstalled(sceneContext);
            }
            else
            {
                sceneContext.OnPostInstall.AddListener(() => contextInstalled(sceneContext));
            }
        }

        /*public static void GetSceneContextAsync(Action<SceneContext, object, bool> contextInstalled, string sceneName, object obj, bool bind = false)
        {
            if (contextInstalled == null) throw new ArgumentNullException(nameof(contextInstalled));
            if (string.IsNullOrEmpty(sceneName)) throw new ArgumentNullException(nameof(sceneName));
            if (!SceneManager.GetSceneByName(sceneName).isLoaded) throw new Exception($"Scene '{sceneName}' is not loaded");
            List<SceneContext> sceneContexts = Resources.FindObjectsOfTypeAll<SceneContext>().Where(sc => sc.gameObject.scene.name == sceneName).ToList();
            if (sceneContexts.Count == 0)
            {
                throw new Exception($"Scene context not found in scene '{sceneName}'");
            }
            if (sceneContexts.Count > 1)
            {
                throw new Exception($"More than one scene context found in scene '{sceneName}'");
            }
            SceneContext sceneContext = sceneContexts[0];
            if (sceneContext.HasInstalled)
            {
                contextInstalled(sceneContext, obj, bind);
            }
            else
            {
                sceneContext.OnPostInstall.AddListener(() => contextInstalled(sceneContext, obj, bind));
            }
        }*/

        public static void InjectSpecialInstance<T>(this DiContainer Container, Component controller)
        {
            Container.BindInstance((T)(object)controller).AsSingle().NonLazy();
            Container.InjectGameObject(controller.gameObject);
        }
    }
}