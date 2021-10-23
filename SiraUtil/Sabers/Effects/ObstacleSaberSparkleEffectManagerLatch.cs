using HarmonyLib;
using IPA.Utilities;
using SiraUtil.Affinity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SiraUtil.Sabers.Effects
{
    internal class ObstacleSaberSparkleEffectManagerLatch : IDisposable, IAffinity
    {
        private readonly ColorManager _colorManager;
        private readonly SiraSaberFactory _siraSaberFactory;
        private readonly Queue<SiraSaber> _earlySabers = new();
        private ObstacleSaberSparkleEffectManager? _obstacleSaberSparkleEffectManager;

        private static readonly FieldAccessor<ObstacleSaberSparkleEffectManager, Saber[]>.Accessor Sabers = FieldAccessor<ObstacleSaberSparkleEffectManager, Saber[]>.GetAccessor("_sabers");
        private static readonly FieldAccessor<ObstacleSaberSparkleEffectManager, bool[]>.Accessor IsSystemActive = FieldAccessor<ObstacleSaberSparkleEffectManager, bool[]>.GetAccessor("_isSystemActive");
        private static readonly FieldAccessor<ObstacleSaberSparkleEffectManager, bool[]>.Accessor WasSystemActive = FieldAccessor<ObstacleSaberSparkleEffectManager, bool[]>.GetAccessor("_wasSystemActive");
        private static readonly FieldAccessor<ObstacleSaberSparkleEffectManager, Vector3[]>.Accessor Positions = FieldAccessor<ObstacleSaberSparkleEffectManager, Vector3[]>.GetAccessor("_burnMarkPositions");
        private static readonly FieldAccessor<ObstacleSaberSparkleEffectManager, Transform[]>.Accessor Transforms = FieldAccessor<ObstacleSaberSparkleEffectManager, Transform[]>.GetAccessor("_effectsTransforms");
        private static readonly FieldAccessor<ObstacleSaberSparkleEffectManager, ObstacleSaberSparkleEffect[]>.Accessor Effects = FieldAccessor<ObstacleSaberSparkleEffectManager, ObstacleSaberSparkleEffect[]>.GetAccessor("_effects");
        private static readonly FieldAccessor<ObstacleSaberSparkleEffectManager, ObstacleSaberSparkleEffect>.Accessor SparkleEffectPrefab = FieldAccessor<ObstacleSaberSparkleEffectManager, ObstacleSaberSparkleEffect>.GetAccessor("_obstacleSaberSparkleEffectPrefab");

        public ObstacleSaberSparkleEffectManagerLatch(ColorManager colorManager, SiraSaberFactory siraSaberFactory)
        {
            _colorManager = colorManager;
            _siraSaberFactory = siraSaberFactory;
            _siraSaberFactory.SaberCreated += SiraSaberFactory_SaberCreated;
        }

        private void SiraSaberFactory_SaberCreated(SiraSaber siraSaber)
        {
            if (_obstacleSaberSparkleEffectManager == null)
                _earlySabers.Enqueue(siraSaber);
            else
                AddSaber(siraSaber.Saber);
        }

        public void Dispose()
        {
            _siraSaberFactory.SaberCreated -= SiraSaberFactory_SaberCreated;
        }

        private void AddSaber(Saber saber)
        {
            if (_obstacleSaberSparkleEffectManager is null)
                return;

            Sabers(ref _obstacleSaberSparkleEffectManager) = Sabers(ref _obstacleSaberSparkleEffectManager).AddToArray(saber);
            IsSystemActive(ref _obstacleSaberSparkleEffectManager) = IsSystemActive(ref _obstacleSaberSparkleEffectManager).AddToArray(default);
            WasSystemActive(ref _obstacleSaberSparkleEffectManager) = WasSystemActive(ref _obstacleSaberSparkleEffectManager).AddToArray(default);
            Positions(ref _obstacleSaberSparkleEffectManager) = Positions(ref _obstacleSaberSparkleEffectManager).AddToArray(default);

            ObstacleSaberSparkleEffect effect = CreateNewObstacleSaberSparkleEffect();
            Transforms(ref _obstacleSaberSparkleEffectManager) = Transforms(ref _obstacleSaberSparkleEffectManager).AddToArray(effect.transform);
            Effects(ref _obstacleSaberSparkleEffectManager) = Effects(ref _obstacleSaberSparkleEffectManager).AddToArray(effect);
        }

        private ObstacleSaberSparkleEffect CreateNewObstacleSaberSparkleEffect()
        {
            ObstacleSaberSparkleEffect obstacleSaberSparkleEffect = UnityEngine.Object.Instantiate(SparkleEffectPrefab(ref _obstacleSaberSparkleEffectManager!));
            obstacleSaberSparkleEffect.name = $"SiraUtil | {obstacleSaberSparkleEffect.name}";
            obstacleSaberSparkleEffect.color = _colorManager.GetObstacleEffectColor();
            return obstacleSaberSparkleEffect;
        }


        [AffinityPostfix]
        [AffinityPatch(typeof(ObstacleSaberSparkleEffectManager), nameof(ObstacleSaberSparkleEffectManager.Start))]
        internal void SparklesStarting(ObstacleSaberSparkleEffectManager __instance)
        {
            _obstacleSaberSparkleEffectManager = __instance;
            foreach (var siraSaber in _earlySabers)
                AddSaber(siraSaber.Saber);
            _earlySabers.Clear();
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(ObstacleSaberSparkleEffectManager), nameof(ObstacleSaberSparkleEffectManager.Update))]
        internal void UpdateExtraSystemStates(ref bool[] ____isSystemActive, ref bool[] ____wasSystemActive)
        {
            if (____isSystemActive.Length > 2 && ____wasSystemActive.Length > 2 && ____isSystemActive.Length == ____wasSystemActive.Length)
            {
                for (int i = 2; i < ____isSystemActive.Length; i++)
                {
                    ____wasSystemActive[i] = ____isSystemActive[i];
                    ____isSystemActive[i] = false;
                }
            }
        }
    }
}