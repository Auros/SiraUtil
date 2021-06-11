using IPA.Utilities;
using SiraUtil.Interfaces;
using UnityEngine;

namespace SiraUtil.Extras
{
    internal static class SaberExtensions
    {
        private static readonly FieldAccessor<SaberTrail, Color>.Accessor SaberTrail_Color = FieldAccessor<SaberTrail, Color>.GetAccessor("_color");
        private static readonly FieldAccessor<SaberTrail, Color>.Accessor SaberTrail_ForcedColor = FieldAccessor<SaberTrail, Color>.GetAccessor("_forcedColor");
        private static readonly FieldAccessor<SaberTrail, bool>.Accessor SaberTrail_ColorOverwrite = FieldAccessor<SaberTrail, bool>.GetAccessor("_colorOverwrite");
        internal static readonly FieldAccessor<SaberModelController, SaberTrail>.Accessor SaberModelController_SaberTrail = FieldAccessor<SaberModelController, SaberTrail>.GetAccessor("_saberTrail");
        internal static readonly FieldAccessor<SaberModelController, TubeBloomPrePassLight>.Accessor SaberModelController_SaberLight = FieldAccessor<SaberModelController, TubeBloomPrePassLight>.GetAccessor("_saberLight");
        internal static readonly FieldAccessor<SaberModelController, SetSaberGlowColor[]>.Accessor SaberModelController_SetSaberGlowColors = FieldAccessor<SaberModelController, SetSaberGlowColor[]>.GetAccessor("_setSaberGlowColors");
        internal static readonly FieldAccessor<SaberModelController, SetSaberFakeGlowColor[]>.Accessor SaberModelController_SetSaberFakeGlowColors = FieldAccessor<SaberModelController, SetSaberFakeGlowColor[]>.GetAccessor("_setSaberFakeGlowColors");

        internal static Color GetColor(this SaberModelController saberModelController)
        {
            if (saberModelController is IColorable colorable)
                return colorable.Color;
            return SaberTrail_Color(ref SaberModelController_SaberTrail(ref saberModelController));
        }

        internal static void SetColor(this SaberModelController saberModelController, Color color)
        {
            SaberTrail trail = SaberModelController_SaberTrail(ref saberModelController);
            TubeBloomPrePassLight? light = SaberModelController_SaberLight(ref saberModelController);
            SetSaberGlowColor[] glowColors = SaberModelController_SetSaberGlowColors(ref saberModelController);
            SetSaberFakeGlowColor[] fakeGlowColors = SaberModelController_SetSaberFakeGlowColors(ref saberModelController);

            float beforeAlpha = SaberTrail_Color(ref trail).a;
            SaberTrail_Color(ref trail) = SaberTrail_ColorOverwrite(ref trail) ? SaberTrail_ForcedColor(ref trail) : color.ColorWithAlpha(beforeAlpha);
            foreach (var glow in glowColors)
                glow.SetColors(color);
            foreach (var fakeGlow in fakeGlowColors)
                fakeGlow.SetColors(color);
            if (light != null)
                light.color = color;
        }

        internal static SetSaberGlowColor[] SaberGlowColors(this SaberModelController saberModelController)
            => SaberModelController_SetSaberGlowColors(ref saberModelController);
        internal static SetSaberFakeGlowColor[] SaberFakeGlowColors(this SaberModelController saberModelController)
            => SaberModelController_SetSaberFakeGlowColors(ref saberModelController);

        private static readonly FieldAccessor<SetSaberGlowColor, MeshRenderer>.Accessor SetSaberGlowColor_MeshRenderer = FieldAccessor<SetSaberGlowColor, MeshRenderer>.GetAccessor("_meshRenderer"); 
        private static readonly FieldAccessor<SetSaberGlowColor, MaterialPropertyBlock>.Accessor SetSaberGlowColor_MaterialPropertyBlock = FieldAccessor<SetSaberGlowColor, MaterialPropertyBlock>.GetAccessor("_materialPropertyBlock"); 
        private static readonly FieldAccessor<SetSaberGlowColor, SetSaberGlowColor.PropertyTintColorPair[]>.Accessor SetSaberGlowColor_PropertyTintColorPairs = FieldAccessor<SetSaberGlowColor, SetSaberGlowColor.PropertyTintColorPair[]>.GetAccessor("_propertyTintColorPairs"); 
        
        internal static void SetColors(this SetSaberGlowColor setSaberGlowColor, Color color)
        {
            MeshRenderer meshRenderer = SetSaberGlowColor_MeshRenderer(ref setSaberGlowColor);
            MaterialPropertyBlock? materialPropertyBlock = SetSaberGlowColor_MaterialPropertyBlock(ref setSaberGlowColor);
            SetSaberGlowColor.PropertyTintColorPair[] propertyTintPairs = SetSaberGlowColor_PropertyTintColorPairs(ref setSaberGlowColor);
        
            if (materialPropertyBlock is null)
                materialPropertyBlock = SetSaberGlowColor_MaterialPropertyBlock(ref setSaberGlowColor) = new MaterialPropertyBlock();

            foreach (var tintPair in propertyTintPairs)
                materialPropertyBlock.SetColor(tintPair.property, color * tintPair.tintColor);

            meshRenderer.SetPropertyBlock(materialPropertyBlock);
        }

        private static readonly FieldAccessor<SetSaberFakeGlowColor, Color>.Accessor SetSaberFakeGlowColor_TintColor = FieldAccessor<SetSaberFakeGlowColor, Color>.GetAccessor("_tintColor");
        private static readonly FieldAccessor<SetSaberFakeGlowColor, Parametric3SliceSpriteController>.Accessor SetSaberFakeGlowColor_Parametric3SliceSprite = FieldAccessor<SetSaberFakeGlowColor, Parametric3SliceSpriteController>.GetAccessor("_parametric3SliceSprite");

        internal static void SetColors(this SetSaberFakeGlowColor setSaberFakeGlowColor, Color color)
        {
            Parametric3SliceSpriteController parametricSpriteController = SetSaberFakeGlowColor_Parametric3SliceSprite(ref setSaberFakeGlowColor);
            Color tint = SetSaberFakeGlowColor_TintColor(ref setSaberFakeGlowColor);
            parametricSpriteController.color = color * tint;
            parametricSpriteController.Refresh();
        }
    }
}