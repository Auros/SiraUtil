using SiraUtil.Affinity.Harmony;
using System.Linq;
using Zenject;

namespace SiraUtil.Affinity
{
    internal class AffinityKernel : IInitializable, ILateDisposable
    {
        private readonly AffinityManager _affinityManager;
        private readonly IAffinityPatcher _affinityPatcher = new HarmonyAffinityPatcher();

        public AffinityKernel([InjectLocal] AffinityManager affinityManager)
        {
            _affinityManager = affinityManager;
            Plugin.Log.Info($"Initializing Affinity Kernel with {affinityManager.Affinities.Count()} registered affinities.");
        }

        public void Initialize()
        {
            foreach (var affinity in _affinityManager.Affinities)
                _affinityPatcher.Patch(affinity);
        }

        public void LateDispose()
        {
            foreach (var affinity in _affinityManager.Affinities)
                _affinityPatcher.Unpatch(affinity);
        }
    }
}