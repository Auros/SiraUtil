using HMUI;
using System;
using Zenject;

namespace SiraUtil.Submissions
{
    internal abstract class SubmissionDisplayer : IInitializable, IDisposable
    {
        private readonly ViewController _resultsViewController;
        private readonly FlowCoordinator _targetFlowCoordinator;

        [Inject]
        private readonly SubmissionDataContainer _submissionDataContainer = null!;

        [Inject]
        private readonly SiraSubmissionViewController _siraSubmissionViewController = null!;

        public SubmissionDisplayer(FlowCoordinator targetFlowCoordinator, ViewController resultsViewController)
        {
            _resultsViewController = resultsViewController;
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
                _targetFlowCoordinator.SetBottomScreenViewController(_siraSubmissionViewController, ViewController.AnimationType.In);
                _siraSubmissionViewController.Enabled(true);
                _siraSubmissionViewController.SetText($"<size=115%><color=#f03030>Score Submission Disabled By</color></size>\n{_submissionDataContainer.Read()}");
            }
        }

        private void ResultsViewController_didDeactivateEvent(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            _submissionDataContainer.Disabled = false;
            _siraSubmissionViewController.Enabled(false);
            if (_siraSubmissionViewController.isInViewControllerHierarchy)
                _targetFlowCoordinator.SetBottomScreenViewController(null, ViewController.AnimationType.Out);
        }

        public void Dispose()
        {
            _resultsViewController.didDeactivateEvent -= ResultsViewController_didDeactivateEvent;
            _resultsViewController.didActivateEvent -= ResultsViewController_didActivateEvent;
        }
    }
}