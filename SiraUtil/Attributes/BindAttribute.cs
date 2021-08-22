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
        public bool NonLazy { get; }
        public Location Location { get; }
        public Type[]? Contracts { get; }
        public BindType BindType { get; } = BindType.Single;

        public BindAttribute()
        {
            Location = Location.App;
        }

        public BindAttribute(Location location)
        {
            Location = location;
        }

        public BindAttribute(params Type[] contracts)
        {
            Contracts = contracts;
        }

        public BindAttribute(Location location, params Type[] contracts)
        {
            Location = location;
            Contracts = contracts;
        }

        public BindAttribute(Location location = Location.App, BindType bindType = BindType.Single, bool nonLazy = false, params Type[]? contracts)
        {
            NonLazy = nonLazy;
            Location = location;
            BindType = bindType;
            Contracts = contracts;
        }
    }
}