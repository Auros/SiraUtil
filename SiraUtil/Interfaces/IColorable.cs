using UnityEngine;

namespace SiraUtil.Interfaces
{
    /// <summary>
    /// An interface which marks something as colorable and allows its color to be changed easily. Mainly used to increase intermod compatibility.
    /// </summary>
    public interface IColorable
    {
        /// <summary>
        /// The current color value of an object.
        /// </summary>
        Color Color { get; }

        /// <summary>
        /// Sets the color of an object.
        /// </summary>
        /// <param name="color">The color to set to.</param>
        void SetColor(Color color);
    }
}