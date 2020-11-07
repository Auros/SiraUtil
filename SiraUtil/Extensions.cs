using HMUI;
using System;
using Zenject;
using Polyglot;
using UnityEngine;
using System.Linq;
using VRUIControls;
using IPA.Utilities;
using System.Reflection;
using SiraUtil.Services;
using SiraUtil.Interfaces;
using UnityEngine.EventSystems;

namespace SiraUtil
{
    public static class Extensions
    {
        /// <summary>
        /// Registers a logger as a SiraLogger, which can be then requested by Dependency Injection
        /// </summary>
        /// <param name="logger">The main logger to be used.</param>
        /// <param name="elevatedDebugMode">If this is true, any calls to .Debug will be redirected to .Info instead.</param>
        public static void BindLoggerAsSiraLogger(this DiContainer container, IPA.Logging.Logger logger, bool elevatedDebugMode = false)
        {
            var siraLogManager = container.Resolve<SiraLogManager>();
            siraLogManager.AddLogger(Assembly.GetCallingAssembly(), logger, elevatedDebugMode);
        }

        /// <summary>
        /// Upgrade a component to a type that inherits it.
        /// </summary>
        /// <typeparam name="T">The type of the component.</typeparam>
        /// <typeparam name="U">The type of the upgraded component.</typeparam>
        /// <param name="monoBehaviour">The original component.</param>
        /// <remarks>By putting an object into here you are permanently destroying it. Any and all references to this object need to be repaired.</remarks>
        /// <returns>The upgraded component.</returns>
        public static U Upgrade<T, U>(this T monoBehaviour) where U : T where T : MonoBehaviour 
        {
            var gameObject = monoBehaviour.gameObject;
            var upgradedDummyComponent = Activator.CreateInstance(typeof(U));
            foreach (FieldInfo info in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                info.SetValue(upgradedDummyComponent, info.GetValue(monoBehaviour));
            }
            UnityEngine.Object.DestroyImmediate(monoBehaviour);
            var upgradedMonoBehaviour = gameObject.AddComponent<U>();
            foreach (FieldInfo info in typeof(U).GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                info.SetValue(upgradedMonoBehaviour, info.GetValue(upgradedDummyComponent));
            }
            return upgradedMonoBehaviour;
        }

        /// <summary>
        /// Gets the localized key of a string from Polyglot, if not found then returns specified alternative.
        /// </summary>
        /// <param name="key">The key of the string in Polyglot.</param>
        /// <param name="or">The string to be used if the key could not be found.</param>
        /// <returns></returns>
        public static string LocalizationGetOr(this string key, string or)
        {
            var localized = Localization.Get(key);
            return string.IsNullOrWhiteSpace(localized) || key == localized ? or : localized;
        }

        /// <summary>
        /// Creates a new component on a new GameObject
        /// </summary>
        /// <param name="name">The name of the GameObject.</param>
        /// <returns></returns>
        public static ScopeConcreteIdArgConditionCopyNonLazyBinder FromNewComponentOnNewGameObject(this FromBinder binder, string name = "GameObject")
        {
            return binder.FromNewComponentOn(new GameObject(name));
        }

        /// <summary>
        /// Creates a new component on a new GameObject
        /// </summary>
        /// <param name="name">The name of the GameObject.</param>
        /// <returns></returns>
        public static ConditionCopyNonLazyBinder FromNewComponentOnNewGameObject(this FactoryFromBinderBase binder, string name = "GameObject")
        {
            return binder.FromNewComponentOn(new GameObject(name));
        }

        /// <summary>
        /// Binds a <see cref="ViewController"/> into the Container. This creates the view controller, repairs its dependencies, and adds it to the container.
        /// </summary>
        /// <typeparam name="T">The type of the ViewController.</typeparam>
        /// <param name="Container">The Container to install this ViewController into.</param>
        /// <param name="active">Whether or not to enable it after its binded.</param>
        public static void BindViewController<T>(this DiContainer Container, bool active = false) where T : ViewController
        {
            T vc = new GameObject(typeof(T).Name, typeof(VRGraphicRaycaster), typeof(CurvedCanvasSettings), typeof(CanvasGroup), typeof(T)).GetComponent<T>();
            var raycaster = Container.Resolve<PhysicsRaycasterWithCache>();
            vc.GetComponent<VRGraphicRaycaster>().SetField("_physicsRaycaster", raycaster);
            vc.rectTransform.anchorMin = new Vector2(0f, 0f);
            vc.rectTransform.anchorMax = new Vector2(1f, 1f);
            vc.rectTransform.sizeDelta = new Vector2(0f, 0f);
            vc.rectTransform.anchoredPosition = new Vector2(0f, 0f);
            vc.gameObject.SetActive(active);

            Container.QueueForInject(vc);
            Container.BindInstance(vc).AsSingle();
        }

        /// <summary>
        /// Binds a <see cref="FlowCoordinator"/> into the Container. This creates the flow coordinator, repairs its dependencies, and adds it to the container.
        /// </summary>
        /// <typeparam name="T">The type of the FlowCoordinator.</typeparam>
        /// <param name="Container">The Container to install this FlowCoordinator into.</param>
        public static void BindFlowCoordinator<T>(this DiContainer Container) where T : FlowCoordinator
        {
            var inputSystem = Container.Resolve<BaseInputModule>();
            T flowCoordinator = new GameObject(typeof(T).Name).AddComponent<T>();
            flowCoordinator.SetField<FlowCoordinator, BaseInputModule>("_baseInputModule", inputSystem);
            Container.QueueForInject(flowCoordinator);
            Container.BindInstance(flowCoordinator).AsSingle();
        }

        internal static void OverrideColor(this SetSaberGlowColor ssgc, Color color)
        {
            MeshRenderer mesh = Accessors.GlowMeshRenderer(ref ssgc);
            MaterialPropertyBlock block = Accessors.GlowMaterialPropertyBlock(ref ssgc);
            SetSaberGlowColor.PropertyTintColorPair[] tintPairs = Accessors.GlowPropertyTintPair(ref ssgc);
            if (block == null)
            {
                block = new MaterialPropertyBlock();
            }
            foreach (SetSaberGlowColor.PropertyTintColorPair ptcp in tintPairs)
            {
                block.SetColor(ptcp.property, color * ptcp.tintColor);
            }
            mesh.SetPropertyBlock(block);
        }

        internal static void OverrideColor(this SetSaberFakeGlowColor ssfgc, Color color)
        {
            Parametric3SliceSpriteController sliceSpriteController = Accessors.FakeGlowSliceSprite(ref ssfgc);
            sliceSpriteController.color = color * Accessors.FakeGlowTint(ref ssfgc);
            sliceSpriteController.Refresh();
        }

        /// <summary>
        /// Gets the color of a saber.
        /// </summary>
        /// <param name="saber">The saber to get the color of.</param>
        /// <returns></returns>
        public static Color GetColor(this Saber saber)
        {
            if (saber is IColorable saberColorable)
            {
                return saberColorable.Color;
            }
            SaberModelController modelController = saber.gameObject.GetComponentInChildren<SaberModelController>(true);
            if (modelController is IColorable)
            {
                var colorable = modelController as IColorable;
                return colorable.Color;
            }
            else if (modelController is SaberModelController smc)
            {
                return Accessors.TrailColor(ref Accessors.SaberTrail(ref smc)).gamma;
            }
            return Color.white;
        }


        /// <summary>
        /// Changes the color of a saber instantly.
        /// </summary>
        /// <param name="saber">The saber to change the color of.</param>
        /// <param name="color">The color to change the saber to.</param>
        public static void ChangeColorInstant(this Saber saber, Color color)
        {
            if (saber.isActiveAndEnabled)
            {
                saber.StartCoroutine(Utilities.ChangeColorCoroutine(saber, color, 0));
            }
        }

        /// <summary>
        /// Changes the color of a saber.
        /// </summary>
        /// <param name="saber">The saber to change the color of.</param>
        /// <param name="color">The color to change the saber to.</param>
        public static void ChangeColor(this Saber saber, Color color)
        {
            if (saber.isActiveAndEnabled)
            {
                saber.StartCoroutine(Utilities.ChangeColorCoroutine(saber, color));
            }
        }

        /// <summary>
        /// Change the color of a saber.
        /// </summary>
        /// <param name="color">The color to change the saber to.</param>
        /// <param name="smc">The model controller of the saber.</param>
        /// <param name="tintColor">The tint color of the new color.</param>
        /// <param name="setSaberGlowColors">The glow color groups of the saber.</param>
        /// <param name="setSaberFakeGlowColors">The fake glow color groups of the saber.</param>
        /// <param name="light">The light of the saber.</param>
        public static void ChangeColor(this Saber _, Color color, SaberModelController smc, Color tintColor, SetSaberGlowColor[] setSaberGlowColors, SetSaberFakeGlowColor[] setSaberFakeGlowColors, TubeBloomPrePassLight light)
        {
            Accessors.TrailColor(ref Accessors.SaberTrail(ref smc)) = (color * tintColor).linear;

            for (int i = 0; i < setSaberGlowColors.Length; i++)
            {
                setSaberGlowColors[i].OverrideColor(color);
            }
            for (int i = 0; i < setSaberFakeGlowColors.Length; i++)
            {
                setSaberFakeGlowColors[i].OverrideColor(color);
            }
            if (light != null)
            {
                light.color = color;
            }
        }

        /// <summary>
        /// Sets the type of a saber. This will change its color and the type of note it can hit.
        /// </summary>
        /// <param name="saber">The saber to change the type of.</param>
        /// <param name="type">The type to change the saber to.</param>
        /// <param name="colorManager">The color manager used to change the color of the saber.</param>
        public static void SetType(this Saber saber, SaberType type, ColorManager colorManager)
        {
            saber.ChangeType(type);
            saber.ChangeColor(colorManager.ColorForSaberType(type));
        }

        /// <summary>
        /// Changes the type of the saber. This does NOT change the color of the saber.
        /// </summary>
        /// <param name="saber">The saber to change the type of.</param>
        /// <param name="type">The type to change the saber to.</param>
        public static void ChangeType(this Saber saber, SaberType type)
        {
            saber.GetComponent<SaberTypeObject>().ChangeType(type);
        }

        /// <summary>
        /// Changes the type of a saber type object.
        /// </summary>
        /// <param name="sto">The SaberTypeObject.</param>
        /// <param name="type">The Saber Type</param>
        public static void ChangeType(this SaberTypeObject sto, SaberType type)
        {
            Accessors.ObjectSaberType(ref sto) = type;
        }

        /// <summary>
        /// Will log the nullity of an object.
        /// </summary>
        /// <param name="logger">The logger to log to.</param>
        /// <param name="toCheck">The object to check nullability.</param>
        public static void NullCheck(this IPA.Logging.Logger logger, object toCheck)
        {
            logger.Info(toCheck != null ? $"The {toCheck.GetType().Name} is not null." : $"The object is null");
        }

        internal static void Sira(this IPA.Logging.Logger logger, string message)
        {
            if (Environment.GetCommandLineArgs().Contains("--siralog"))
            {
                logger.Debug(message);
            }
        }

        /// <summary>
        /// Gets the event of an object.
        /// </summary>
        public static TDel GetEventHandlers<TTarget, TDel>(this TTarget target, string name)
        {
            return FieldAccessor<TTarget, TDel>.Get(target, name);
        }
    }
}