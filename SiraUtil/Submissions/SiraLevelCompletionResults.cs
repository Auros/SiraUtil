namespace SiraUtil.Submissions
{
    internal class SiraLevelCompletionResults : LevelCompletionResults
    {
        public bool ShouldSubmitScores { get; }

        public SiraLevelCompletionResults(LevelCompletionResults lcr, bool scoreSubmission) : base(lcr.gameplayModifiers, lcr.score,
            lcr.rank, lcr.fullCombo, lcr.leftSaberMovementDistance, lcr.rightSaberMovementDistance, lcr.leftHandMovementDistance,
            lcr.rightHandMovementDistance, lcr.levelEndStateType, lcr.levelEndAction, lcr.energy, lcr.goodCutsCount,
            lcr.badCutsCount, lcr.missedCutsCount, lcr.notGoodCount, lcr.okCount, lcr.maxCutScore, lcr.totalCutScore, lcr.goodCutsCountForNotesWithFullScoreScoringType, lcr.averageCenterDistanceCutScoreForNotesWithFullScoreScoringType, lcr.averageCutScoreForNotesWithFullScoreScoringType,
            lcr.maxCombo, lcr.endSongTime, lcr.invalidated, lcr.startSongTime)
        {
            ShouldSubmitScores = scoreSubmission;
        }
    }
}