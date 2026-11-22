using HMUI;
using System.Linq;
using TMPro;
using UnityEngine;

namespace SiraUtil.Submissions
{
    internal class SiraSubmissionViewController : ViewController
    {
        private CurvedTextMeshPro? _curvedText;

        public override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            if (firstActivation)
            {
                GameObject textGameObject = gameObject;
                _curvedText = textGameObject.AddComponent<CurvedTextMeshPro>();

                // TMP_Settings.defaultFontAsset is null in the game, so without a font the text mesh is never generated.
                if (Resources.FindObjectsOfTypeAll<TMP_FontAsset>().FirstOrDefault(f => f.name == "Teko-Medium SDF") is { } font)
                {
                    _curvedText.font = font;
                }

                textGameObject.transform.SetParent(transform);
                (textGameObject.transform as RectTransform)!.sizeDelta = new Vector2(40f, 100f);
                (textGameObject.transform as RectTransform)!.anchorMin = new Vector2(0f, 0f);
                (textGameObject.transform as RectTransform)!.anchorMax = new Vector2(0f, 0f);
                textGameObject.transform.localPosition = new Vector2(0f, -51f);
                textGameObject.transform.localScale = Vector3.one;
                _curvedText.alignment = TextAlignmentOptions.Center;
                _curvedText.lineSpacing = -45f;
                _curvedText.fontSize = 3.5f;
                _curvedText.gameObject.SetActive(false);
            }
        }

        public void Enabled(bool value)
        {
            if (_curvedText != null)
            {
                _curvedText.gameObject.SetActive(value);
            }
        }

        public void SetText(string text)
        {
            if (_curvedText != null)
            {
                _curvedText.text = text;
            }
        }
    }
}