using System;

namespace SiraUtil.Affinity
{
    /// <summary>
    /// Have an affinity patch run before other patches.
    /// </summary>
    [JetBrains.Annotations.MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Method)]
    public class AffinityBeforeAttribute : Attribute
    {
        internal string[] Before { get; }

        /// <summary>
        /// Construct an Affinity Before
        /// </summary>
        /// <param name="before">The IDs of the patches to run before.</param>
        public AffinityBeforeAttribute(params string[] before)
        {
            Before = before;
        }
    }
}