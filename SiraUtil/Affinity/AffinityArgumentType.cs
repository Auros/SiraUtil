namespace SiraUtil.Affinity
{
    /// <summary>
    /// A mirror to <see cref="HarmonyLib.ArgumentType" />.
    /// </summary>
    public enum AffinityArgumentType
    {
        /// <summary>
        /// A normal argument type.
        /// </summary>
        Normal,

        /// <summary>
        /// A reference argument type.
        /// </summary>
        Ref,

        /// <summary>
        /// An out argument type.
        /// </summary>
        Out,

        /// <summary>
        /// A pointer argument type.
        /// </summary>
        Pointer
    }
}