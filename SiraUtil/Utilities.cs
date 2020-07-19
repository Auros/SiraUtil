using Xft;
using UnityEngine;
using IPA.Utilities;
using System.Collections.Generic;
using System.Collections;
using HarmonyLib;
using System.Reflection.Emit;
using SiraUtil.Sabers;

namespace SiraUtil
{
    public static class Utilities
    {
        internal static FieldAccessor<BasicSaberModelController, Light>.Accessor SaberLight = FieldAccessor<BasicSaberModelController, Light>.GetAccessor("_light");
        internal static FieldAccessor<BasicSaberModelController, XWeaponTrail>.Accessor SaberTrail = FieldAccessor<BasicSaberModelController, XWeaponTrail>.GetAccessor("_saberWeaponTrail");
        internal static FieldAccessor<BasicSaberModelController, SetSaberGlowColor[]>.Accessor SaberGlowColor = FieldAccessor<BasicSaberModelController, SetSaberGlowColor[]>.GetAccessor("_setSaberGlowColors");
        internal static FieldAccessor<BasicSaberModelController, SetSaberFakeGlowColor[]>.Accessor FakeSaberGlowColor = FieldAccessor<BasicSaberModelController, SetSaberFakeGlowColor[]>.GetAccessor("_setSaberFakeGlowColors");
        internal static FieldAccessor<BasicSaberModelController, BasicSaberModelController.InitData>.Accessor ModelInitData = FieldAccessor<BasicSaberModelController, BasicSaberModelController.InitData>.GetAccessor("_initData");

        internal static FieldAccessor<SetSaberGlowColor, MeshRenderer>.Accessor GlowMeshRenderer = FieldAccessor<SetSaberGlowColor, MeshRenderer>.GetAccessor("_meshRenderer");
        internal static FieldAccessor<SetSaberGlowColor, MaterialPropertyBlock>.Accessor GlowMaterialPropertyBlock = FieldAccessor<SetSaberGlowColor, MaterialPropertyBlock>.GetAccessor("_materialPropertyBlock");
        internal static FieldAccessor<SetSaberGlowColor, SetSaberGlowColor.PropertyTintColorPair[]>.Accessor GlowPropertyTintPair = FieldAccessor<SetSaberGlowColor, SetSaberGlowColor.PropertyTintColorPair[]>.GetAccessor("_propertyTintColorPairs");

        internal static FieldAccessor<SetSaberFakeGlowColor, Parametric3SliceSpriteController>.Accessor FakeGlowSliceSprite = FieldAccessor<SetSaberFakeGlowColor, Parametric3SliceSpriteController>.GetAccessor("_parametric3SliceSprite");
        internal static FieldAccessor<SetSaberFakeGlowColor, Color>.Accessor FakeGlowTint = FieldAccessor<SetSaberFakeGlowColor, Color>.GetAccessor("_tintColor");

        internal static FieldAccessor<SaberTypeObject, SaberType>.Accessor ObjectSaberType = FieldAccessor<SaberTypeObject, SaberType>.GetAccessor("_saberType");
        internal static FieldAccessor<Saber, SaberTypeObject>.Accessor SaberObjectType = FieldAccessor<Saber, SaberTypeObject>.GetAccessor("_saberType");

        internal static FieldAccessor<Saber, Transform>.Accessor TopPos = FieldAccessor<Saber, Transform>.GetAccessor("_topPos");
        internal static FieldAccessor<Saber, Transform>.Accessor BottomPos = FieldAccessor<Saber, Transform>.GetAccessor("_bottomPos");
        internal static FieldAccessor<Saber, Transform>.Accessor HandlePos = FieldAccessor<Saber, Transform>.GetAccessor("_handlePos");
        internal static FieldAccessor<Saber, List<SaberSwingRatingCounter>>.Accessor SwingRatingCounters = FieldAccessor<Saber, List<SaberSwingRatingCounter>>.GetAccessor("_swingRatingCounters");
        internal static FieldAccessor<Saber, List<SaberSwingRatingCounter>>.Accessor UnusedSwingRatingCounters = FieldAccessor<Saber, List<SaberSwingRatingCounter>>.GetAccessor("_unusedSwingRatingCounters");
        internal static FieldAccessor<Saber, SaberMovementData>.Accessor MovementData = FieldAccessor<Saber, SaberMovementData>.GetAccessor("_movementData");
        internal static FieldAccessor<Saber, float>.Accessor Time = FieldAccessor<Saber, float>.GetAccessor("_time");
        internal static FieldAccessor<Saber, Cutter>.Accessor Cutter = FieldAccessor<Saber, Cutter>.GetAccessor("_cutter");

        internal static void OverrideColor(this SetSaberGlowColor ssgc, Color color)
        {
            MeshRenderer mesh = GlowMeshRenderer(ref ssgc);
            MaterialPropertyBlock block = GlowMaterialPropertyBlock(ref ssgc);
            SetSaberGlowColor.PropertyTintColorPair[] tintPairs = GlowPropertyTintPair(ref ssgc);

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
            Parametric3SliceSpriteController sliceSpriteController = FakeGlowSliceSprite(ref ssfgc);
            sliceSpriteController.color = color * FakeGlowTint(ref ssfgc);
            sliceSpriteController.Refresh();
        }

        public static Color GetColor(this Saber saber)
        {
            ISaberModelController modelController = saber.gameObject.GetComponentInChildren<ISaberModelController>(true);
            if (modelController is BasicSaberModelController)
            {
                BasicSaberModelController bsmc = modelController as BasicSaberModelController;
                Light light = SaberLight(ref bsmc);
                return light.color;
            }
            else if (modelController is IColorable)
            {
                IColorable colorable = modelController as IColorable;
                return colorable.Color;
            }
            return Color.white;
        }

        public static void ChangeColor(this Saber saber, Color color)
        {
            saber.StartCoroutine(ChangeColorCoroutine(saber, color));
        }

        private static IEnumerator ChangeColorCoroutine(Saber saber, Color color)
        {
            yield return new WaitForSeconds(0.05f);
            ISaberModelController modelController = saber.gameObject.GetComponentInChildren<MonoBehaviourSaberModelController>(true);
            if (modelController == null)
                Plugin.Log.Info("It's null.");
            if (modelController is BasicSaberModelController)
            {
                BasicSaberModelController bsmc = modelController as BasicSaberModelController;
                Color tintColor = ModelInitData(ref bsmc).trailTintColor;
                SetSaberGlowColor[] setSaberGlowColors = SaberGlowColor(ref bsmc);
                SetSaberFakeGlowColor[] setSaberFakeGlowColors = FakeSaberGlowColor(ref bsmc);
                Light light = SaberLight(ref bsmc);
                saber.ChangeColor(color, bsmc, tintColor, setSaberGlowColors, setSaberFakeGlowColors, light);
            }
            else if (modelController is IColorable)
            {

                IColorable colorable = modelController as IColorable;
                colorable.SetColor(color);
            }
        }

        public static void ChangeColor(this Saber _, Color color, BasicSaberModelController bsmc, Color tintColor, SetSaberGlowColor[] setSaberGlowColors, SetSaberFakeGlowColor[] setSaberFakeGlowColors, Light light)
        {
            SaberTrail(ref bsmc).color = (color * tintColor).linear;

            for (int i = 0; i < setSaberGlowColors.Length; i++)
            {
                setSaberGlowColors[i].OverrideColor(color);
            }
            for (int i = 0; i < setSaberFakeGlowColors.Length; i++)
            {
                setSaberFakeGlowColors[i].OverrideColor(color);
            }
            light.color = color;
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
            ObjectSaberType(ref sto) = type;
        }

        /// <summary>
        /// Check if the following code instructions starting from a given index match a list of opcodes.
        /// </summary>
        /// <param name="codes">A list of code instructions to check</param>
        /// <param name="toCheck">A list of op codes that is expected to match</param>
        /// <param name="startIndex">Index to start checking from (inclusive)</param>
        /// <returns>Whether or not the op codes found in the code instructions match.</returns>
        internal static bool OpCodeSequence(List<CodeInstruction> codes, List<OpCode> toCheck, int startIndex)
        {
            for (int i = 0; i < toCheck.Count; i++)
            {
                if (codes[startIndex + i].opcode != toCheck[i]) return false;
            }
            return true;
        }
    }
}