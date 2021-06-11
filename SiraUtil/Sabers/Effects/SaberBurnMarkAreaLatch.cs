using HarmonyLib;
using IPA.Utilities;
using SiraUtil.Affinity;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private static readonly FieldAccessor<SaberBurnMarkArea, RenderTexture[]>.Accessor Textures = FieldAccessor<SaberBurnMarkArea, RenderTexture[]>.GetAccessor("_renderTextures");
        private static readonly FieldAccessor<SaberBurnMarkArea, LineRenderer>.Accessor LinePrefab = FieldAccessor<SaberBurnMarkArea, LineRenderer>.GetAccessor("_saberBurnMarkLinePrefab");
        private static readonly FieldAccessor<SaberBurnMarkArea, int>.Accessor TextureWidth = FieldAccessor<SaberBurnMarkArea, int>.GetAccessor("_textureWidth");
        private static readonly FieldAccessor<SaberBurnMarkArea, int>.Accessor TextureHeight = FieldAccessor<SaberBurnMarkArea, int>.GetAccessor("_textureHeight");
        private int _lineFactoryIncrement;

        public SaberBurnMarkAreaLatch(SiraSaberFactory siraSaberFactory, SaberModelManager saberModelManager)
        {
            _lineFactoryIncrement = 2;
            _siraSaberFactory = siraSaberFactory;
            _saberModelManager = saberModelManager;
            _siraSaberFactory.SaberCreated += SiraSaberFactory_SaberCreated;
            _siraSaberFactory.ColorUpdated += SiraSaberFactory_ColorUpdated;
        }

        private void SiraSaberFactory_SaberCreated(SiraSaber siraSaber)
        {
            if (_saberBurnMarkArea is null)
                _earlySabers.Enqueue(siraSaber);
            else
                AddSaber(siraSaber.Saber);
        }

        private void SiraSaberFactory_ColorUpdated(Saber saber, Color color)
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
            _siraSaberFactory.ColorUpdated -= SiraSaberFactory_ColorUpdated;
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
            RenderTexture tex = CreateNewRenderTexture();
            Textures(ref _saberBurnMarkArea) = Textures(ref _saberBurnMarkArea).AddToArray(tex);
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

        private RenderTexture CreateNewRenderTexture()
        {
            RenderTexture renderTexture = new(TextureWidth(ref _saberBurnMarkArea!), TextureHeight(ref _saberBurnMarkArea!), 0, RenderTextureFormat.ARGB32);
            renderTexture.name = $"SiraUtil | SaberBurnMarkArea Texture {_lineFactoryIncrement++}";
            renderTexture.hideFlags = HideFlags.DontSave;
            return renderTexture;
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

        [AffinityPostfix]
        [AffinityPatch(typeof(SaberBurnMarkArea), nameof(SaberBurnMarkArea.LateUpdate))]
        internal void AbsoluteCarouselRenderShift(ref RenderTexture[] ____renderTextures)
        {
            // Shift every render texture down one in the array
            RenderTexture lastTexture = ____renderTextures[____renderTextures.Length - 1];
            for (int i = ____renderTextures.Length - 1; i > 0; i--)
            {
                ____renderTextures[i] = ____renderTextures[i - 1];
            }
            ____renderTextures[0] = lastTexture;
        }
    }
}