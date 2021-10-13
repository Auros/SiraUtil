namespace SiraUtil.Zenject
{
    /// <summary>
    /// Zenject bind type.
    /// </summary>
    public enum BindType
    {
        /// <summary>
        /// AsSingle()
        /// </summary>
        Single,

        /// <summary>
        /// AsTransient()
        /// </summary>
        Transient,

        /// <summary>
        /// AsCached()
        /// </summary>
        Cached
    }
}