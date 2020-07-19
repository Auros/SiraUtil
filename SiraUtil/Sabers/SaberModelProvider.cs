using System;
using System.Collections.Generic;

namespace SiraUtil.Sabers
{
    public class SaberModelProvider
    {
        internal static List<SaberModelProvider> providers = new List<SaberModelProvider>();

        public static void AddProvider(SaberModelProvider provider)
        {
            if (!providers.Contains(provider))
            {
                providers.Add(provider);
            }
        }

        public static void RemoveProvider(SaberModelProvider provider)
        {
            providers.Remove(provider);
        }

        public SaberModelProvider(int priority, MonoBehaviourSaberModelController controller)
        {
            Priority = priority;
            ModelController = controller;
        }


        public Type Type { get; private set; }
        public int Priority { get; private set; }
        public MonoBehaviourSaberModelController ModelController { get; private set; }
    }
}