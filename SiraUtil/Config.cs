using System;
using Polyglot;
using UnityEngine;
using IPA.Config.Stores;
using SiraUtil.Converters;
using Version = SemVer.Version;
using System.Collections.Generic;
using IPA.Config.Stores.Converters;
using IPA.Config.Stores.Attributes;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace SiraUtil
{
    internal class Config
    {
        [NonNullable, UseConverter(typeof(VersionConverter))]
        public virtual Version Version { get; set; }

        [NonNullable]
        public virtual FPFCToggleOptions FPFCToggle { get; set; } = new FPFCToggleOptions();

        [NonNullable]
        public virtual SongControlOptions SongControl { get; set; } = new SongControlOptions();

        [Obsolete("SiraUtil no longer handles the loading of localization sheets. You can now optionally request an ILocalizer with an ID of SIRA.Localizer in a container to get the localizer component from SiraLocalizer (separate mod).")]
        public virtual LocalizationOptions Localization { get; set; } = null;

        public class SongControlOptions
        {
            public virtual bool Enabled { get; set; } = false;

            [UseConverter(typeof(EnumConverter<KeyCode>))]
            public virtual KeyCode RestartKeyCode { get; set; } = KeyCode.F4;

            [UseConverter(typeof(EnumConverter<KeyCode>))]
            public virtual KeyCode ExitKeyCode { get; set; } = KeyCode.Escape;

            [UseConverter(typeof(EnumConverter<KeyCode>))]
            public virtual KeyCode PauseToggleKeyCode { get; set; } = KeyCode.F2;
        }

        public class FPFCToggleOptions
        {
            public virtual bool Enabled { get; set; } = false;
            public virtual float CameraFOV { get; set; } = 90f;
            public virtual float MoveSensitivity { get; set; } = 5f;
            public virtual bool OnFirstRequest { get; set; } = false;

            [UseConverter(typeof(EnumConverter<KeyCode>))]
            public virtual KeyCode ToggleKeyCode { get; set; } = KeyCode.G;
        }

        public class LocalizationOptions
        {
            public virtual bool Enabled { get; set; } = false;

            [NonNullable, UseConverter(typeof(DictionaryConverter<LocalizationSource>))]
            public virtual Dictionary<string, LocalizationSource> Sources { get; set; } = new Dictionary<string, LocalizationSource>();

            public class LocalizationSource
            {
                public bool Enabled { get; set; }
                public string URL { get; set; }
                public string IsOnline { get; set; }

                [UseConverter(typeof(EnumConverter<GoogleDriveDownloadFormat>))]
                public GoogleDriveDownloadFormat Format { get; set; }
            }
        }
    }
}