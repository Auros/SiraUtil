using HarmonyLib;
using IPA.Utilities;
using SiraUtil.Affinity;
using System;
using System.Collections.Generic;
using Zenject;

namespace SiraUtil.Sabers.Effects
{
    internal class ObstacleSaberSparkleEffectManagerLatch : IDisposable, IAffinity
    {
        private readonly ColorManager _colorManager;
        private readonly SiraSaberFactory _siraSaberFactory;
        private readonly DiContainer _container;
        private readonly Queue<SiraSaber> _earlySabers = new();
        private ObstacleSaberSparkleEffectManager? _obstacleSaberSparkleEffectManager;

        // TODO: These are both readonly fields. Figure out if there's a way to get around that.
        private static readonly FieldAccessor<ObstacleSaberSparkleEffectManager, Saber[]>.Accessor Sabers = FieldAccessor<ObstacleSaberSparkleEffectManager, Saber[]>.GetAccessor(nameof(ObstacleSaberSparkleEffectManager._sabers));
        private static readonly FieldAccessor<ObstacleSaberSparkleEffectManager, ObstacleSaberSparkleEffect[]>.Accessor Effects = FieldAccessor<ObstacleSaberSparkleEffectManager, ObstacleSaberSparkleEffect[]>.GetAccessor(nameof(ObstacleSaberSparkleEffectManager._effects));

        public ObstacleSaberSparkleEffectManagerLatch(ColorManager colorManager, SiraSaberFactory siraSaberFactory, DiContainer container)
        {
            _colorManager = colorManager;
            _siraSaberFactory = siraSaberFactory;
            _container = container;
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
            if (_obstacleSaberSparkleEffectManager == null)
                return;

            Sabers(ref _obstacleSaberSparkleEffectManager) = Sabers(ref _obstacleSaberSparkleEffectManager).AddToArray(saber);

            ObstacleSaberSparkleEffect effect = CreateNewObstacleSaberSparkleEffect();
            Effects(ref _obstacleSaberSparkleEffectManager) = Effects(ref _obstacleSaberSparkleEffectManager).AddToArray(effect);
        }

        private ObstacleSaberSparkleEffect CreateNewObstacleSaberSparkleEffect()
        {
            ObstacleSaberSparkleEffect obstacleSaberSparkleEffect = _container.InstantiatePrefabForComponent<ObstacleSaberSparkleEffect>(_obstacleSaberSparkleEffectManager!._obstacleSaberSparkleEffectPrefab);
            obstacleSaberSparkleEffect.name = $"SiraUtil | {obstacleSaberSparkleEffect.name}";
            obstacleSaberSparkleEffect.color = _colorManager.GetObstacleEffectColor();
            return obstacleSaberSparkleEffect;
        }


        [AffinityPostfix]
        [AffinityPatch(typeof(ObstacleSaberSparkleEffectManager), nameof(ObstacleSaberSparkleEffectManager.Start))]
        internal void SparklesStarting(ObstacleSaberSparkleEffectManager __instance)
        {
            _obstacleSaberSparkleEffectManager = __instance;
            foreach (SiraSaber siraSaber in _earlySabers)
                AddSaber(siraSaber.Saber);
            _earlySabers.Clear();
        }
    }
}