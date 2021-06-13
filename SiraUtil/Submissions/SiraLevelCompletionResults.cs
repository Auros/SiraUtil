namespace SiraUtil.Submissions
{
    internal class SiraLevelCompletionResults : LevelCompletionResults
    {
        public bool ShouldSubmitScores { get; }

        public SiraLevelCompletionResults(LevelCompletionResults levelCompletionResults, bool scoreSubmission) : base(levelCompletionResults.gameplayModifiers, levelCompletionResults.modifiedScore, levelCompletionResults.rawScore,
            levelCompletionResults.rank, levelCompletionResults.fullCombo, levelCompletionResults.leftSaberMovementDistance, levelCompletionResults.rightSaberMovementDistance, levelCompletionResults.leftHandMovementDistance,
			levelCompletionResults.rightHandMovementDistance, levelCompletionResults.songDuration, levelCompletionResults.levelEndStateType, levelCompletionResults.levelEndAction, levelCompletionResults.energy, levelCompletionResults.goodCutsCount,
			levelCompletionResults.badCutsCount, levelCompletionResults.missedCount, levelCompletionResults.notGoodCount, levelCompletionResults.okCount, levelCompletionResults.averageCutScore, levelCompletionResults.maxCutScore,
			levelCompletionResults.averageCutDistanceRawScore, levelCompletionResults.maxCombo, levelCompletionResults.minDirDeviation, levelCompletionResults.maxDirDeviation, levelCompletionResults.averageDirDeviation,
			levelCompletionResults.minTimeDeviation, levelCompletionResults.maxTimeDeviation, levelCompletionResults.averageTimeDeviation, levelCompletionResults.endSongTime)
        {
            ShouldSubmitScores = scoreSubmission;
		}
    }
}