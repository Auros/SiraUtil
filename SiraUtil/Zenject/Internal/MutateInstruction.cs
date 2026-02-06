using System;
using UnityEngine;
using Zenject;

namespace SiraUtil.Zenject.Internal
{
    internal struct MutateInstruction<TMonoBehaviour> : IInjectableMonoBehaviourInstruction where TMonoBehaviour : MonoBehaviour
    {
        internal MutateInstruction(Action<Context, TMonoBehaviour> action)
        {
            this.action = action;
        }

        internal Action<Context, TMonoBehaviour>? action { get; }

        public void Apply(Context context, MonoBehaviour monoBehaviour)
        {
            if (monoBehaviour is not TMonoBehaviour tMonoBehaviour)
            {
                return;
            }

            action?.Invoke(context, tMonoBehaviour);
        }
    }

    internal struct MutateInstruction<TMonoBehaviour, TNewComponent> : IInjectableMonoBehaviourInstruction where TMonoBehaviour : MonoBehaviour where TNewComponent : Component
    {
        internal MutateInstruction(Action<Context, TMonoBehaviour, TNewComponent>? action = null)
        {
            this.action = action;
        }

        internal Action<Context, TMonoBehaviour, TNewComponent>? action { get; }

        public void Apply(Context context, MonoBehaviour monoBehaviour)
        {
            if (monoBehaviour is not TMonoBehaviour tMonoBehaviour)
            {
                return;
            }

            TNewComponent newComponent = monoBehaviour.gameObject.AddComponent<TNewComponent>();
            context.Container.QueueForInject(newComponent);
            action?.Invoke(context, tMonoBehaviour, newComponent);
        }
    }

    internal struct SceneDecoratorMutateInstruction<TMonoBehaviour> : IInjectableMonoBehaviourInstruction
    {
        private string _contractName;

        internal SceneDecoratorMutateInstruction(string contractName, Action<SceneDecoratorContext, TMonoBehaviour> action)
        {
            _contractName = contractName;
            this.action = action;
        }

        internal Action<SceneDecoratorContext, TMonoBehaviour>? action { get; }

        public void Apply(Context context, MonoBehaviour monoBehaviour)
        {
            if (context is not SceneDecoratorContext sceneDecoratorContext || sceneDecoratorContext.DecoratedContractName != _contractName || monoBehaviour is not TMonoBehaviour tMonoBehaviour)
            {
                return;
            }

            action?.Invoke(sceneDecoratorContext, tMonoBehaviour);
        }
    }
}
