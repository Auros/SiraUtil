using IPA.Utilities;
using UnityEngine;
using UnityEngine.XR;

namespace SiraUtil.Tools.FPFC
{
    internal class GameTransformFPFCListener : IFPFCListener
    {
        private readonly PlayerTransforms _playerTransforms;
        private readonly Transform _originalHeadTransform;
        private readonly Transform _originalSaberParent;
        private readonly Transform _fpfcHeadTransform;
        private readonly VRController _rightHand;
        private readonly VRController _leftHand;

        private const string _headTransform = "_headTransform";

        public GameTransformFPFCListener(SaberManager saberManager, PlayerTransforms playerTransforms)
        {
            _leftHand = saberManager.leftSaber.GetComponentInParent<VRController>();
            _rightHand = saberManager.rightSaber.GetComponentInParent<VRController>();
            _originalSaberParent = _rightHand.transform.parent;

            _playerTransforms = playerTransforms;
            _fpfcHeadTransform = new GameObject("FPFC Player Head").transform;
            _originalHeadTransform = _playerTransforms.GetField<Transform, PlayerTransforms>(_headTransform);
        }

        public void Enabled()
        {
            _leftHand.transform.SetParent(null);
            _rightHand.transform.SetParent(null);
            _playerTransforms.SetField(_headTransform, _fpfcHeadTransform);
        }

        public void Disabled()
        {
            if (XRSettings.enabled)
            {
                _leftHand.transform.SetParent(_originalSaberParent);
                _rightHand.transform.SetParent(_originalSaberParent);
                _playerTransforms.SetField(_headTransform, _originalHeadTransform);
            }
        }
    }
}