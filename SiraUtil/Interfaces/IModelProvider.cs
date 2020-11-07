using System;

namespace SiraUtil.Interfaces
{
    /// <summary>
    /// An interface for mods to provide custom models to other objects.
    /// </summary>
    public interface IModelProvider
    {
        /// <summary>
        /// The type of the container that will be instantiated to replace the model.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// The priority of the provider. The higher this is the more important it is. Set this to -1 to have this provider not recognized at all.
        /// </summary>
        int Priority { get; }
    }
}