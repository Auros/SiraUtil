namespace SiraUtil.Submissions
{
    internal class SiraLevelCompletionResults : LevelCompletionResults
    {
        public bool ShouldSubmitScores { get; }

        public SiraLevelCompletionResults(LevelCompletionResults lcr, bool scoreSubmission) : base(lcr.gameplayModifiers, lcr.modifiedScore, lcr.multipliedScore,
            lcr.rank, lcr.fullCombo, lcr.leftSaberMovementDistance, lcr.rightSaberMovementDistance, lcr.leftHandMovementDistance,
            lcr.rightHandMovementDistance, lcr.levelEndStateType, lcr.levelEndAction, lcr.energy, lcr.goodCutsCount,
            lcr.badCutsCount, lcr.missedCount, lcr.notGoodCount, lcr.okCount, lcr.maxCutScore, lcr.totalCutScore, lcr.goodCutsCountForNotesWithFullScoreScoringType, lcr.averageCenterDistanceCutScoreForNotesWithFullScoreScoringType, lcr.averageCutScoreForNotesWithFullScoreScoringType,
            lcr.maxCombo, lcr.endSongTime)
        {
            ShouldSubmitScores = scoreSubmission;
        }
    }
}