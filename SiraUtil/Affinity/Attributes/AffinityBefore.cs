using System;

namespace SiraUtil.Affinity
{
    /// <summary>
    /// Have an affinity patch run before other patches.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AffinityBefore : Attribute
    {
        internal string[] Before { get; }

        /// <summary>
        /// Construct an Affinity Before
        /// </summary>
        /// <param name="before">The IDs of the patches to run before.</param>
        public AffinityBefore(params string[] before)
        {
            Before = before;
        }
    }
}