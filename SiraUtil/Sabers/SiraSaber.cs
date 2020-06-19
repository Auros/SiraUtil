using System;
using Zenject;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SiraUtil.Sabers
{
    public class SiraSaber : MonoBehaviour
    {

        public static SaberType nextType = SaberType.SaberB;

        private Saber _saber;
        private ColorManager _colorManager;
        private SaberTypeObject _saberTypeObject;
        private IEnumerator _changeColorCoroutine = null;
        private ISaberModelController _saberModelController;

        [Inject]
        public void Construct(ISaberModelController modelController, ColorManager colorManager)
        {
            // Woohoo! We received the saber model from Zenject!
            _saberModelController = modelController;
            _saberModelController.Init(transform, nextType);

            _colorManager = colorManager;

            // Create all the stuff thats supposed to be on the saber
            _saberTypeObject = gameObject.AddComponent<SaberTypeObject>();
            Utilities.ObjectSaberType(ref _saberTypeObject) = nextType;

            // Create and populate the saber object
            _saber = gameObject.AddComponent<Saber>();
            Utilities.SaberObjectType(ref _saber) = _saberTypeObject;
            GameObject top = new GameObject("Top");
            GameObject bottom = new GameObject("Bottom");
            top.transform.SetParent(transform);
            bottom.transform.SetParent(transform);
            top.transform.position = new Vector3(0f, 0f, 1f);

            Utilities.TopPos(ref _saber) = top.transform;
            Utilities.BottomPos(ref _saber) = bottom.transform;
            Utilities.HandlePos(ref _saber) = bottom.transform;
        }

        public void Update()
        {
            if (_saber)
            {
                Utilities.Time(ref _saber) += Time.deltaTime;
                Vector3 topPosition = Utilities.TopPos(ref _saber).position;
                Vector3 bottomPosition = Utilities.BottomPos(ref _saber).position;
                int i = 0;
                while (i < Utilities.SwingRatingCounters(ref _saber).Count)
                {
                    SaberSwingRatingCounter swingCounter = Utilities.SwingRatingCounters(ref _saber)[i];
                    if (swingCounter.didFinish)
                    {
                        swingCounter.Deinit();
                        Utilities.SwingRatingCounters(ref _saber).RemoveAt(i);
                        Utilities.UnusedSwingRatingCounters(ref _saber).Add(swingCounter);
                    }
                    else
                    {
                        i++;
                    }
                }
                SaberMovementData.Data lastAddedData = Utilities.MovementData(ref _saber).lastAddedData;
                Utilities.MovementData(ref _saber).AddNewData(topPosition, bottomPosition, Utilities.Time(ref _saber));
                if (!_saber.disableCutting)
                {
                    Utilities.Cutter(ref _saber).Cut(_saber, topPosition, bottomPosition, lastAddedData.topPos, lastAddedData.bottomPos);
                }
            }
        }

        public void SetType(SaberType type)
        {
            ChangeType(type);
            ChangeColor(_colorManager.ColorForSaberType(type));
        }

        public void ChangeType(SaberType type)
        {
            Utilities.ObjectSaberType(ref _saberTypeObject) = type;
        }

        public void SwapType(bool resetColor = false)
        {
            var saberType = _saberTypeObject.saberType == SaberType.SaberA ? SaberType.SaberB : SaberType.SaberA;
            if (resetColor)
            {
                SetType(saberType);
            }
            else
            {
                ChangeType(saberType);
            }
        }

        public void ChangeColor(Color color, float timeout = 1f)
        {
            if (_changeColorCoroutine != null)
            {
                StopCoroutine(_changeColorCoroutine);
                _changeColorCoroutine = null;
            }
            StartCoroutine(ChangeColorCoroutine(color, timeout));
        }

        private IEnumerator ChangeColorCoroutine(Color color, float timeout = 1f)
        {
            yield return new WaitForSecondsRealtime(0.05f);
            BasicSaberModelController bsmc = _saberModelController as BasicSaberModelController;
            Color tintColor = Utilities.ModelInitData(ref bsmc).trailTintColor;
            Utilities.SaberTrail(ref bsmc).color = (color * tintColor).linear;
            SetSaberGlowColor[] setSaberGlowColors = Utilities.SaberGlowColor(ref bsmc);
            SetSaberFakeGlowColor[] setSaberFakeGlowColors = Utilities.FakeSaberGlowColor(ref bsmc);
            Light light = Utilities.SaberLight(ref bsmc);

            if (IsCustomSabersInstalled())
            {
                // AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
                float timer = 0f;
                while (timer < timeout)
                {
                    UpdateColors(color, light, setSaberGlowColors, setSaberFakeGlowColors);
                    if (HasColorProperlyUpdated(gameObject, color))
                    {
                        break;
                    }
                    timer += 0.1f;
                    yield return new WaitForSecondsRealtime(0.01f);
                }
            }
            else
            {
                UpdateColors(color, light, setSaberGlowColors, setSaberFakeGlowColors);

            }
        }

        private void UpdateColors(Color color, Light light, SetSaberGlowColor[] setSaberGlowColors, SetSaberFakeGlowColor[] setSaberFakeGlowColors)
        {
            // Update the light group pairs.
            for (int i = 0; i < setSaberGlowColors.Length; i++)
            {
                setSaberGlowColors[i].OverrideColor(color);
            }
            for (int i = 0; i < setSaberFakeGlowColors.Length; i++)
            {
                setSaberFakeGlowColors[i].OverrideColor(color);
            }

            // Change the light color
            light.color = color;

            // This is for custom sabers (OMEGALUL)
            IEnumerable<Renderer> renderers = gameObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                if (renderer != null)
                {
                    foreach (Material renderMaterial in renderer.sharedMaterials)
                    {
                        if (renderMaterial == null)
                        {
                            continue;
                        }

                        if (renderMaterial.HasProperty("_Color"))
                        {
                            if (renderMaterial.HasProperty("_CustomColors"))
                            {
                                if (renderMaterial.GetFloat("_CustomColors") > 0)
                                    renderMaterial.SetColor("_Color", color);
                            }
                            else if (renderMaterial.HasProperty("_Glow") && renderMaterial.GetFloat("_Glow") > 0
                                || renderMaterial.HasProperty("_Bloom") && renderMaterial.GetFloat("_Bloom") > 0)
                            {
                                renderMaterial.SetColor("_Color", color);
                            }
                        }
                    }
                }
            }
        }

        #region Zenject
        public class Factory : PlaceholderFactory<SiraSaber>
        {

        }

        public class SaberFactory : IFactory<SiraSaber>
        {
            private readonly DiContainer _container;

            public SaberFactory(DiContainer container)
            {
                _container = container;
            }

            public SiraSaber Create()
            {
                GameObject saberObject = new GameObject("SiraUtil Saber");
                SiraSaber sira = saberObject.AddComponent<SiraSaber>();
                _container.InjectGameObject(saberObject);
                return sira;
            }
        }
        #endregion Zenject

        private bool HasColorProperlyUpdated(GameObject saber, Color target)
        {
            if (saber.transform.childCount > 1)
            {
                IEnumerable<Renderer> renderers = gameObject.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers)
                {
                    if (renderer != null)
                    {
                        foreach (Material renderMaterial in renderer.sharedMaterials)
                        {
                            if (renderMaterial == null)
                            {
                                continue;
                            }

                            if (renderMaterial.HasProperty("_Color"))
                            {
                                if (renderMaterial.GetColor("_Color") == target)
                                    return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool IsColorCloseToOtherColor(Color color, Color newColor, float threshold = 0.01f)
        {
            float r = Math.Abs(color.r - newColor.r);
            float g = Math.Abs(color.g - newColor.g);
            float b = Math.Abs(color.b - newColor.b);
            float a = Math.Abs(color.a - newColor.a);
            return !(r > threshold || g > threshold || b > threshold || a > threshold);
        }

        private bool _alreadyFound = false;
        private bool _customSaberStatus = false;
        private bool IsCustomSabersInstalled()
        {
            if (_alreadyFound == true)
                return _customSaberStatus;

            var data = IPA.Loader.PluginManager.GetPlugin("Custom Sabers");
        
            _alreadyFound = true;
            if (data == null)
            {
                _customSaberStatus = false;
            }
            else
            {
                _customSaberStatus = true;
            }
            return _customSaberStatus;
        }
    }
}