using HarmonyLib;
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
            if (_saberBurnMarkArea is null)
                _earlySabers.Enqueue(siraSaber);
            else
                AddSaber(siraSaber.Saber);
        }

        private void ColorUpdated(Saber saber, Color color)
        {
            if (_saberBurnMarkArea is null)
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
            if (_saberBurnMarkArea is null)
                return;

            Saber[] sabers = _saberBurnMarkArea._sabers = _saberBurnMarkArea._sabers.AddToArray(saber);
            _saberBurnMarkArea._prevBurnMarkPos = _saberBurnMarkArea._prevBurnMarkPos.AddToArray(default);
            _saberBurnMarkArea._prevBurnMarkPosValid = _saberBurnMarkArea._prevBurnMarkPosValid.AddToArray(default);

            LineRenderer line = CreateNewLineRenderer(_saberModelManager.GetPhysicalSaberColor(saber));
            _saberBurnMarkArea._lineRenderers = _saberBurnMarkArea._lineRenderers.AddToArray(line);
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
        [AffinityPatch(typeof(SaberBurnMarkArea), nameof(SaberBurnMarkArea.Start))]
        internal void BurnAreaStarting(SaberBurnMarkArea __instance)
        {
            _saberBurnMarkArea = __instance;
            foreach (var siraSaber in _earlySabers)
                AddSaber(siraSaber.Saber);
            _earlySabers.Clear();
        }
    }
}