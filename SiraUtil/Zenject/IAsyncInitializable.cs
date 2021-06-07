using System.Threading.Tasks;

namespace SiraUtil.Zenject
{
    /// <summary>
    /// An interface for initializing an object asynchronously.
    /// </summary>
    public interface IAsyncInitializable
    {
        /// <summary>
        /// Initializees asynchronously.
        /// </summary>
        /// <returns>The task of the initialization.</returns>
        Task InitializeAsync();
    }
}