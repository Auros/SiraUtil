using System;

namespace SiraUtil.Objects.Debris
{
    /// <summary>
    /// Registers a redecorator for the high-quality burst slider head (i.e. head in the chain) debris.
    /// </summary>
    public sealed class BurstSliderHeadNoteDebrisHDRegistration : TemplateRedecoratorRegistration<NoteDebris, NoteDebrisPoolInstaller>
    {
        /// <summary>
        /// Creates a new redecorator registration.
        /// </summary>
        /// <param name="redecorateCall">This is called when the object is being redecorated.</param>
        /// <param name="priority">The redecoration priority.</param>
        /// <param name="chain">Whether to chain this redecoration with others. Every redecoration is now aggregated.
        /// The chain will start if the highest priority object has chaining enabled and will stop once a registration
        /// in the aggregate has chaining disabled.</param>
        public BurstSliderHeadNoteDebrisHDRegistration(Func<NoteDebris, NoteDebris> redecorateCall, int priority = 0, bool chain = true) : base("_burstSliderHeadNoteDebrisHDPrefab", redecorateCall, priority, chain) { }
    }
}
