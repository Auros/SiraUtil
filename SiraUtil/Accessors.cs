using Xft;
using Zenject;
using UnityEngine;
using IPA.Utilities;
using System.Collections.Generic;

namespace SiraUtil
{
    public static class Accessors
    {
        public static PropertyAccessor<MonoInstallerBase, DiContainer>.Getter GetDiContainer = PropertyAccessor<MonoInstallerBase, DiContainer>.GetGetter("Container");

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
    }
}