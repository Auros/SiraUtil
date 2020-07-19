using Zenject;
using UnityEngine;

namespace SiraUtil.Sabers
{
    public class SaberTest : MonoBehaviour
    {
        private SiraSaber.Factory _saberFactory;
        readonly System.Random random = new System.Random();

        [Inject]
        public void Construct(PlayerController controller, SiraSaber.Factory saberFactory)
        {
            _saberFactory = saberFactory;

            for (int i = 0; i < 50; i++)
            {
                var saber = _saberFactory.Create();
                saber.transform.rotation = Quaternion.Euler(random.Next(0, 360), random.Next(0, 360), random.Next(0, 360));
                saber.ChangeColor(new Color(Random.value, Random.value, Random.value, 1f));
            }

            controller.leftSaber.ChangeColor(Color.yellow);
        }
    }
}