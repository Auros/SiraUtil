using System;

namespace SiraUtil.Interfaces
{
    /// <summary>
    /// Interface for having a callback with a parent who manages type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type (of self) that the parent manages.</typeparam>
    public interface ICallback<T>
    {
        /// <summary>
        /// The callback for the parent to subscribe to.
        /// </summary>
        event Action<T> Callback;
    }
}
