using IPA.Loader;

namespace SiraUtil.Zenject
{
    /// <summary>
    /// A constructor class for building Zenject installer registration events.
    /// </summary>
    public class Zenjector
    {
        internal PluginMetadata Metadata { get; }

        internal Zenjector(PluginMetadata metadata)
        {
            Metadata = metadata;
        }
    }
}