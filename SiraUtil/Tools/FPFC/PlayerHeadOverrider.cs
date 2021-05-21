using UnityEngine;
using Zenject;

namespace SiraUtil.Tools.FPFC
{
    internal class PlayerHeadOverrider : IInitializable
    {
        private readonly PlayerTransforms _playerTransforms;

        public PlayerHeadOverrider(PlayerTransforms playerTransforms)
        {
            _playerTransforms = playerTransforms;
        }

        public void Initialize()
        {
            _playerTransforms.OverrideHeadPos(Vector3.zero);
        }
    }
}