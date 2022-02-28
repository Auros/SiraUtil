using System;

namespace SiraUtil.Objects
{
    /// <summary>
    /// Registers an object to be redecorated.
    /// </summary>
    public abstract class RedecoratorRegistration
    {
        internal bool Chain { get; }
        internal int Priority { get; }
        internal string Contract { get; }
        internal Type PrefabType { get; }
        internal Type ContainerType { get; }

        /// <summary>
        /// Creates a new registration.
        /// </summary>
        /// <param name="contract">The prefab contract.</param>
        /// <param name="prefabType">The type of the prefab.</param>
        /// <param name="containerType">The parent of the prefab. This is usually an installer.</param>
        /// <param name="priority">The redecoration priority.</param>
        /// <param name="chain">Whether to chain this redecoration with others. Every redecoration is now aggregated.
        /// The chain will start if the highest priority object has chaining enabled and will stop once a registration
        /// in the aggregate has chaining disabled.</param>
        public RedecoratorRegistration(string contract, Type prefabType, Type containerType, int priority = 0, bool chain = true)
        {
            Chain = chain;
            Priority = priority;
            Contract = contract;
            PrefabType = prefabType;
            ContainerType = containerType;

            Plugin.Log.Info($"Installing redecorator with contract {contract} on {containerType}");
        }

        /// <summary>
        /// Redecorates an object.
        /// </summary>
        /// <param name="value">The object to redecorate.</param>
        /// <returns>The redecorated object.</returns>
        protected internal abstract object Redecorate(object value);
    }
}