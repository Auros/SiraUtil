using System;

namespace SiraUtil.Objects.Beatmap
{
    /// <summary>
    /// Registers a redecorator for a long slider note (arc)
    /// </summary>
    public sealed class LongSliderNoteRegistration : TemplateRedecoratorRegistration<SliderController, BeatmapObjectsInstaller>
    {
        /// <summary>
        /// Creates a new redecorator registration.
        /// </summary>
        /// <param name="redecorateCall">This is called when the object is being redecorated.</param>
        /// <param name="priority">The redecoration priority.</param>
        /// <param name="chain">Whether to chain this redecoration with others. Every redecoration is now aggregated.
        /// The chain will start if the highest priority object has chaining enabled and will stop once a registration
        /// in the aggregate has chaining disabled.</param>
        public LongSliderNoteRegistration(Func<SliderController, SliderController> redecorateCall, int priority = 0, bool chain = true) : base("_sliderLongPrefab", redecorateCall, priority, chain) {}
    }
}