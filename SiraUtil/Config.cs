using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using SiraUtil.Converters;
using SiraUtil.Tools.FPFC;
using System;
using System.Runtime.CompilerServices;
using Version = Hive.Versioning.Version;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace SiraUtil
{
    internal class Config
    {
        public Action<Config>? Updated;

        [NonNullable, UseConverter(typeof(VersionConverter))]
        public virtual Version Version { get; set; } = null!;

        [NonNullable]
        public virtual FPFCOptions FPFCToggle { get; set; } = new FPFCOptions();

        public virtual void Changed()
        {
            Updated?.Invoke(this);
            FPFCToggle.Changed();
        }
    }
}