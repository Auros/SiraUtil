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
        }
    }
}