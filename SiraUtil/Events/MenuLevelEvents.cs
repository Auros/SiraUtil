using System;
using Zenject;

namespace SiraUtil.Events
{
    /// <summary>
    /// A service to unify menu level events.
    /// </summary>
    public sealed class MenuLevelEvents : IInitializable, IDisposable
    {
        private readonly StandardLevelDetailViewController _levelDetailViewController;
        private readonly MissionSelectionMapViewController _missionSelectionViewController;

        /// <summary>
        /// Called only when the Level is changed
        /// </summary>
        public event Action<IPreviewBeatmapLevel>? PreviewBeatmapLevelUpdated;

        /// <summary>
        /// Called when the Difficulty or Level is changed
        /// </summary>
        public event Action<IDifficultyBeatmap>? DifficultyBeatmapUpdated;

        internal MenuLevelEvents(StandardLevelDetailViewController levelDetailViewController, MissionSelectionMapViewController missionSelectionViewController)
        {
            _levelDetailViewController = levelDetailViewController;
            _missionSelectionViewController = missionSelectionViewController;
        }

        public void Initialize()
        {
            _levelDetailViewController.didChangeContentEvent += LevelDetailContentChanged;
            _levelDetailViewController.didChangeDifficultyBeatmapEvent += LevelDetailDifficultyChanged;
            _missionSelectionViewController.didSelectMissionLevelEvent += MissionSelected;
        }

        public void Dispose()
        {
            _levelDetailViewController.didChangeContentEvent -= LevelDetailContentChanged;
            _levelDetailViewController.didChangeDifficultyBeatmapEvent -= LevelDetailDifficultyChanged;
            _missionSelectionViewController.didSelectMissionLevelEvent -= MissionSelected;
        }

        private void LevelDetailContentChanged(StandardLevelDetailViewController levelDetailViewController, StandardLevelDetailViewController.ContentType _)
        {
            IDifficultyBeatmap difficultyBeatmap = levelDetailViewController.selectedDifficultyBeatmap;
            PreviewBeatmapLevelUpdated?.Invoke(difficultyBeatmap.level);
            DifficultyBeatmapUpdated?.Invoke(difficultyBeatmap);
        }

        private void LevelDetailDifficultyChanged(StandardLevelDetailViewController _, IDifficultyBeatmap difficultyBeatmap)
        {
            DifficultyBeatmapUpdated?.Invoke(difficultyBeatmap);
        }

        private void MissionSelected(MissionSelectionMapViewController missionSelectionMapViewController, MissionNode missionNode)
        {
            BeatmapLevelSO level = missionNode.missionData.level;
            IDifficultyBeatmap difficultyBeatmap = level.beatmapLevelData.GetDifficultyBeatmap(missionNode.missionData.beatmapCharacteristic, missionNode.missionData.beatmapDifficulty);
            PreviewBeatmapLevelUpdated?.Invoke(difficultyBeatmap.level);
            DifficultyBeatmapUpdated?.Invoke(difficultyBeatmap);
        }
    }
}
