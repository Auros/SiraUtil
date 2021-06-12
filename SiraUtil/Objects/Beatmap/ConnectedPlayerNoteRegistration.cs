using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiraUtil.Objects.Beatmap
{
    /// <summary>
    /// Registers a redecorator for the connected multiplayer note.
    /// </summary>
    public class ConnectedPlayerNoteRegistration : TemplateRedecoratorRegistration<MultiplayerConnectedPlayerGameNoteController, MultiplayerConnectedPlayerInstaller>
    {
        /// <summary>
        /// Creates a new redecorator registration.
        /// </summary>
        /// <param name="redecorateCall">This is called when the object is being redecorated.</param>
        /// <param name="priority">The redecoration priority.</param>
        /// <param name="chain">Whether to chain this redecoration with others. Every redecoration is now aggregated.
        /// The chain will start if the highest priority object has chaining enabled and will stop once a registration
        /// in the aggregate has chaining disabled.</param>
        public ConnectedPlayerNoteRegistration(Func<MultiplayerConnectedPlayerGameNoteController, MultiplayerConnectedPlayerGameNoteController> redecorateCall, int priority = 0, bool chain = true) : base("_multiplayerGameNoteControllerPrefab", redecorateCall, priority, chain) { }
    }
}
