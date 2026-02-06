using UnityEngine;
using Zenject;

namespace SiraUtil.Zenject.Internal
{
    internal interface IInjectableMonoBehaviourInstruction
    {
        void Apply(Context context, MonoBehaviour monoBehaviour);
    }
}
