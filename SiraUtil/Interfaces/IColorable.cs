using UnityEngine;

namespace SiraUtil.Interfaces
{
    /// <summary>
    /// An interface which marks something as colorable and allows its color to be changed easily. Mainly used to increase intermod compatibility.
    /// </summary>
    public interface IColorable
    {
        Color Color { get; }
        void SetColor(Color color);
    }
}