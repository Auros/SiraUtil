using UnityEngine;
using System.Linq;
using SiraUtil.Interfaces;
using System.Collections.Generic;

namespace SiraUtil.Sabers
{
    /// <summary>
    /// An upgraded version of the SaberBurnMarkArea class which has support for more than two sabers.
    /// </summary>
    public class SiraSaberBurnMarkArea : SaberBurnMarkArea, ISaberRegistrar
    {
        private int _renderIndex = 0;
        private readonly List<SaberBurnDatum> _saberBurnData = new List<SaberBurnDatum>();

        private bool _initted = false;

        /// <summary>
        /// The start method .
        /// </summary>
        public override void Start()
        {
            if (!_initted)
            {
                base.Start();
                for (int i = 0; i < 2; i++)
                {
                    if (_sabers[i])
                    {
                        var saberBurnDatum = new SaberBurnDatum
                        {
                            saber = _sabers[i],
                            lineRenderer = _lineRenderers[i],
                            renderTexture = _renderTextures[i],
                            prevBurnMarkPos = _prevBurnMarkPos[i],
                            prevBurnMarkPosValid = _prevBurnMarkPosValid[i]
                        };
                        _saberBurnData.Add(saberBurnDatum);
                    }
                }
                _initted = true;
            }
        }

        /// <summary>
        /// The initialization method.
        /// </summary>
        /// <param name="saberManager">The saber manager used to gather saber references.</param>
        public void Initialize(SaberManager saberManager)
        {
            if (!_initted)
            {
                base.Start();
                for (int i = 0; i < 2; i++)
                {
                    if (_sabers[i])
                    {
                        var saberBurnDatum = new SaberBurnDatum
                        {
                            saber = _sabers[i],
                            lineRenderer = _lineRenderers[i],
                            renderTexture = _renderTextures[i],
                            prevBurnMarkPos = _prevBurnMarkPos[i],
                            prevBurnMarkPosValid = _prevBurnMarkPosValid[i]
                        };
                        _saberBurnData.Add(saberBurnDatum);
                    }
                }
                _initted = true;
            }
            _sabers = new Saber[2];
            _sabers[0] = saberManager.leftSaber;
            _sabers[1] = saberManager.rightSaber;
            _saberBurnData[0].saber = _sabers[0];
            _saberBurnData[1].saber = _sabers[1];
        }

        /// <summary>
        /// The late update method.
        /// </summary>
        public override void LateUpdate()
        {
            for (int i = 0; i < _saberBurnData.Count; i++)
            {
                Saber saber = _saberBurnData[i].saber;
                LineRenderer lineRenderer = _saberBurnData[i].lineRenderer;

                Vector3 zero = Vector3.zero;
                bool flag = saber != null && saber.isActiveAndEnabled && GetBurnMarkPos(saber.saberBladeBottomPos, saber.saberBladeTopPos, out zero);
                if (flag && _saberBurnData[i].prevBurnMarkPosValid)
                {
                    Vector3 vector = zero - _saberBurnData[i].prevBurnMarkPos;
                    float magnitude = vector.magnitude;
                    int num2 = (int)(magnitude / 0.01f);
                    int num4 = (num2 > 0) ? num2 : 1;
                    Vector3 normalized = new Vector3(vector.z, 0f, -vector.x).normalized;
                    int num5 = 0;
                    while (num5 <= num4 && num5 < _linePoints.Length)
                    {
                        Vector3 vector2 = _saberBurnData[i].prevBurnMarkPos + vector * num5 / num4;
                        vector2 += normalized * Random.Range(-_blackMarkLineRandomOffset, _blackMarkLineRandomOffset);
                        _linePoints[num5] = WorldToCameraBurnMarkPos(vector2);
                        num5++;
                    }
                    lineRenderer.positionCount = num4 + 1;
                    lineRenderer.SetPositions(_linePoints);
                    lineRenderer.enabled = true;
                }
                else
                {
                    lineRenderer.enabled = false;
                }
                _saberBurnData[i].prevBurnMarkPosValid = flag;
                _saberBurnData[i].prevBurnMarkPos = zero;
            }
            if (_saberBurnData.Any(sbd => sbd.lineRenderer.enabled))
            {
                _camera.Render();
            }
            _renderIndex = (_renderIndex != _saberBurnData.Count - 1) ? _renderIndex + 1 : 0;
            int nextToRender = _renderIndex + 1 != _saberBurnData.Count ? _renderIndex + 1 : 0;
            _camera.targetTexture = _saberBurnData[1].renderTexture;
            _renderer.sharedMaterial.mainTexture = _saberBurnData[1].renderTexture;
            _fadeOutMaterial.mainTexture = _saberBurnData[0].renderTexture;
            _fadeOutMaterial.SetFloat(_fadeOutStrengthShaderPropertyID, Mathf.Max(0f, 1f - Time.deltaTime * _burnMarksFadeOutStrength));
            Graphics.Blit(_saberBurnData[0].renderTexture, _saberBurnData[1].renderTexture, _fadeOutMaterial);
            RenderTexture renderTexture = _saberBurnData[0].renderTexture;
            _saberBurnData[0].renderTexture = _saberBurnData[1].renderTexture;
            _saberBurnData[1].renderTexture = renderTexture;
        }

        /// <summary>
        /// The destroy method.
        /// </summary>
        public override void OnDestroy()
        {
            foreach (SaberBurnDatum saberBurnDatum in _saberBurnData)
            {
                if (saberBurnDatum.lineRenderer)
                {
                    Destroy(saberBurnDatum.lineRenderer.gameObject);
                }
                if (saberBurnDatum.renderTexture)
                {
                    saberBurnDatum.renderTexture.Release();
                    EssentialHelpers.SafeDestroy(saberBurnDatum.renderTexture);
                }
            }
        }

        /// <summary>
        /// The OnEnable method.
        /// </summary>
        public override void OnEnable()
        {
            foreach (var sbm in _saberBurnData)
            {
                if (sbm.lineRenderer != null)
                {
                    sbm.lineRenderer.gameObject.SetActive(true);
                }
            }
        }

        /// <summary>
        /// The disable method.
        /// </summary>
        public override void OnDisable()
        {
            foreach (var sbm in _saberBurnData)
            {
                if (sbm.lineRenderer != null)
                {
                    sbm.lineRenderer.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Registers a saber into the Sira saber burn mark area manager.
        /// </summary>
        /// <param name="saber">The saber to register.</param>
        public void RegisterSaber(Saber saber)
        {
            var newSaberDatum = new SaberBurnDatum
            {
                saber = saber,
                prevBurnMarkPos = default,
                prevBurnMarkPosValid = false,
                lineRenderer = Instantiate(_saberBurnMarkLinePrefab, Vector3.zero, Quaternion.identity, null),
                renderTexture = new RenderTexture(_textureWidth, _textureHeight, 0, RenderTextureFormat.ARGB32)
                {
                    hideFlags = HideFlags.DontSave,
                    name = "SaberBurnMarkArea Texture: " + saber.name
                }
            };

            Color color = saber.GetColor();
            Color.RGBToHSV(color, out float h, out float s, out _);
            color = Color.HSVToRGB(h, s, 1f);
            newSaberDatum.lineRenderer.startColor = color;
            newSaberDatum.lineRenderer.endColor = color;
            newSaberDatum.lineRenderer.positionCount = 2;
            _saberBurnData.Add(newSaberDatum);
        }

        /// <summary>
        /// Unregisters a saber in the Sira saber burn mark area manager.
        /// </summary>
        /// <param name="saber">The saber to unregister.</param>
        public void UnregisterSaber(Saber saber)
        {
            SaberBurnDatum saberBurnDatum = _saberBurnData.Where(sbd => sbd.saber == saber).FirstOrDefault();
            if (saberBurnDatum != null)
            {
                if (saberBurnDatum.lineRenderer)
                {
                    Destroy(saberBurnDatum.lineRenderer.gameObject);
                }
                if (saberBurnDatum.renderTexture)
                {
                    saberBurnDatum.renderTexture.Release();
                    EssentialHelpers.SafeDestroy(saberBurnDatum.renderTexture);
                }
                _saberBurnData.Remove(saberBurnDatum);
            }
        }

        /// <summary>
        /// Forces a registered saber to change its color.
        /// </summary>
        /// <param name="saber">The saber being forced to change its color.</param>
        public void ChangeColor(Saber saber)
        {
            SaberBurnDatum saberBurnDatum = _saberBurnData.FirstOrDefault(sbd => sbd.saber == saber);
            if (saberBurnDatum != null)
            {
                Color color = saber.GetColor();
                Color.RGBToHSV(color, out float h, out float s, out _);
                color = Color.HSVToRGB(h, s, 1f);
                saberBurnDatum.lineRenderer.startColor = color;
                saberBurnDatum.lineRenderer.endColor = color;
                saberBurnDatum.lineRenderer.positionCount = 2;
            }
        }

        private class SaberBurnDatum
        {
            public Saber saber;
            public Vector3 prevBurnMarkPos;
            public bool prevBurnMarkPosValid;
            public LineRenderer lineRenderer;
            public RenderTexture renderTexture;
        }
    }
}