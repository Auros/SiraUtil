using UnityEngine;
using Zenject;

namespace SiraUtil.Zenject.Internal
{
    internal interface IInjectableMonoBehaviourInstruction
    {
        int Order { get; }

        void Apply(Context context, MonoBehaviour monoBehaviour);
    }
}
