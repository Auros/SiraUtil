using System;
using System.Collections.Generic;

namespace SiraUtil.Sabers
{
    /// <summary>
    /// A container to provide a saber model.
    /// </summary>
    public class SaberModelProvider
    {
        internal static List<SaberModelProvider> providers = new List<SaberModelProvider>();
        public Type Type { get; private set; }
        public int Priority { get; private set; }
        public ISaberModelController ModelController { get; private set; }

        /// <summary>
        /// Register a provider to be installed.
        /// </summary>
        /// <param name="provider"></param>
        public static void AddProvider(SaberModelProvider provider)
        {
            if (!providers.Contains(provider))
            {
                providers.Add(provider);
            }
        }

        /// <summary>
        /// Unregisters a provider.
        /// </summary>
        /// <param name="provider"></param>
        public static void RemoveProvider(SaberModelProvider provider)
        {
            providers.Remove(provider);
        }

        /// <summary>
        /// Create a saber model provider.
        /// </summary>
        /// <param name="priority">The priority of the provider. The highest provider is the model controller that is used.</param>
        /// <param name="controller">The model controller that is used to setup the saber.</param>
        public SaberModelProvider(int priority, ISaberModelController controller)
        {
            Priority = priority;
            ModelController = controller;
        }
    }
}