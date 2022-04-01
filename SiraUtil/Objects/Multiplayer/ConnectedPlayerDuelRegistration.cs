using System;

namespace SiraUtil.Objects.Multiplayer
{
    /// <summary>
    /// Registers a redecorator for the connected player in the duel.
    /// </summary>
    public sealed class ConnectedPlayerDuelRegistration : TemplateRedecoratorRegistration<MultiplayerConnectedPlayerFacade, MultiplayerPlayersManager>
    {
        /// <summary>
        /// Creates a new redecorator registration.
        /// </summary>
        /// <param name="redecorateCall">This is called when the object is being redecorated.</param>
        /// <param name="priority">The redecoration priority.</param>
        /// <param name="chain">Whether to chain this redecoration with others. Every redecoration is now aggregated.
        /// The chain will start if the highest priority object has chaining enabled and will stop once a registration
        /// in the aggregate has chaining disabled.</param>
        public ConnectedPlayerDuelRegistration(Func<MultiplayerConnectedPlayerFacade, MultiplayerConnectedPlayerFacade> redecorateCall, int priority = 0, bool chain = true) : base("_connectedPlayerDuelControllerPrefab", redecorateCall, priority, chain) { }
    }
}
