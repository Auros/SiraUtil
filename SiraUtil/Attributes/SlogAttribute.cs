using System;
using System.Diagnostics;

namespace SiraUtil.Attributes
{
    /// <summary>
    /// Allows SiraUtil to detect if plugins are built for release.
    /// </summary>
    [Conditional("DEBUG")]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class SlogAttribute : Attribute
    {
        /// <summary>
        /// An empty constructor.
        /// </summary>
        public SlogAttribute() { }
    }
}