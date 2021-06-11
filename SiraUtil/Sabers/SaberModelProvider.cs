using IPA.Utilities;
using SiraUtil.Affinity;
using SiraUtil.Extras;
using SiraUtil.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace SiraUtil.Sabers
{
    /// <summary>
    /// Provides copies of the active saber model.
    /// </summary>
    public class SaberModelProvider : IAffinity
    {
        private readonly SiraLog _siraLog;
        private readonly DiContainer _container;
        private readonly SaberManager _saberManager;
        private readonly SaberModelRegistration _activeSaberModelRegistration;
        private readonly SaberModelRegistration _defaultSaberModelRegistration;
        private readonly HashSet<SetSaberGlowColor> _earlyInittingGlowColors = new();
        private readonly HashSet<SetSaberFakeGlowColor> _earlyInittingFakeGlowColors = new();
        private static readonly FieldAccessor<SaberModelContainer, SaberModelController>.Accessor SaberModelContainer_SaberModelController = FieldAccessor<SaberModelContainer, SaberModelController>.GetAccessor("_saberModelControllerPrefab");

        internal SaberModelProvider(SiraLog siraLog, DiContainer container, SaberManager saberManager, List<SaberModelRegistration> saberModelRegistrations)
        {
            _siraLog = siraLog;
            _container = container;
            _saberManager = saberManager;

            SaberModelContainer leftDefaultContainer = _saberManager.leftSaber.GetComponent<SaberModelContainer>();
            SaberModelContainer rightDefaultContainer = _saberManager.rightSaber.GetComponent<SaberModelContainer>();
            _defaultSaberModelRegistration = new(SaberModelContainer_SaberModelController(ref leftDefaultContainer), SaberModelContainer_SaberModelController(ref rightDefaultContainer), -1);

            List<SaberModelRegistration> registrations = new();
            registrations.Add(_defaultSaberModelRegistration);
            registrations.AddRange(saberModelRegistrations);

            _activeSaberModelRegistration = registrations.OrderByDescending(r => r.Priority).First();
        }

        /// <summary>
        /// Creates a new model of the saber.
        /// </summary>
        /// <param name="saberType">The type of the saber. Setting this to null will have the saber type be of the left saber, however its color will forcibly be set to white.</param>
        /// <returns>A newly instantiated model of the saber.</returns>
        public SaberModelController NewModel(SaberType? saberType)
        {
            SaberModelController newModel = CreateNew(saberType ?? SaberType.SaberA);
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

        private SaberModelController CreateNew(SaberType saberType)
        {
            SaberModelController newModel = null!;
            try
            {
                if (_activeSaberModelRegistration.LeftType is not null && _activeSaberModelRegistration.RightType is not null)
                {
                    SaberModelController defaultPrefab = saberType == SaberType.SaberA ? _defaultSaberModelRegistration.LeftTemplate! : _defaultSaberModelRegistration.RightTemplate!;
                    Type type = saberType == SaberType.SaberA ? _activeSaberModelRegistration.LeftType : _activeSaberModelRegistration.RightType;
                    GameObject gameObject = new(type.Name);
                    gameObject.SetActive(false);

                    SaberTrail defaultTrail = SaberExtensions.SaberModelController_SaberTrail(ref defaultPrefab);
                    SaberTrail pseudoTrail = gameObject.AddComponent<SaberTrail>();

                    // TODO: Convert to accessors
                    // Give a new trail that doesn't really do anything. However, by default the SaberModelController needs a trail, so...
                    pseudoTrail.SetField("_granularity", defaultTrail.GetField<int, SaberTrail>("_granularity"));
                    pseudoTrail.SetField("_trailDuration", defaultTrail.GetField<float, SaberTrail>("_trailDuration"));
                    pseudoTrail.SetField("_samplingFrequency", defaultTrail.GetField<int, SaberTrail>("_samplingFrequency"));
                    pseudoTrail.SetField("_whiteSectionMaxDuration", defaultTrail.GetField<float, SaberTrail>("_whiteSectionMaxDuration"));
                    pseudoTrail.SetField("_trailRendererPrefab", defaultTrail.GetField<SaberTrailRenderer, SaberTrail>("_trailRendererPrefab"));
                    pseudoTrail.enabled = false;

                    newModel = (_container.InstantiateComponent(type, gameObject) as SaberModelController)!;
                    SaberExtensions.SaberModelController_SaberTrail(ref newModel) = pseudoTrail;
                    SaberExtensions.SaberModelController_SetSaberGlowColors(ref newModel) = Array.Empty<SetSaberGlowColor>();
                    SaberExtensions.SaberModelController_SetSaberFakeGlowColors(ref newModel) = Array.Empty<SetSaberFakeGlowColor>();
                    gameObject.SetActive(true);
                }
                else if (_activeSaberModelRegistration.LeftTemplate != null && _activeSaberModelRegistration.RightTemplate != null)
                {
                    newModel = _container.InstantiatePrefab(saberType == SaberType.SaberA ? _activeSaberModelRegistration.LeftTemplate : _activeSaberModelRegistration.RightTemplate).GetComponent<SaberModelController>();
                }
                else if (_activeSaberModelRegistration.LeftInstruction != null && _activeSaberModelRegistration.RightInstruction != null)
                {
                    if (saberType == SaberType.SaberA)
                    {
                        newModel = _activeSaberModelRegistration.LeftInstruction.Invoke();
                    }
                    else
                    {
                        newModel = _activeSaberModelRegistration.RightInstruction.Invoke();
                    }
                    _container.InjectGameObject(newModel.gameObject);
                }
                else
                {
                    throw new Exception("Invalid Saber Registration");
                }
            }
            catch (Exception e)
            {
                _siraLog.Error($"An error occured while trying to create the saber model, using the default saber model! {e.Message}");
                _siraLog.Debug(e);

                newModel = _container.InstantiatePrefab(saberType == SaberType.SaberA ? _defaultSaberModelRegistration.LeftTemplate : _defaultSaberModelRegistration.RightTemplate).GetComponent<SaberModelController>();
            }
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

        [AffinityPrefix]
        [AffinityPatch(typeof(SaberModelContainer), nameof(SaberModelContainer.Start))]
        private void DefaultSaberPrefabSwap(ref SaberModelController ____saberModelControllerPrefab, ref Saber ____saber)
        {
            if (_activeSaberModelRegistration == _defaultSaberModelRegistration)
                return;

            ____saberModelControllerPrefab = NewModel(____saber.saberType);
        }
    }
}