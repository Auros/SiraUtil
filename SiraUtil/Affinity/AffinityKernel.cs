using SiraUtil.Affinity.Harmony;
using System;
using System.Collections.Generic;
using System.Reflection;
using Zenject;

namespace SiraUtil.Affinity
{
    internal class AffinityKernel : IInitializable, ILateDisposable
    {
        private readonly AffinityManager _affinityManager;
        private readonly HashSet<ValueTuple<Guid, Assembly>> _patchedAffinities = new();
        private readonly IAffinityPatcher _affinityPatcher = new HarmonyAffinityPatcher();

        public AffinityKernel([InjectLocal] AffinityManager affinityManager)
        {
            _affinityManager = affinityManager;
        }

        public void Initialize()
        {
            foreach (var affinity in _affinityManager.Affinities)
            {
                Guid? id = _affinityPatcher.Patch(affinity);
                if (id.HasValue)
                    _patchedAffinities.Add((id.Value, affinity.GetType().Assembly));
            }
        }

        public void LateDispose()
        {
            foreach (var affinity in _patchedAffinities)
                _affinityPatcher.Unpatch(affinity.Item1, affinity.Item2);
            _affinityPatcher.Dispose();
        }
    }
}