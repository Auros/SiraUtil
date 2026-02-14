using HarmonyLib;
using SiraUtil.Affinity;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SiraUtil.Sabers.Effects
{
    internal class SaberBurnMarkSparklesLatch : IDisposable, IAffinity
    {
        private readonly SiraSaberFactory _siraSaberFactory;
        private readonly SaberModelManager _saberModelManager;
        private readonly DiContainer _container;
        private readonly Queue<SiraSaber> _earlySabers = new();

        private SaberBurnMarkSparkles? _saberBurnMarkSparkles;
        private bool _sisterLoopActive = false;
        private int _activeSaberIndex = 0;

        public SaberBurnMarkSparklesLatch(SiraSaberFactory siraSaberFactory, SaberModelManager saberModelManager, DiContainer container)
        {
            _siraSaberFactory = siraSaberFactory;
            _saberModelManager = saberModelManager;
            _container = container;
            _siraSaberFactory.SaberCreated += SiraSaberFactory_SaberCreated;
        }

        public void Dispose()
        {
            _siraSaberFactory.SaberCreated -= SiraSaberFactory_SaberCreated;
        }

        private void SiraSaberFactory_SaberCreated(SiraSaber siraSaber)
        {
            if (_saberBurnMarkSparkles == null)
                _earlySabers.Enqueue(siraSaber);
            else
                AddSaber(siraSaber.Saber);
        }

        private void AddSaber(Saber saber)
        {
            if (_saberBurnMarkSparkles == null)
                return;

            _saberBurnMarkSparkles._sabers = _saberBurnMarkSparkles._sabers.AddToArray(saber);
            _saberBurnMarkSparkles._prevBurnMarkPos = _saberBurnMarkSparkles._prevBurnMarkPos.AddToArray(default);
            _saberBurnMarkSparkles._prevBurnMarkPosValid = _saberBurnMarkSparkles._prevBurnMarkPosValid.AddToArray(default);

            ParticleSystem newPs = CreateNewBurnMarkParticles();
            _saberBurnMarkSparkles._burnMarksPS = _saberBurnMarkSparkles._burnMarksPS.AddToArray(newPs);
            _saberBurnMarkSparkles._burnMarksEmissionModules = _saberBurnMarkSparkles._burnMarksEmissionModules.AddToArray(newPs.emission);
        }

        private ParticleSystem CreateNewBurnMarkParticles()
        {
            Quaternion rotation = default;
            rotation.eulerAngles = new Vector3(-90f, 0f, 0f);
            ParticleSystem ps = _container.InstantiatePrefabForComponent<ParticleSystem>(_saberBurnMarkSparkles!._burnMarksPSPrefab, Vector3.zero, rotation, null!);
            ps.name = $"SiraUtil | {ps.name}";
            return ps;
        }

        [AffinityPostfix]
        [AffinityPatch(typeof(SaberBurnMarkSparkles), nameof(SaberBurnMarkSparkles.Start))]
        internal void SparklesStarting(SaberBurnMarkSparkles __instance)
        {
            _saberBurnMarkSparkles = __instance;
            foreach (var siraSaber in _earlySabers)
                AddSaber(siraSaber.Saber);
            _earlySabers.Clear();
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(SaberBurnMarkSparkles), nameof(SaberBurnMarkSparkles.LateUpdate))]
        internal void StartSisterHookLoop()
        {
            _activeSaberIndex = 0;
            _sisterLoopActive = true;
        }

        [AffinityPostfix]
        [AffinityPatch(typeof(SaberBurnMarkSparkles), nameof(SaberBurnMarkSparkles.LateUpdate))]
        internal void EndSisterHookLoop()
        {
            _activeSaberIndex = 0;
            _sisterLoopActive = false;
        }

        [AffinityPrefix]
        [AffinityPriority(int.MaxValue)]
        [AffinityPatch(typeof(ColorManager), nameof(ColorManager.ColorForSaberType))]
        internal bool SisterLoopColorOverrideLock(ref Color __result)
        {
            if (!_sisterLoopActive || _saberBurnMarkSparkles == null)
                return true;

            Saber[] sabers = _saberBurnMarkSparkles._sabers;
            if (_activeSaberIndex >= sabers.Length)
                return true;

            __result = _saberModelManager.GetPhysicalSaberColor(sabers[_activeSaberIndex++]).ColorWithAlpha(1f);
            return false;
        }
    }
}