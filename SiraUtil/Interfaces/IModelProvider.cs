using System;

namespace SiraUtil.Interfaces
{
    /// <summary>
    /// An interface for mods to provide custom models to other objects.
    /// </summary>
    public interface IModelProvider
    {
        Type Type { get; }
        int Priority { get; }
    }
}