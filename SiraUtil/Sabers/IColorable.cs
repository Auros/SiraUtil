using UnityEngine;

namespace SiraUtil.Sabers
{
    public interface IColorable
    {
        Color Color { get; }
        void SetColor(Color color);
    }
}