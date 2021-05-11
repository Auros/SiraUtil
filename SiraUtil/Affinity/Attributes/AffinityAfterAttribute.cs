using System;

namespace SiraUtil.Affinity
{
    /// <summary>
    /// Have an affinity patch run after other patches.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AffinityAfterAttribute : Attribute
    {
        internal string[] After { get; }

        /// <summary>
        /// Construct an Affinity After
        /// </summary>
        /// <param name="after">The IDs of the patches to run after.</param>
        public AffinityAfterAttribute(params string[] after)
        {
            After = after;
        }
    }
}