using System;

namespace SiraUtil.Affinity
{
    /// <summary>
    /// Have an affinity patch run after other patches.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AffinityAfter : Attribute
    {
        internal string[] After { get; }

        /// <summary>
        /// Construct an Affinity After
        /// </summary>
        /// <param name="after">The IDs of the patches to run after.</param>
        public AffinityAfter(params string[] after)
        {
            After = after;
        }
    }
}