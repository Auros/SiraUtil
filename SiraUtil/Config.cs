using UnityEngine;
using IPA.Config.Stores.Attributes;

namespace SiraUtil
{
    public class Config
    {
        [NonNullable]
        public virtual SongControlOptions SongControl { get; set; } = new SongControlOptions();

        public class SongControlOptions
        {
            public virtual bool Enabled { get; set; } = false;
            public virtual KeyCode ExitKeyCode { get; set; } = KeyCode.Escape;
            public virtual KeyCode RestartKeyCode { get; set; } = KeyCode.F4;
            public virtual KeyCode PauseToggleKeyCode { get; set; } = KeyCode.F2;
        }
    }
}
