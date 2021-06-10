using SiraUtil.Sabers;
using UnityEngine;
using Zenject;

namespace SiraUtil.Suite.Tests.Sabers
{
    internal class SpawnFullSaberTest : IInitializable
    {
        private readonly SaberManager _saberManager;
        private readonly SiraSaberFactory _siraSaberFactory;

        public SpawnFullSaberTest(SaberManager saberManager, SiraSaberFactory siraSaberFactory)
        {
            _saberManager = saberManager;
            _siraSaberFactory = siraSaberFactory;
        }

        public void Initialize()
        {
            SiraSaber saber = _siraSaberFactory.Spawn(SaberType.SaberA);
        
            saber.transform.SetParent(_saberManager.leftSaber.transform);
            saber.transform.localPosition = new Vector3(0.25f, 0f, 0f);
            saber.transform.localRotation = Quaternion.Euler(0, 90f, 0f);

            saber.SetColor(Color.yellow);
        }
    }
}