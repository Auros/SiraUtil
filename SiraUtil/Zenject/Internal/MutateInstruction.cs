using HarmonyLib;
using SiraUtil.Extras;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SiraUtil.Zenject.Internal
{
    internal record MutateInstruction<TMonoBehaviour> : IInjectableMonoBehaviourInstruction where TMonoBehaviour : MonoBehaviour
    {
        private readonly Action<Context, TMonoBehaviour>? _action;

        internal MutateInstruction(Action<Context, TMonoBehaviour> action, int order)
        {
            _action = action;

            Order = order;
        }

        public int Order { get; }

        public void Apply(Context context, MonoBehaviour monoBehaviour)
        {
            if (monoBehaviour is not TMonoBehaviour tMonoBehaviour)
            {
                return;
            }

            _action?.Invoke(context, tMonoBehaviour);
        }
    }

    internal record MutateInstruction<TMonoBehaviour, TNewComponent> : IInjectableMonoBehaviourInstruction where TMonoBehaviour : MonoBehaviour where TNewComponent : Component
    {
        private readonly Action<Context, TMonoBehaviour, TNewComponent>? _action;
        private readonly Func<Context, TMonoBehaviour, GameObject>? _gameObjectGetter;
        private readonly Func<Context, TMonoBehaviour, bool>? _condition;
        private readonly IEnumerable<Type>? _bindTypes;

        internal MutateInstruction(Action<Context, TMonoBehaviour, TNewComponent>? action, Func<Context, TMonoBehaviour, GameObject>? gameObjectGetter, Func<Context, TMonoBehaviour, bool>? condition, IEnumerable<Type>? bindTypes, int order)
        {
            _action = action;
            _gameObjectGetter = gameObjectGetter;
            _condition = condition;
            _bindTypes = bindTypes;

            Order = order;
        }

        public int Order { get; }

        public void Apply(Context context, MonoBehaviour monoBehaviour)
        {
            if (monoBehaviour is not TMonoBehaviour tMonoBehaviour)
            {
                return;
            }

            if (_condition?.Invoke(context, tMonoBehaviour) == false)
            {
                return;
            }

            GameObject gameObject;

            if (_gameObjectGetter != null)
            {
                gameObject = _gameObjectGetter(context, tMonoBehaviour);

                if (gameObject == null)
                {
                    throw new ArgumentException($"The provided GameObject getter for {nameof(MutateInstruction<,>)} returned null.");
                }
            }
            else
            {
                gameObject = monoBehaviour.gameObject;
            }

            Plugin.Log.Debug($"Adding '{typeof(TNewComponent).FullDescription()}' to '{gameObject.GetTransformPath()}' from {context.FullDescription()}");

            TNewComponent newComponent = gameObject.AddComponent<TNewComponent>();
            context.Container.QueueForInject(newComponent);

            if (_bindTypes != null)
            {
                context.Container.Bind(_bindTypes).FromInstance(newComponent);
            }

            _action?.Invoke(context, tMonoBehaviour, newComponent);
        }
    }

    [Obsolete]
    internal record SceneDecoratorMutateInstruction<TMonoBehaviour> : IInjectableMonoBehaviourInstruction
    {
        private readonly string _contractName;
        private readonly Action<SceneDecoratorContext, TMonoBehaviour>? _action;

        internal SceneDecoratorMutateInstruction(string contractName, Action<SceneDecoratorContext, TMonoBehaviour> action)
        {
            _contractName = contractName;
            _action = action;
        }

        public int Order => 0;

        public void Apply(Context context, MonoBehaviour monoBehaviour)
        {
            if (context is not SceneDecoratorContext sceneDecoratorContext || sceneDecoratorContext.DecoratedContractName != _contractName || monoBehaviour is not TMonoBehaviour tMonoBehaviour)
            {
                return;
            }

            _action?.Invoke(sceneDecoratorContext, tMonoBehaviour);
        }
    }
}
