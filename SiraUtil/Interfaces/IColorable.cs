using UnityEngine;

namespace SiraUtil.Interfaces
{
    /// <summary>
    /// An interface which describes that an object can be described by a color and can be colored.
    /// </summary>
    public interface IColorable
    {
        /// <summary>
        /// The color of this <see cref="IColorable" />
        /// </summary>
        Color Color { get; set; }
    }
}