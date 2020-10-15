using HMUI;
using System;
using Zenject;
using UnityEngine;
using System.Linq;
using IPA.Utilities;
using SiraUtil.Interfaces;
using VRUIControls;
using UnityEngine.EventSystems;

namespace SiraUtil
{
	public static class Extensions
    {
        public static ScopeConcreteIdArgConditionCopyNonLazyBinder FromNewComponentOnNewGameObject(this FromBinder binder, string name = "GameObject")
        {
            return binder.FromNewComponentOn(new GameObject(name));
        }

        public static ConditionCopyNonLazyBinder FromNewComponentOnNewGameObject(this FactoryFromBinderBase binder, string name = "GameObject")
        {
            return binder.FromNewComponentOn(new GameObject(name));
        }

		public static void BindViewController<T>(this DiContainer Container, ViewController viewController) where T : ViewController
		{
			var raycaster = Container.Resolve<PhysicsRaycasterWithCache>();
			viewController.GetComponent<VRGraphicRaycaster>().SetField("_physicsRaycaster", raycaster);
			Container.QueueForInject(viewController);
			Container.BindInstance(viewController as T).AsCached();

			viewController.rectTransform.anchorMin = new Vector2(0f, 0f);
			viewController.rectTransform.anchorMax = new Vector2(1f, 1f);
			viewController.rectTransform.sizeDelta = new Vector2(0f, 0f);
			viewController.rectTransform.anchoredPosition = new Vector2(0f, 0f);
		}

		public static void BindFlowCoordinator<T>(this DiContainer Container, FlowCoordinator flowCoordinator) where T : FlowCoordinator
		{
			var inputSystem = Container.Resolve<BaseInputModule>();
			flowCoordinator.SetField("_baseInputModule", inputSystem);
			Container.QueueForInject(flowCoordinator);
			Container.BindInstance(flowCoordinator as T).AsCached();
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

        public static Color GetColor(this Saber saber)
        {
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

        public static void ChangeColorInstant(this Saber saber, Color color)
        {
            if (saber.isActiveAndEnabled)
            {
                saber.StartCoroutine(Utilities.ChangeColorCoroutine(saber, color, 0));
            }
        }

        public static void ChangeColor(this Saber saber, Color color)
        {
            if (saber.isActiveAndEnabled)
            {
                saber.StartCoroutine(Utilities.ChangeColorCoroutine(saber, color));
            }
        }

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

        public static void SetType(this Saber saber, SaberType type, ColorManager colorManager)
        {
            saber.ChangeType(type);
            saber.ChangeColor(colorManager.ColorForSaberType(type));
        }

        public static void ChangeType(this Saber saber, SaberType type)
        {
            saber.GetComponent<SaberTypeObject>().ChangeType(type);
        }

        public static void ChangeType(this SaberTypeObject sto, SaberType type)
        {
            Accessors.ObjectSaberType(ref sto) = type;
        }

        public static void NullCheck(this IPA.Logging.Logger logger, object toCheck)
        {
            logger.Info(toCheck != null ? $"{toCheck.GetType().Name} is not null." : $"{toCheck.GetType().FullName} is null");
        }

        internal static void Sira(this IPA.Logging.Logger logger, string message)
        {
            if (Environment.GetCommandLineArgs().Contains("--siralog"))
            {
                logger.Debug(message);
            }
        }

        public static TDel GetEventHandlers<TTarget, TDel>(this TTarget target, string name)
        {
            return FieldAccessor<TTarget, TDel>.Get(target, name);
        }
	}
}