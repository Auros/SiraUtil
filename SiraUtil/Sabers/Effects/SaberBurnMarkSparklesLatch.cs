using HarmonyLib;
using IPA.Utilities;
using SiraUtil.Affinity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SiraUtil.Sabers.Effects
{
    internal class SaberBurnMarkSparklesLatch : IDisposable, IAffinity
    {
        private static readonly FieldAccessor<SaberBurnMarkSparkles, Saber[]>.Accessor Sabers = FieldAccessor<SaberBurnMarkSparkles, Saber[]>.GetAccessor("_sabers");
        private static readonly FieldAccessor<SaberBurnMarkSparkles, Vector3[]>.Accessor PreviousMarks = FieldAccessor<SaberBurnMarkSparkles, Vector3[]>.GetAccessor("_prevBurnMarkPos");
        private static readonly FieldAccessor<SaberBurnMarkSparkles, bool[]>.Accessor PreviousMarksValid = FieldAccessor<SaberBurnMarkSparkles, bool[]>.GetAccessor("_prevBurnMarkPosValid");
        private static readonly FieldAccessor<SaberBurnMarkSparkles, ParticleSystem[]>.Accessor Particles = FieldAccessor<SaberBurnMarkSparkles, ParticleSystem[]>.GetAccessor("_burnMarksPS");
        private static readonly FieldAccessor<SaberBurnMarkSparkles, ParticleSystem>.Accessor ParticlesPrefab = FieldAccessor<SaberBurnMarkSparkles, ParticleSystem>.GetAccessor("_burnMarksPSPrefab");
        private static readonly FieldAccessor<SaberBurnMarkSparkles, ParticleSystem.EmissionModule[]>.Accessor Emissions = FieldAccessor<SaberBurnMarkSparkles, ParticleSystem.EmissionModule[]>.GetAccessor("_burnMarksEmissionModules");

        private readonly MethodInfo _colorForSaberType;
        private readonly SiraSaberFactory _siraSaberFactory;
        private readonly SaberModelManager _saberModelManager;
        private SaberBurnMarkSparkles? _saberBurnMarkSparkles;
        private readonly Queue<SiraSaber> _earlySabers = new();
        private bool _sisterLoopActive = false;
        private int _activeSaberIndex = 0;

        public SaberBurnMarkSparklesLatch(SiraSaberFactory siraSaberFactory, SaberModelManager saberModelManager, ColorManager colorManager)
        {
            _siraSaberFactory = siraSaberFactory;
            _saberModelManager = saberModelManager;
            _colorForSaberType = SymbolExtensions.GetMethodInfo(() => colorManager.ColorForSaberType(default));
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
            if (_saberBurnMarkSparkles is null)
                return;

            Sabers(ref _saberBurnMarkSparkles) = Sabers(ref _saberBurnMarkSparkles).AddItem(saber).ToArray();
            PreviousMarks(ref _saberBurnMarkSparkles) = PreviousMarks(ref _saberBurnMarkSparkles).AddItem(default).ToArray();
            PreviousMarksValid(ref _saberBurnMarkSparkles) = PreviousMarksValid(ref _saberBurnMarkSparkles).AddItem(default).ToArray();

            ParticleSystem newPs = CreateNewBurnMarkParticles();
            Particles(ref _saberBurnMarkSparkles) = Particles(ref _saberBurnMarkSparkles).AddToArray(newPs);
            Emissions(ref _saberBurnMarkSparkles) = Emissions(ref _saberBurnMarkSparkles).AddToArray(newPs.emission);
        }

        private ParticleSystem CreateNewBurnMarkParticles()
        {
            Quaternion rotation = default;
            rotation.eulerAngles = new Vector3(-90f, 0f, 0f);
            ParticleSystem ps = UnityEngine.Object.Instantiate(ParticlesPrefab(ref _saberBurnMarkSparkles!), Vector3.zero, rotation, null!);
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
            if (!_sisterLoopActive || _saberBurnMarkSparkles is null)
                return true;

            Saber[] sabers = Sabers(ref _saberBurnMarkSparkles);
            if (_activeSaberIndex >= sabers.Length)
                return true;

            __result = _saberModelManager.GetPhysicalSaberColor(sabers[_activeSaberIndex++]).ColorWithAlpha(1f);
            return false;
        }

        private Color PhysicalSaberColor(Saber saber)
        {
            return _saberModelManager.GetPhysicalSaberColor(saber);
        }
    }
}