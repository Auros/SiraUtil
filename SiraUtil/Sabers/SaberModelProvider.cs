using SiraUtil.Affinity;
using SiraUtil.Extras;
using SiraUtil.Interfaces;
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
        private readonly SaberModelContainer _localLeftContainer;
        private readonly SaberModelContainer _localRightContainer;
        private readonly SaberModelRegistration _activeSaberModelRegistration;
        private readonly SaberModelRegistration _defaultSaberModelRegistration;
        private readonly HashSet<SetSaberGlowColor> _earlyInittingGlowColors = new();
        private readonly HashSet<SetSaberFakeGlowColor> _earlyInittingFakeGlowColors = new();

        internal SaberModelProvider(SiraLog siraLog, DiContainer container, SaberManager saberManager, List<SaberModelRegistration> saberModelRegistrations)
        {
            _siraLog = siraLog;
            _container = container;
            _saberManager = saberManager;

            _localLeftContainer = _saberManager.leftSaber.GetComponent<SaberModelContainer>();
            _localRightContainer = _saberManager.rightSaber.GetComponent<SaberModelContainer>();
            _defaultSaberModelRegistration = new(_localLeftContainer._saberModelControllerPrefab, _localRightContainer._saberModelControllerPrefab);

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

                    SaberTrail defaultTrail = defaultPrefab._saberTrail;
                    SaberTrail pseudoTrail = gameObject.AddComponent<SaberTrail>();

                    // Give a new trail that doesn't really do anything. However, by default the SaberModelController needs a trail, so...
                    pseudoTrail._granularity = defaultTrail._granularity;
                    pseudoTrail._trailDuration = defaultTrail._trailDuration;
                    pseudoTrail._samplingFrequency = defaultTrail._samplingFrequency;
                    pseudoTrail._whiteSectionMaxDuration = defaultTrail._whiteSectionMaxDuration;
                    pseudoTrail._trailRendererPrefab = defaultTrail._trailRendererPrefab;
                    pseudoTrail.enabled = false;

                    newModel = (_container.InstantiateComponent(type, gameObject) as SaberModelController)!;
                    newModel._saberTrail = pseudoTrail;
                    newModel._setSaberGlowColors = Array.Empty<SetSaberGlowColor>();
                    newModel._setSaberFakeGlowColors = Array.Empty<SetSaberFakeGlowColor>();
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
        private void DefaultSaberPrefabSwap(ref SaberModelContainer __instance, ref SaberModelController ____saberModelControllerPrefab, ref Saber ____saber)
        {
            // If the SaberModelContainer doesn't belong to us, don't do anything.
            if (__instance != _localLeftContainer && __instance != _localRightContainer)
                return;

            if (_activeSaberModelRegistration == _defaultSaberModelRegistration)
                return;

            ____saberModelControllerPrefab = NewModel(____saber.saberType);
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(SaberModelController), nameof(SaberModelController.Init))]
        private bool PreInit(ref SaberModelController __instance, Transform parent, Saber saber)
        {
            return !__instance.TryGetComponent(out IPreSaberModelInit runner) || runner.PreInit(parent, saber);
        }


        [AffinityPostfix]
        [AffinityPatch(typeof(SaberModelController), nameof(SaberModelController.Init))]
        private void PostInit(ref SaberModelController __instance, Transform parent, Saber saber)
        {
            if (!__instance.TryGetComponent(out IPostSaberModelInit runner))
            {
                return;
            }

            runner.PostInit(parent, saber);
        }
    }
}