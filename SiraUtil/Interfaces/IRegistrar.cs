namespace SiraUtil.Interfaces
{
    /// <summary>
    /// A generic interface which defines a registration.
    /// </summary>
    /// <typeparam name="T">The type to register.</typeparam>
    public interface IRegistrar<T>
    {
        /// <summary>
        /// Adds a registration to this registrar.
        /// </summary>
        /// <param name="value"></param>
        void Add(T value);

        /// <summary>
        /// Removes a registration from this registrar.
        /// </summary>
        /// <param name="value"></param>
        void Remove(T value);
    }
}