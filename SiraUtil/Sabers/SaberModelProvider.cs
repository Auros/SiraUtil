using IPA.Utilities;
using SiraUtil.Affinity;
using SiraUtil.Extras;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SiraUtil.Sabers
{
    /// <summary>
    /// Provides copies of the active saber model.
    /// </summary>
    public class SaberModelProvider : IAffinity
    {
        private readonly DiContainer _container;
        private readonly SaberManager _saberManager;
        private readonly SaberModelController _leftSaberDefaultTemplate;
        private readonly SaberModelController _rightSaberDefaultTemplate;
        private readonly HashSet<SetSaberGlowColor> _earlyInittingGlowColors = new();
        private readonly HashSet<SetSaberFakeGlowColor> _earlyInittingFakeGlowColors = new();
        private static readonly FieldAccessor<SaberModelContainer, SaberModelController>.Accessor SaberModelContainer_SaberModelController = FieldAccessor<SaberModelContainer, SaberModelController>.GetAccessor("_saberModelControllerPrefab");

        internal SaberModelProvider(DiContainer container, SaberManager saberManager, AffinityManager affinityManager)
        {
            _container = container;
            affinityManager.Add(this); // We manually add this to the affinity manager so the patches are only created when people depend on the SaberModelProvider
            _saberManager = saberManager;
            SaberModelContainer leftDefaultContainer = _saberManager.leftSaber.GetComponent<SaberModelContainer>();
            SaberModelContainer rightDefaultContainer = _saberManager.rightSaber.GetComponent<SaberModelContainer>();
            _leftSaberDefaultTemplate = SaberModelContainer_SaberModelController(ref leftDefaultContainer);
            _rightSaberDefaultTemplate = SaberModelContainer_SaberModelController(ref rightDefaultContainer);
        }

        /// <summary>
        /// Creates a new model of the saber.
        /// </summary>
        /// <param name="saberType">The type of the saber. Setting this to null will have the saber type be of the left saber, however its color will forcibly be set to white.</param>
        /// <returns>A newly instantiated model of the saber.</returns>
        public SaberModelController NewModel(SaberType? saberType)
        {
            SaberModelController newModel = null!;
            newModel = _container.InstantiatePrefab(saberType == SaberType.SaberA ? _leftSaberDefaultTemplate : _rightSaberDefaultTemplate).GetComponent<SaberModelController>();
            foreach (var glow in newModel.SaberGlowColors())
            {
                if (!saberType.HasValue)
                    _earlyInittingGlowColors.Add(glow);
                glow.saberType = saberType.GetValueOrDefault();
            }
            foreach (var fakeGlow in newModel.SaberFakeGlowColors())
            {
                if (!saberType.HasValue)
                    _earlyInittingFakeGlowColors.Add(fakeGlow);
                fakeGlow.saberType = saberType.GetValueOrDefault();
            }
            if (!saberType.HasValue)
                newModel.SetColor(Color.white);
            return newModel;
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(SetSaberGlowColor), nameof(SetSaberGlowColor.Start))]
        private bool SetSaberGlowColor_IsStarting(SetSaberGlowColor __instance)
        {
            if (_earlyInittingGlowColors.Contains(__instance))
            {
                _earlyInittingGlowColors.Remove(__instance);
                return false;
            }
            return true;
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(SetSaberFakeGlowColor), nameof(SetSaberFakeGlowColor.Start))]
        private bool SetSaberFakeGlowColor_IsStarting(SetSaberFakeGlowColor __instance, SaberTypeObject ____saberTypeObject, ref SaberType ____saberType)
        {
            if (_earlyInittingFakeGlowColors.Contains(__instance))
            {
                if (____saberTypeObject != null)
                    ____saberType = ____saberTypeObject.saberType;
                _earlyInittingFakeGlowColors.Remove(__instance);
                return false;
            }
            return true;
        }
    }
}