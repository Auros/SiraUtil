using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using System;
using UnityEngine;

namespace SiraUtil.Tools.SongControl
{
    internal class SongControlOptions
    {
        public Action<SongControlOptions>? Updated;

        [UseConverter(typeof(EnumConverter<KeyCode>))]
        public virtual KeyCode RestartKeyCode { get; set; } = KeyCode.F4;


        [UseConverter(typeof(EnumConverter<KeyCode>))]
        public virtual KeyCode ExitKeyCode { get; set; } = KeyCode.Escape;


        [UseConverter(typeof(EnumConverter<KeyCode>))]
        public virtual KeyCode PauseToggleKeyCode { get; set; } = KeyCode.F2;

        public void Changed()
        {
            Updated?.Invoke(this);
        }
    }
}