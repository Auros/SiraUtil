using UnityEngine;

namespace SiraUtil.Suite.Tests.Sabers
{
    internal class TestSaberModelController : SaberModelController
    {
        public override void Init(Transform parent, Saber saber)
        {
            Plugin.Log.Info("ASDERJLHGSRTH DSRFTGH DFSGH");
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
            g.transform.localScale *= 0.1f;
            g.transform.SetParent(transform);
            g.transform.localPosition = Vector3.zero;
            base.Init(parent, saber);
        }
    }
}