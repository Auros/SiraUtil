using HMUI;
using IPA.Utilities;
using System;
using Zenject;

namespace SiraUtil.Submission
{
    internal abstract class SubmissionDisplayer : IInitializable, IDisposable
    {
        private readonly FlowCoordinator _targetFlowCoordinator;

        [Inject]
        private readonly ResultsViewController _resultsViewController = null!;

        [Inject]
        private readonly SubmissionDataContainer _submissionDataContainer = null!;

        [Inject]
        private readonly SiraSubmissionViewController _siraSubmissionViewController = null!;

        public SubmissionDisplayer(FlowCoordinator targetFlowCoordinator)
        {
            _targetFlowCoordinator = targetFlowCoordinator;
        }

        public void Initialize()
        {
            _resultsViewController.didActivateEvent += ResultsViewController_didActivateEvent;
            _resultsViewController.didDeactivateEvent += ResultsViewController_didDeactivateEvent;
        }

        private void ResultsViewController_didActivateEvent(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (_submissionDataContainer.Disabled)
            {
                _targetFlowCoordinator.InvokeMethod<object, FlowCoordinator>("SetBottomScreenViewController", _siraSubmissionViewController, ViewController.AnimationType.In);
                _siraSubmissionViewController.Enabled(true);
                _siraSubmissionViewController.SetText($"<size=115%><color=#f00e0e>Score Submission Disabled By</color></size>\n{_submissionDataContainer.Read()}");
            }
        }

        private void ResultsViewController_didDeactivateEvent(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            _submissionDataContainer.Disabled = false;
            _siraSubmissionViewController.Enabled(false);
            if (_siraSubmissionViewController.isInViewControllerHierarchy)
                _targetFlowCoordinator.InvokeMethod<object, FlowCoordinator>("SetBottomScreenViewController", null, ViewController.AnimationType.Out);
        }

        public void Dispose()
        {
            _resultsViewController.didDeactivateEvent -= ResultsViewController_didDeactivateEvent;
            _resultsViewController.didActivateEvent -= ResultsViewController_didActivateEvent;
        }
    }
}