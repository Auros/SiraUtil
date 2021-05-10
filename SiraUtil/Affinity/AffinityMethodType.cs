namespace SiraUtil.Affinity
{
    /// <summary>
    /// A mirror to <see cref="HarmonyLib.MethodType" />.
    /// </summary>
    public enum AffinityMethodType
    {
        /// <summary>
        /// A normal method.
        /// </summary>
        Normal,

        /// <summary>
        /// The getter of a property.
        /// </summary>
        Getter,

        /// <summary>
        /// The setter of a property.
        /// </summary>
        Setter,

        /// <summary>
        /// The construtor of a class.
        /// </summary>
        Constructor,

        /// <summary>
        /// The static constructor of a class.
        /// </summary>
        StaticConstructor
    }
}