using System;
using Zenject;
using UnityEngine;
using System.Linq;
using IPA.Utilities;
using SiraUtil.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;

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