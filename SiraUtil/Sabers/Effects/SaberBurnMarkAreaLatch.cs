using HarmonyLib;
using IPA.Utilities;
using SiraUtil.Affinity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SiraUtil.Sabers.Effects
{
    internal class SaberBurnMarkAreaLatch : IDisposable, IAffinity
    {
        private SaberBurnMarkArea? _saberBurnMarkArea;
        private readonly SiraSaberFactory _siraSaberFactory;
        private readonly SaberModelManager _saberModelManager;
        private readonly Queue<SiraSaber> _earlySabers = new();

        // TODO: These are all readonly fields. Figure out if there's a way to get around that.
        private static readonly FieldAccessor<SaberBurnMarkArea, Saber[]>.Accessor Sabers = FieldAccessor<SaberBurnMarkArea, Saber[]>.GetAccessor(nameof(SaberBurnMarkArea._sabers));
        private static readonly FieldAccessor<SaberBurnMarkArea, Vector3[]>.Accessor PrevBurnMarkPos = FieldAccessor<SaberBurnMarkArea, Vector3[]>.GetAccessor(nameof(SaberBurnMarkArea._prevBurnMarkPos));
        private static readonly FieldAccessor<SaberBurnMarkArea, bool[]>.Accessor PrevBurnMarkPosValid = FieldAccessor<SaberBurnMarkArea, bool[]>.GetAccessor(nameof(SaberBurnMarkArea._prevBurnMarkPosValid));
        private static readonly FieldAccessor<SaberBurnMarkArea, LineRenderer[]>.Accessor LineRenderers = FieldAccessor<SaberBurnMarkArea, LineRenderer[]>.GetAccessor(nameof(SaberBurnMarkArea._lineRenderers));

        public SaberBurnMarkAreaLatch(SiraSaberFactory siraSaberFactory, SaberModelManager saberModelManager)
        {
            _siraSaberFactory = siraSaberFactory;
            _saberModelManager = saberModelManager;
            _siraSaberFactory.SaberCreated += SiraSaberFactory_SaberCreated;
            _saberModelManager.ColorUpdated += ColorUpdated;
            _siraSaberFactory.ColorUpdated += ColorUpdated;
        }

        private void SiraSaberFactory_SaberCreated(SiraSaber siraSaber)
        {
            if (_saberBurnMarkArea == null)
                _earlySabers.Enqueue(siraSaber);
            else
                AddSaber(siraSaber.Saber);
        }

        private void ColorUpdated(Saber saber, Color color)
        {
            if (_saberBurnMarkArea == null)
                return;

            int index = _saberBurnMarkArea._sabers.IndexOf(saber);
            
            if (index == -1)
                return;

            Color.RGBToHSV(color.ColorWithAlpha(1f), out float h, out float s, out float _);
            color = Color.HSVToRGB(h, s, 1f);

            LineRenderer line = _saberBurnMarkArea._lineRenderers[index];
            line.startColor = color;
            line.endColor = color;
        }

        public void Dispose()
        {
            _siraSaberFactory.ColorUpdated -= ColorUpdated;
            _saberModelManager.ColorUpdated -= ColorUpdated;
            _siraSaberFactory.SaberCreated -= SiraSaberFactory_SaberCreated;
        }

        private void AddSaber(Saber saber)
        {
            if (_saberBurnMarkArea == null)
                return;

            Sabers(ref _saberBurnMarkArea) = _saberBurnMarkArea._sabers.AddToArray(saber);
            PrevBurnMarkPos(ref _saberBurnMarkArea) = _saberBurnMarkArea._prevBurnMarkPos.AddToArray(default);
            PrevBurnMarkPosValid(ref _saberBurnMarkArea) = _saberBurnMarkArea._prevBurnMarkPosValid.AddToArray(default);

            LineRenderer line = CreateNewLineRenderer(_saberModelManager.GetPhysicalSaberColor(saber));
            LineRenderers(ref _saberBurnMarkArea) = _saberBurnMarkArea._lineRenderers.AddToArray(line);
        }

        private LineRenderer CreateNewLineRenderer(Color initialColor)
        {
            LineRenderer newLine = UnityEngine.Object.Instantiate(_saberBurnMarkArea!._saberBurnMarkLinePrefab, Vector3.zero, Quaternion.identity, null);
            newLine.name = $"SiraUtil | {newLine.name}";
            newLine.startColor = initialColor;
            newLine.endColor = initialColor;
            newLine.positionCount = 2;
            return newLine;
        }

        [AffinityPostfix]
        [AffinityPatch(typeof(SaberBurnMarkArea), nameof(SaberBurnMarkArea.Initialize))]
        internal void BurnAreaStarting(SaberBurnMarkArea __instance)
        {
            _saberBurnMarkArea = __instance;
            // TODO: This allocates a new array on every iteration. This could be more efficient.
            foreach (SiraSaber siraSaber in _earlySabers)
                AddSaber(siraSaber.Saber);
            _earlySabers.Clear();
        }
    }
}