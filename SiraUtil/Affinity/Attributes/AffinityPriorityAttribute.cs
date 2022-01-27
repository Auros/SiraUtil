using System;

namespace SiraUtil.Affinity
{
    /// <summary>
    /// Assign an affinity patch a priority.
    /// </summary>
    [JetBrains.Annotations.MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Method)]
    public class AffinityPriorityAttribute : Attribute
    {
        internal int Priority { get; } = -1;

        /// <summary>
        /// Constructs an Affinity Priority
        /// </summary>
        /// <param name="priority">An arbitrary number representing the level of priority this patch should receive.</param>
        public AffinityPriorityAttribute(int priority)
        {
            Priority = priority;
        }
    }
}
