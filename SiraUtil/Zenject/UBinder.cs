namespace SiraUtil.Zenject
{
    /// <summary>
    /// Type binder to make intermod container injection simpler.
    /// </summary>
    /// <typeparam name="T">The type of the host parent (key).</typeparam>
    /// <typeparam name="U">The type of the shared service (value).</typeparam>
    public class UBinder<T, U>
    {
        /// <summary>
        /// The binder value.
        /// </summary>
        public U Value;

        /// <summary>
        /// Create a type binder.
        /// </summary>
        /// <param name="value">The value for the binder.</param>
        public UBinder(U value)
        {
            Value = value;
        }
    }
}