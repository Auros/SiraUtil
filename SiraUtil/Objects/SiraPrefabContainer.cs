using Zenject;
using UnityEngine;

namespace SiraUtil.Objects
{
    public class SiraPrefabContainer : MonoBehaviour
    {
        [SerializeField]
        private GameObject _prefab;
        public GameObject Prefab
        {
            get => _prefab;
            set
            {
                _prefab = Instantiate(value);
                _prefab.transform.SetParent(transform);
                _prefab.SetActive(false);
            }
        }

        public void Start()
        {
            if (transform.childCount > 0)
            {
                _prefab = transform.GetChild(0).gameObject;
            }
        }

        public class Pool : MonoMemoryPool<SiraPrefabContainer>
        {

        }
    }
}