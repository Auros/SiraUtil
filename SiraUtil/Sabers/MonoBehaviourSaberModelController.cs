using UnityEngine;

namespace SiraUtil.Sabers
{
    public abstract class MonoBehaviourSaberModelController : MonoBehaviour, ISaberModelController
    {
        public abstract void Init(Transform parent, SaberType saberTypeObject);
    }
}