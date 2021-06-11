using HarmonyLib;
using IPA.Utilities;
using SiraUtil.Affinity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace SiraUtil.Sabers.Effects
{
    internal class SaberBurnMarkSparklesLatch : IInitializable, IDisposable, IAffinity
    {
        private static FieldAccessor<SaberBurnMarkSparkles, Saber[]>.Accessor Sabers = FieldAccessor<SaberBurnMarkSparkles, Saber[]>.GetAccessor("_sabers");
        private static FieldAccessor<SaberBurnMarkSparkles, Vector3[]>.Accessor PreviousMarks = FieldAccessor<SaberBurnMarkSparkles, Vector3[]>.GetAccessor("_prevBurnMarkPos");
        private static FieldAccessor<SaberBurnMarkSparkles, bool[]>.Accessor PreviousMarksValid = FieldAccessor<SaberBurnMarkSparkles, bool[]>.GetAccessor("_prevBurnMarkPosValid");
        private static FieldAccessor<SaberBurnMarkSparkles, ParticleSystem[]>.Accessor Particles = FieldAccessor<SaberBurnMarkSparkles, ParticleSystem[]>.GetAccessor("_burnMarksPS");
        private static FieldAccessor<SaberBurnMarkSparkles, ParticleSystem.EmissionModule[]>.Accessor Emissions = FieldAccessor<SaberBurnMarkSparkles, ParticleSystem.EmissionModule[]>.GetAccessor("_burnMarksEmissionModules");

        private readonly SiraSaberFactory _siraSaberFactory;
        private readonly SaberModelManager _saberModelManager;

        public SaberBurnMarkSparklesLatch(SiraSaberFactory siraSaberFactory, SaberModelManager saberModelManager)
        {
            _siraSaberFactory = siraSaberFactory;
            _saberModelManager = saberModelManager;
        }

        public void Initialize()
        {
            _siraSaberFactory.SaberCreated += SiraSaberFactory_SaberCreated;
        }

        private void SiraSaberFactory_SaberCreated(SiraSaber siraSaber)
        {

        }

        public void Dispose()
        {
            _siraSaberFactory.SaberCreated -= SiraSaberFactory_SaberCreated;
        }

        [AffinityTranspiler]
        [AffinityPatch(typeof(SaberBurnMarkSparkles), nameof(SaberBurnMarkSparkles.LateUpdate))]
        internal IEnumerable<CodeInstruction> ShiftColorReceiver(IEnumerable<CodeInstruction> instructions)
        {

        }
    }
}