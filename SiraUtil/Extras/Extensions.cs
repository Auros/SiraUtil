using SiraUtil.Objects;
using Zenject;

namespace SiraUtil.Extras
{
    /// <summary>
    /// Some public extensions.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Registers a redecorator for the object API.
        /// </summary>
        /// <remarks>
        /// This cannot be called on the App scene, please only call this as you're installing your game related bindings.
        /// </remarks>
        /// <typeparam name="TRegistrator"></typeparam>
        /// <param name="container"></param>
        /// <param name="registrator"></param>
        public static void RegisterRedecorator<TRegistrator>(this DiContainer container, TRegistrator registrator) where TRegistrator : RedecoratorRegistration
        {
            container.AncestorContainers[0].Bind<RedecoratorRegistration>().FromInstance(registrator).AsCached();
        }
    }
}