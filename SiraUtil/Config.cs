using UnityEngine;
using IPA.Config.Stores.Attributes;

namespace SiraUtil
{
    public class Config
    {
        public virtual int MajorVersion { get; set; }
        public virtual int MinorVersion { get; set; }
        public virtual int BuildVersion { get; set; }

        [NonNullable]
        public virtual FPFCToggleOptions FPFCToggle { get; set; } = new FPFCToggleOptions();

        [NonNullable]
        public virtual SongControlOptions SongControl { get; set; } = new SongControlOptions();

        public class SongControlOptions
        {
            public virtual bool Enabled { get; set; } = false;
            public virtual KeyCode ExitKeyCode { get; set; } = KeyCode.Escape;
            public virtual KeyCode RestartKeyCode { get; set; } = KeyCode.F4;
            public virtual KeyCode PauseToggleKeyCode { get; set; } = KeyCode.F2;
        }

        public class FPFCToggleOptions
        {
            public virtual bool Enabled { get; set; } = false;
            public virtual KeyCode ToggleKeyCode { get; set; } = KeyCode.G;
        }
    }
}
