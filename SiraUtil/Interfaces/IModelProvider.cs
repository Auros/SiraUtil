using System;

namespace SiraUtil.Interfaces
{
    public interface IModelProvider
    {
		Type Type { get; }
        int Priority { get; }
    }
}