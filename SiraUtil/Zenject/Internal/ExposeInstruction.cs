using UnityEngine;
using Zenject;

namespace SiraUtil.Zenject.Internal
{
    internal readonly struct ExposeInstruction<TMonoBehaviour> : IInjectableMonoBehaviourInstruction where TMonoBehaviour : MonoBehaviour
    {
        public readonly void Apply(Context context, MonoBehaviour monoBehaviour)
        {
            if (monoBehaviour is not TMonoBehaviour tMonoBehaviour)
            {
                return;
            }

            if (context.Container.HasBinding<TMonoBehaviour>())
            {
                return;
            }

            Plugin.Log.Debug($"Exposing {typeof(TMonoBehaviour).FullName} on {context.name} ({context.GetType().Name})");
            context.Container.Bind<TMonoBehaviour>().FromInstance(tMonoBehaviour).AsSingle();
        }
    }

    internal readonly struct SceneDecoratorExposeInstruction<TMonoBehaviour> : IInjectableMonoBehaviourInstruction
    {
        private readonly string? _contractName;

        internal SceneDecoratorExposeInstruction(string contractName)
        {
            _contractName = contractName;
        }

        public readonly void Apply(Context context, MonoBehaviour monoBehaviour)
        {
            if (monoBehaviour is not TMonoBehaviour tMonoBehaviour || context is not SceneDecoratorContext sceneDecoratorContext || sceneDecoratorContext.DecoratedContractName != _contractName)
            {
                return;
            }

            if (context.Container.HasBinding<TMonoBehaviour>())
            {
                return;
            }

            Plugin.Log.Debug($"Exposing {typeof(TMonoBehaviour).FullName} on {context.name} ({context.GetType().Name}) [{context.gameObject.scene.name}]");
            context.Container.Bind<TMonoBehaviour>().FromInstance(tMonoBehaviour).AsSingle();
        }
    }
}
