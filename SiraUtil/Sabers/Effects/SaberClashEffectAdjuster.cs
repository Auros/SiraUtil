using SiraUtil.Affinity;
using System;
using UnityEngine;
using Zenject;

namespace SiraUtil.Sabers.Effects
{
    internal class SaberClashEffectAdjuster : IInitializable, IDisposable, IAffinity
    {
        private SaberClashEffect? _saberClashEffect;
        private ParticleSystem? _glowParticleSystem;
        private ParticleSystem? _sparkleParticleSystem;
        private readonly SaberModelManager _saberModelManager;
        private readonly SiraSaberClashChecker _saberClashChecker;

        public SaberClashEffectAdjuster(SaberModelManager saberModelManager, SaberClashChecker saberClashChecker)
        {
            _saberModelManager = saberModelManager;
            _saberClashChecker = (saberClashChecker as SiraSaberClashChecker)!;
        }

        public void Initialize()
        {
            _saberClashChecker.NewSabersClashed += SaberClashChecker_NewSabersClashed;
        }

        private void SaberClashChecker_NewSabersClashed(Saber saberA, Saber saberB)
        {
            if (_glowParticleSystem == null || _sparkleParticleSystem == null || _saberClashEffect == null)
                return;

            Color colorA = _saberModelManager.GetPhysicalSaberColor(saberA);
            Color colorB = _saberModelManager.GetPhysicalSaberColor(saberB);

            Color combinedColor = Color.Lerp(colorA, colorB, 0.5f).ColorWithAlpha(1f);
            ParticleSystem.MainModule glowMainModule = _glowParticleSystem.main;
            ParticleSystem.MainModule sparkleMainModule = _sparkleParticleSystem.main;

            glowMainModule.startColor = combinedColor;
            sparkleMainModule.startColor = combinedColor;
        }

        public void Dispose()
        {
            _saberClashChecker.NewSabersClashed -= SaberClashChecker_NewSabersClashed;
        }

        [AffinityPostfix]
        [AffinityPatch(typeof(SaberClashEffect), nameof(SaberClashEffect.Start))]
        private void ClashersInit(SaberClashEffect __instance, ref ParticleSystem ____glowParticleSystem, ref ParticleSystem ____sparkleParticleSystem)
        {
            _saberClashEffect = __instance;
            _glowParticleSystem = ____glowParticleSystem;
            _sparkleParticleSystem = ____sparkleParticleSystem;
        }
    }
}