using IPA.Utilities;
using System;
using UnityEngine;
using Zenject;

namespace SiraUtil.Sabers.Effects
{
    internal class SaberClashEffectAdjuster : IInitializable, IDisposable
    {
        private readonly SaberClashEffect _saberClashEffect;
        private readonly SaberModelManager _saberModelManager;
        private readonly SiraSaberClashChecker _saberClashChecker;

        private readonly ParticleSystem _glowParticleSystem;
        private readonly ParticleSystem _sparkleParticleSystem;
        private static readonly FieldAccessor<SaberClashEffect, ParticleSystem>.Accessor GlowParticles = FieldAccessor<SaberClashEffect, ParticleSystem>.GetAccessor("_glowParticleSystem");
        private static readonly FieldAccessor<SaberClashEffect, ParticleSystem>.Accessor SparkleParticles = FieldAccessor<SaberClashEffect, ParticleSystem>.GetAccessor("_sparkleParticleSystem");

        public SaberClashEffectAdjuster(SaberClashEffect saberClashEffect, SaberModelManager saberModelManager, SaberClashChecker saberClashChecker)
        {
            _saberClashEffect = saberClashEffect;
            _saberModelManager = saberModelManager;
            _saberClashChecker = (saberClashChecker as SiraSaberClashChecker)!;

            _glowParticleSystem = GlowParticles(ref _saberClashEffect);
            _sparkleParticleSystem = SparkleParticles(ref _saberClashEffect);
        }

        public void Initialize()
        {
            _saberClashChecker.NewSabersClashed += SaberClashChecker_NewSabersClashed;
        }

        private void SaberClashChecker_NewSabersClashed(Saber saberA, Saber saberB)
        {
            Color colorA = _saberModelManager.GetPhysicalSaberColor(saberA);
            Color colorB = _saberModelManager.GetPhysicalSaberColor(saberB);

            Color combinedColor = Color.Lerp(colorA, colorB, 0.5f).ColorWithAlpha(1f);
            var glowMainModule = _glowParticleSystem.main;
            var sparkleMainModule = _sparkleParticleSystem.main;

            glowMainModule.startColor = combinedColor;
            sparkleMainModule.startColor = combinedColor;
        }

        public void Dispose()
        {
            _saberClashChecker.NewSabersClashed -= SaberClashChecker_NewSabersClashed;
        }
    }
}