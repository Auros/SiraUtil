namespace SiraUtil.Interfaces
{
    /// <summary>
    /// An interface for a manager to handle a dynamic number of sabers.
    /// </summary>
    public interface ISaberRegistrar
    {
        /// <summary>
        /// Initialize the sabers.
        /// </summary>
        /// <param name="saberManager">The saber manager used to get the original saber references.</param>
        void Initialize(SaberManager saberManager);

        /// <summary>
        /// Change the color of a registered saber.
        /// </summary>
        /// <param name="saber">The saber to change the color of.</param>
        void ChangeColor(Saber saber);

        /// <summary>
        /// Registers a saber into the registrar.
        /// </summary>
        /// <param name="saber">The saber to register.</param>
        void RegisterSaber(Saber saber);

        /// <summary>
        /// Unregisters a saber in the registrar.
        /// </summary>
        /// <param name="saber">The saber to unregister.</param>
        void UnregisterSaber(Saber saber);
    }
}