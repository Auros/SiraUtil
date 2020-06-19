using UnityEngine;
using Zenject;

namespace SiraUtil.Sabers
{
    public class SaberTest : MonoBehaviour
    {
        private PlayerController _playerController;
        private SiraSaber.Factory _saberFactory;

        private SiraSaber _saberThree;

        [Inject]
        public void Construct(PlayerController playerController, SiraSaber.Factory saberFactory)
        {
            _playerController = playerController;

            _saberFactory = saberFactory;
            _saberThree = _saberFactory.Create();

            _saberThree.ChangeColor(Color.green);

            //saber.transform.SetParent(_playerController.rightSaber.transform);
            _saberThree.transform.parent = _playerController.rightSaber.transform;
            _saberThree.transform.Rotate(Vector3.up * 90f);
        }

        public void Update()
        {
            if (_saberThree)
            {
                
                
            }
        }
    }
}