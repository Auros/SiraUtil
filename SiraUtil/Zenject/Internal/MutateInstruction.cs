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
        private Action<Context, TMonoBehaviour, TNewComponent>? _action;
        private Func<TMonoBehaviour, GameObject>? _gameObjectGetter;

        internal MutateInstruction(Action<Context, TMonoBehaviour, TNewComponent>? action, Func<TMonoBehaviour, GameObject>? gameObjectGetter)
        {
            _action = action;
            _gameObjectGetter = gameObjectGetter;
        }

        public void Apply(Context context, MonoBehaviour monoBehaviour)
        {
            if (monoBehaviour is not TMonoBehaviour tMonoBehaviour)
            {
                return;
            }

            GameObject gameObject;

            if (_gameObjectGetter != null)
            {
                gameObject = _gameObjectGetter(tMonoBehaviour);
            }
            else
            {
                gameObject = monoBehaviour.gameObject;
            }

            TNewComponent newComponent = gameObject.AddComponent<TNewComponent>();
            context.Container.QueueForInject(newComponent);
            _action?.Invoke(context, tMonoBehaviour, newComponent);
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
