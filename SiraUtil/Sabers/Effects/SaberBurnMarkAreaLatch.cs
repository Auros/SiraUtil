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

        private static readonly FieldAccessor<SaberBurnMarkArea, Saber[]>.Accessor Sabers = FieldAccessor<SaberBurnMarkArea, Saber[]>.GetAccessor("_sabers");
        private static readonly FieldAccessor<SaberBurnMarkArea, Vector3[]>.Accessor Points = FieldAccessor<SaberBurnMarkArea, Vector3[]>.GetAccessor("_linePoints");
        private static readonly FieldAccessor<SaberBurnMarkArea, LineRenderer[]>.Accessor Lines = FieldAccessor<SaberBurnMarkArea, LineRenderer[]>.GetAccessor("_lineRenderers");
        private static readonly FieldAccessor<SaberBurnMarkArea, Vector3[]>.Accessor PreviousMarks = FieldAccessor<SaberBurnMarkArea, Vector3[]>.GetAccessor("_prevBurnMarkPos");
        private static readonly FieldAccessor<SaberBurnMarkArea, bool[]>.Accessor PreviousMarksValid = FieldAccessor<SaberBurnMarkArea, bool[]>.GetAccessor("_prevBurnMarkPosValid");
        private static readonly FieldAccessor<SaberBurnMarkArea, LineRenderer>.Accessor LinePrefab = FieldAccessor<SaberBurnMarkArea, LineRenderer>.GetAccessor("_saberBurnMarkLinePrefab");

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

            int index = Sabers(ref _saberBurnMarkArea).IndexOf(saber);
            
            if (index == -1)
                return;

            Color.RGBToHSV(color.ColorWithAlpha(1f), out float h, out float s, out float _);
            color = Color.HSVToRGB(h, s, 1f);

            LineRenderer line = Lines(ref _saberBurnMarkArea)[index];
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

            Saber[] sabers = Sabers(ref _saberBurnMarkArea) = Sabers(ref _saberBurnMarkArea).AddToArray(saber);
            PreviousMarks(ref _saberBurnMarkArea) = PreviousMarks(ref _saberBurnMarkArea).AddToArray(default);
            PreviousMarksValid(ref _saberBurnMarkArea) = PreviousMarksValid(ref _saberBurnMarkArea).AddToArray(default);

            LineRenderer line = CreateNewLineRenderer(_saberModelManager.GetPhysicalSaberColor(saber));
            Lines(ref _saberBurnMarkArea) = Lines(ref _saberBurnMarkArea).AddToArray(line);
        }

        private LineRenderer CreateNewLineRenderer(Color initialColor)
        {
            LineRenderer newLine = UnityEngine.Object.Instantiate(LinePrefab(ref _saberBurnMarkArea!), Vector3.zero, Quaternion.identity, null);
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