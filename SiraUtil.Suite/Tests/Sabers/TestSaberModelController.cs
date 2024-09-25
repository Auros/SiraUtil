using SiraUtil.Interfaces;
using UnityEngine;
using Zenject;

namespace SiraUtil.Suite.Tests.Sabers
{
    internal class TestSaberModelController : SaberModelController, IColorable, IPreSaberModelInit
    {
        public Color Color { get; set; }

        [Inject]
        private readonly ColorManager _colorManager = null!;

        public bool PreInit(Transform parent, Saber saber)
        {
            Color = _colorManager.ColorForSaberType(saber.saberType);
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            g.transform.localScale *= 0.1f;
            g.transform.SetParent(transform);
            g.transform.localPosition = new Vector3(0f, 0f, 1f);
            return true;
        }
    }
}