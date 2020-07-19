using System;

namespace SiraUtil.Events
{
    public static class SiraEvents
    {
        public static event Action<IDifficultyBeatmap, BeatmapCharacteristicSO> LevelSelectionChanged;

        internal static void InvokeLevelSelectionChange(IDifficultyBeatmap beatmap, BeatmapCharacteristicSO characteristic)
        {
            LevelSelectionChanged?.Invoke(beatmap, characteristic);
        }
    }
}