using Polyglot;
using UnityEngine;
using System.Collections.Generic;
using IPA.Config.Stores.Converters;
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

		[NonNullable]
		public virtual LocalizationOptions Localization { get; set; } = new LocalizationOptions();

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
			public virtual float MoveSensitivity { get; set; } = 0.1f;

			[UseConverter(typeof(EnumConverter<KeyCode>))]
			public virtual KeyCode ToggleKeyCode { get; set; } = KeyCode.G;
		}

		public class LocalizationOptions
		{
			public virtual bool Enabled { get; set; } = true;

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