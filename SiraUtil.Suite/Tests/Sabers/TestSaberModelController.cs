using SiraUtil.Interfaces;
using UnityEngine;

namespace SiraUtil.Suite.Tests.Sabers
{
    internal class TestSaberModelController : SaberModelController, IColorable
    {
        public Color Color { get; set; }

        public override void Init(Transform parent, Saber saber)
        {
            Color = _colorManager.ColorForSaberType(saber.saberType);
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            g.transform.localScale *= 0.1f;
            g.transform.SetParent(transform);
            g.transform.localPosition = new Vector3(0f, 0f, 1f);
            base.Init(parent, saber);
        }
    }
}