using System;
using UnityEngine;
using Zenject;

namespace SiraUtil.Zenject.Internal
{
    internal readonly struct MutateInstruction<TMonoBehaviour> : IInjectableMonoBehaviourInstruction where TMonoBehaviour : MonoBehaviour
    {
        private readonly Action<Context, TMonoBehaviour>? _action;

        internal MutateInstruction(Action<Context, TMonoBehaviour> action)
        {
            _action = action;
        }

        public readonly void Apply(Context context, MonoBehaviour monoBehaviour)
        {
            if (monoBehaviour is not TMonoBehaviour tMonoBehaviour)
            {
                return;
            }

            _action?.Invoke(context, tMonoBehaviour);
        }
    }

    internal readonly struct MutateInstruction<TMonoBehaviour, TNewComponent> : IInjectableMonoBehaviourInstruction where TMonoBehaviour : MonoBehaviour where TNewComponent : Component
    {
        private readonly Action<Context, TMonoBehaviour, TNewComponent>? _action;
        private readonly Func<TMonoBehaviour, GameObject>? _gameObjectGetter;

        internal MutateInstruction(Action<Context, TMonoBehaviour, TNewComponent>? action, Func<TMonoBehaviour, GameObject>? gameObjectGetter)
        {
            _action = action;
            _gameObjectGetter = gameObjectGetter;
        }

        public readonly void Apply(Context context, MonoBehaviour monoBehaviour)
        {
            if (monoBehaviour is not TMonoBehaviour tMonoBehaviour)
            {
                return;
            }

            GameObject gameObject;

            if (_gameObjectGetter != null)
            {
                gameObject = _gameObjectGetter(tMonoBehaviour);

                if (gameObject == null)
                {
                    throw new ArgumentException("The provided GameObject getter returned null.");
                }
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

    internal readonly struct SceneDecoratorMutateInstruction<TMonoBehaviour> : IInjectableMonoBehaviourInstruction
    {
        private readonly string _contractName;
        private readonly Action<SceneDecoratorContext, TMonoBehaviour>? _action;

        internal SceneDecoratorMutateInstruction(string contractName, Action<SceneDecoratorContext, TMonoBehaviour> action)
        {
            _contractName = contractName;
            _action = action;
        }

        public readonly void Apply(Context context, MonoBehaviour monoBehaviour)
        {
            if (context is not SceneDecoratorContext sceneDecoratorContext || sceneDecoratorContext.DecoratedContractName != _contractName || monoBehaviour is not TMonoBehaviour tMonoBehaviour)
            {
                return;
            }

            _action?.Invoke(sceneDecoratorContext, tMonoBehaviour);
        }
    }
}
