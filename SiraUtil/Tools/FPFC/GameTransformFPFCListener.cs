using UnityEngine;
using UnityEngine.XR;

namespace SiraUtil.Tools.FPFC
{
    internal class GameTransformFPFCListener : IFPFCListener
    {
        private readonly PlayerTransforms _playerTransforms;
        private readonly Transform _originalHeadTransform;
        private readonly Transform _fpfcHeadTransform;
        private readonly SaberManager _saberManager;
        private Transform _originalSaberParent = null!;
        private VRController _rightHand = null!;
        private VRController _leftHand = null!;

        public GameTransformFPFCListener(SaberManager saberManager, PlayerTransforms playerTransforms)
        {
            _saberManager = saberManager;
            _playerTransforms = playerTransforms;
            _fpfcHeadTransform = new GameObject("FPFC Player Head").transform;
            _originalHeadTransform = _playerTransforms._headTransform;
        }

        public void Enabled()
        {
            _leftHand = _saberManager.leftSaber.GetComponentInParent<VRController>();
            _rightHand = _saberManager.rightSaber.GetComponentInParent<VRController>();
            _originalSaberParent = _rightHand.transform.parent;

            _leftHand.transform.SetParent(null);
            _rightHand.transform.SetParent(null);
            _playerTransforms._headTransform = _fpfcHeadTransform;
        }

        public void Disabled()
        {
            if (XRSettings.enabled)
            {
                _leftHand.transform.SetParent(_originalSaberParent);
                _rightHand.transform.SetParent(_originalSaberParent);
                _playerTransforms._headTransform = _originalHeadTransform;
            }
        }
    }
}