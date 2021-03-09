using Zenject;
using UnityEngine;
using IPA.Utilities;
using System.Collections.Generic;

namespace SiraUtil
{
    /// <summary>
    /// A collection of accesors used in SiraUtil.
    /// </summary>
    public static class Accessors
    {
        /// <summary>
        /// Gets a container from a MonoInstaller
        /// </summary>
        public static PropertyAccessor<MonoInstallerBase, DiContainer>.Getter GetDiContainer = PropertyAccessor<MonoInstallerBase, DiContainer>.GetGetter("Container");

        internal static FieldAccessor<SaberTrail, Color>.Accessor TrailColor = FieldAccessor<SaberTrail, Color>.GetAccessor("_color");
        internal static FieldAccessor<SceneDecoratorContext, List<MonoBehaviour>>.Accessor Injectables = FieldAccessor<SceneDecoratorContext, List<MonoBehaviour>>.GetAccessor("_injectableMonoBehaviours");

        internal static FieldAccessor<SaberManager, Saber>.Accessor SMLeftSaber = FieldAccessor<SaberManager, Saber>.GetAccessor("_leftSaber");
        internal static FieldAccessor<SaberManager, Saber>.Accessor SMRightSaber = FieldAccessor<SaberManager, Saber>.GetAccessor("_rightSaber");

        internal static FieldAccessor<SaberModelController, SaberTrail>.Accessor SaberTrail = FieldAccessor<SaberModelController, SaberTrail>.GetAccessor("_saberTrail");
        internal static FieldAccessor<SaberModelController, ColorManager>.Accessor SaberColorManager = FieldAccessor<SaberModelController, ColorManager>.GetAccessor("_colorManager");
        internal static FieldAccessor<SaberModelController, TubeBloomPrePassLight>.Accessor SaberLight = FieldAccessor<SaberModelController, TubeBloomPrePassLight>.GetAccessor("_saberLight");
        internal static FieldAccessor<SaberModelController, SetSaberGlowColor[]>.Accessor SaberGlowColor = FieldAccessor<SaberModelController, SetSaberGlowColor[]>.GetAccessor("_setSaberGlowColors");
        internal static FieldAccessor<SaberModelController, SetSaberFakeGlowColor[]>.Accessor FakeSaberGlowColor = FieldAccessor<SaberModelController, SetSaberFakeGlowColor[]>.GetAccessor("_setSaberFakeGlowColors");
        internal static FieldAccessor<SaberModelController, SaberModelController.InitData>.Accessor ModelInitData = FieldAccessor<SaberModelController, SaberModelController.InitData>.GetAccessor("_initData");

        internal static FieldAccessor<SetSaberGlowColor, MeshRenderer>.Accessor GlowMeshRenderer = FieldAccessor<SetSaberGlowColor, MeshRenderer>.GetAccessor("_meshRenderer");
        internal static FieldAccessor<SetSaberGlowColor, MaterialPropertyBlock>.Accessor GlowMaterialPropertyBlock = FieldAccessor<SetSaberGlowColor, MaterialPropertyBlock>.GetAccessor("_materialPropertyBlock");
        internal static FieldAccessor<SetSaberGlowColor, SetSaberGlowColor.PropertyTintColorPair[]>.Accessor GlowPropertyTintPair = FieldAccessor<SetSaberGlowColor, SetSaberGlowColor.PropertyTintColorPair[]>.GetAccessor("_propertyTintColorPairs");

        internal static FieldAccessor<SetSaberFakeGlowColor, Parametric3SliceSpriteController>.Accessor FakeGlowSliceSprite = FieldAccessor<SetSaberFakeGlowColor, Parametric3SliceSpriteController>.GetAccessor("_parametric3SliceSprite");
        internal static FieldAccessor<SetSaberFakeGlowColor, Color>.Accessor FakeGlowTint = FieldAccessor<SetSaberFakeGlowColor, Color>.GetAccessor("_tintColor");

        internal static FieldAccessor<SaberTypeObject, SaberType>.Accessor ObjectSaberType = FieldAccessor<SaberTypeObject, SaberType>.GetAccessor("_saberType");
        internal static FieldAccessor<Saber, SaberTypeObject>.Accessor SaberObjectType = FieldAccessor<Saber, SaberTypeObject>.GetAccessor("_saberType");

        internal static FieldAccessor<Saber, Transform>.Accessor SaberBladeTopTransform = FieldAccessor<Saber, Transform>.GetAccessor("_saberBladeTopTransform");
        internal static FieldAccessor<Saber, Transform>.Accessor SaberBladeBottomTransform = FieldAccessor<Saber, Transform>.GetAccessor("_saberBladeBottomTransform");
        internal static FieldAccessor<Saber, Transform>.Accessor SaberHandleTransform = FieldAccessor<Saber, Transform>.GetAccessor("_handleTransform");
        //internal static FieldAccessor<Saber, List<SaberSwingRatingCounter>>.Accessor SwingRatingCounters = FieldAccessor<Saber, List<SaberSwingRatingCounter>>.GetAccessor("_swingRatingCounters");
        //internal static FieldAccessor<Saber, List<SaberSwingRatingCounter>>.Accessor UnusedSwingRatingCounters = FieldAccessor<Saber, List<SaberSwingRatingCounter>>.GetAccessor("_unusedSwingRatingCounters");
        internal static FieldAccessor<Saber, SaberMovementData>.Accessor MovementData = FieldAccessor<Saber, SaberMovementData>.GetAccessor("_movementData");
        internal static FieldAccessor<Saber, Vector3>.Accessor SaberBladeTopPosition = FieldAccessor<Saber, Vector3>.GetAccessor("_saberBladeTopPos");
        internal static FieldAccessor<Saber, Vector3>.Accessor SaberBladeBottomPosition = FieldAccessor<Saber, Vector3>.GetAccessor("_saberBladeBottomPos");
    }
}