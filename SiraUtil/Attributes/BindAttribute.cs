using HMUI;
using SiraUtil.Affinity;
using SiraUtil.Zenject;
using System;
using Zenject;

namespace SiraUtil.Attributes
{
    /// <summary>
    /// Allows the ability to install Zenject bindings through an attribute. This can help with rapid development and make Zenject setup easier. The default settings
    /// for this attribute are <see cref="Location.App"/>, no special contracts, and binds as as Single (Lazy).
    /// Make sure to call zenjector.UseAutoBinder() to set it up. If the class this is placed on inherits <see cref="IInitializable"/>, <see cref="ITickable"/>,
    /// <see cref="IFixedTickable"/>, <see cref="ILateTickable"/>, <see cref="IDisposable"/>, <see cref="ILateDisposable"/>, <see cref="IAsyncInitializable"/>, 
    /// <see cref="ViewController"/>, <see cref="FlowCoordinator"/>, or <see cref="IAffinity"/> it will bind those appropriately unless you provide a custom type contract array.
    /// </summary>
    /// <remarks>
    /// This simplifies binding, but that doesn't mean <see cref="Installer"/>s are suddenly useless.
    /// Installers are a great way of organization and conditional binding (when you only want to install something if a setting is enabled, etc).
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class BindAttribute : Attribute
    {
        /// <summary>
        /// Whether or not the binding is NonLazy
        /// </summary>
        public bool NonLazy { get; }

        /// <summary>
        /// The location to bind to.
        /// </summary>
        public Location Location { get; }

        /// <summary>
        /// The type contracts for this binding.
        /// </summary>
        public Type[]? Contracts { get; }

        /// <summary>
        /// The Zenject bind type.
        /// </summary>
        public BindType BindType { get; } = BindType.Single;

        /// <summary>
        /// Creates a attributed-binding.
        /// </summary>
        public BindAttribute()
        {
            Location = Location.App;
        }

        /// <summary>
        /// Creates a attributed-binding.
        /// </summary>
        /// <param name="location">The location to install to.</param>
        public BindAttribute(Location location)
        {
            Location = location;
        }

        /// <summary>
        /// Creates a attributed-binding.
        /// </summary>
        /// <param name="contracts">The type contracts for this binding.</param>
        public BindAttribute(params Type[] contracts)
        {
            Contracts = contracts;
        }

        /// <summary>
        /// Creates a attributed-binding.
        /// </summary>
        /// <param name="location">The location to install to.</param>
        /// <param name="contracts">The type contracts for this binding.</param>
        public BindAttribute(Location location, params Type[] contracts)
        {
            Location = location;
            Contracts = contracts;
        }

        /// <summary>
        /// Creates a attributed-binding.
        /// </summary>
        /// <param name="location">The location to install to.</param>
        /// <param name="bindType">The Zenject bind type.</param>
        /// <param name="nonLazy">Whether or not this binding is NonLazy</param>
        /// <param name="contracts">The type contracts for this binding.</param>
        public BindAttribute(Location location = Location.App, BindType bindType = BindType.Single, bool nonLazy = false, params Type[]? contracts)
        {
            NonLazy = nonLazy;
            Location = location;
            BindType = bindType;
            Contracts = contracts;
        }
    }
}