using System;

namespace SiraUtil.Affinity
{
    /// <summary>
    /// Tells Harmony to emit IL of the patch to a DLL.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AffinityEmitILAttribute : Attribute
    {
        internal string Path { get; }

        /// <summary>
        /// Tells Harmony to emit IL of the patch to the current working directory.
        /// </summary>
        public AffinityEmitILAttribute()
        {
            Path = "./";
        }

        /// <summary>
        /// Tells Harmony to emit IL of the patch to the given path.
        /// </summary>
        /// <param name="path">Directory to which emit the patch.</param>
        public AffinityEmitILAttribute(string path)
        {
            Path = path;
        }
    }
}