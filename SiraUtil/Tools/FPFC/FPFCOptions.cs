using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace SiraUtil.Tools.FPFC
{
    internal class FPFCOptions
    {
        public Action<FPFCOptions>? Updated; 

        public virtual float CameraFOV { get; set; } = 100f;
        public virtual float MouseSensitivity { get; set; } = 3.5f;

        [UseConverter(typeof(EnumConverter<KeyCode>))]
        public virtual KeyCode ToggleKeyCode { get; set; } = KeyCode.G;

        public void Changed()
        {
            Updated?.Invoke(this);
        }
    }
}