using SiraUtil.Interfaces;
using UnityEngine;

namespace SiraUtil.Extras
{
    internal static class SaberExtensions
    {
        internal static Color GetColor(this SaberModelController saberModelController)
        {
            if (saberModelController is IColorable colorable)
                return colorable.Color;
            return saberModelController._saberTrail._color;
        }

        internal static void SetColor(this SaberModelController saberModelController, Color color)
        {
            if (saberModelController is IColorable colorable)
                colorable.Color = color;

            SaberTrail trail = saberModelController._saberTrail;
            TubeBloomPrePassLight? light = saberModelController._saberLight;
            SetSaberGlowColor[] glowColors = saberModelController._setSaberGlowColors;
            SetSaberFakeGlowColor[] fakeGlowColors = saberModelController._setSaberFakeGlowColors;

            float beforeAlpha = trail._color.a;
            trail._color = trail._colorOverwrite ? trail._forcedColor : color.ColorWithAlpha(beforeAlpha);
            foreach (SetSaberGlowColor glow in glowColors)
                glow.SetColors(color);
            foreach (SetSaberFakeGlowColor fakeGlow in fakeGlowColors)
                fakeGlow.SetColors(color);
            if (light != null)
                light.color = color;
        }

        internal static SetSaberGlowColor[] SaberGlowColors(this SaberModelController saberModelController)
            => saberModelController._setSaberGlowColors;
        internal static SetSaberFakeGlowColor[] SaberFakeGlowColors(this SaberModelController saberModelController)
            => saberModelController._setSaberFakeGlowColors;

        internal static void SetColors(this SetSaberGlowColor setSaberGlowColor, Color color)
        {
            MeshRenderer meshRenderer = setSaberGlowColor._meshRenderer;
            MaterialPropertyBlock? materialPropertyBlock = setSaberGlowColor._materialPropertyBlock;
            SetSaberGlowColor.PropertyTintColorPair[] propertyTintPairs = setSaberGlowColor._propertyTintColorPairs;

            if (materialPropertyBlock is null)
                materialPropertyBlock = setSaberGlowColor._materialPropertyBlock = new MaterialPropertyBlock();

            foreach (SetSaberGlowColor.PropertyTintColorPair tintPair in propertyTintPairs)
                materialPropertyBlock.SetColor(tintPair.property, color * tintPair.tintColor);

            meshRenderer.SetPropertyBlock(materialPropertyBlock);
        }

        internal static void SetColors(this SetSaberFakeGlowColor setSaberFakeGlowColor, Color color)
        {
            Parametric3SliceSpriteController parametricSpriteController = setSaberFakeGlowColor._parametric3SliceSprite;
            Color tint = setSaberFakeGlowColor._tintColor;
            parametricSpriteController.color = color * tint;
            parametricSpriteController.Refresh();
        }
    }
}