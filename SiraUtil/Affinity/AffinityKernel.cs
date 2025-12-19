using HarmonyLib;
using SiraUtil.Affinity.Harmony;
using SiraUtil.Logging;
using System;
using Zenject;

namespace SiraUtil.Affinity
{
    internal class AffinityKernel : ILateDisposable
    {
        private readonly AffinityManager _affinityManager;
        private readonly IAffinityPatcher _affinityPatcher = new HarmonyAffinityPatcher();

        public AffinityKernel([InjectLocal] AffinityManager affinityManager, SiraLog logger)
        {
            _affinityManager = affinityManager;

            foreach (var affinity in _affinityManager.Affinities)
            {
                try
                {
                    _affinityPatcher.Patch(affinity);
                }
                catch (Exception ex)
                {
                    logger.Error($"Failed to apply Affinity patches on class '{affinity.GetType().FullDescription()}'\n{ex}");
                }
            }
        }

        public void LateDispose()
        {
            _affinityPatcher.Dispose();
        }
    }
}