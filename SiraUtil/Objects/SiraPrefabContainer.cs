using Zenject;
using UnityEngine;

namespace SiraUtil.Objects
{
    /// <summary>
    /// A container to wrap a prefab into.
    /// </summary>
    public class SiraPrefabContainer : MonoBehaviour
    {
        [SerializeField]
        private GameObject _prefab = null!;

        /// <summary>
        /// The prefab in this container.
        /// </summary>
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

        /// <summary>
        /// The start method.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Correctness", "UNT0021:Prefer protected Unity Message.", Justification = "Already part of the public API")]
        public void Start()
        {
            if (transform.childCount > 0)
            {
                _prefab = transform.GetChild(0).gameObject;
            }
        }

        /// <summary>
        /// The Zenject pool used to create more prefabs.
        /// </summary>
        public class Pool : MonoMemoryPool<SiraPrefabContainer>
        {

        }
    }
}