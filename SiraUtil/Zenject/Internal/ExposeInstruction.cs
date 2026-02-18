using HarmonyLib;
using SiraUtil.Extras;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace SiraUtil.Zenject.Internal
{
    internal readonly struct ExposeInstruction<TMonoBehaviour> : IInjectableMonoBehaviourInstruction where TMonoBehaviour : MonoBehaviour
    {
        private readonly object? _identifier;
        private readonly bool _useSceneContext;
        private readonly bool _ifNotBound;
        private readonly Func<Context, TMonoBehaviour, bool>? _condition;
        private readonly IEnumerable<Type> _bindTypes;

        internal ExposeInstruction(object? identifier, bool useSceneContext, bool ifNotBound, Func<Context, TMonoBehaviour, bool>? condition, IEnumerable<Type> bindTypes)
        {
            _identifier = identifier;
            _useSceneContext = useSceneContext;
            _ifNotBound = ifNotBound;
            _condition = condition;
            _bindTypes = bindTypes;
        }

        public int Order => 0;

        public readonly void Apply(Context context, MonoBehaviour monoBehaviour)
        {
            if (monoBehaviour is not TMonoBehaviour tMonoBehaviour)
            {
                return;
            }

            if (_condition?.Invoke(context, tMonoBehaviour) == false)
            {
                return;
            }

            DiContainer container = context.Container;

            if (_useSceneContext && context is GameObjectContext)
            {
                // at this point the parent container is installed and validated so calling Resolve is safe
                context = context.Container.ParentContainers.Single().Resolve<SceneContext>();
            }

            Plugin.Log.Debug($"Exposing '{typeof(TMonoBehaviour).FullDescription()}' on {context.FullDescription()}");

            ScopeConcreteIdArgConditionCopyNonLazyBinder binder = container.Bind(_bindTypes).WithId(_identifier).FromInstance(tMonoBehaviour);

            if (_ifNotBound)
            {
                binder.IfNotBound();
            }
        }
    }

    [Obsolete]
    internal readonly struct SceneDecoratorExposeInstruction<TMonoBehaviour> : IInjectableMonoBehaviourInstruction
    {
        private readonly string? _contractName;

        internal SceneDecoratorExposeInstruction(string contractName)
        {
            _contractName = contractName;
        }

        public int Order => 0;

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

            Plugin.Log.Debug($"Exposing {typeof(TMonoBehaviour).FullName} on {context.FullDescription()}");
            context.Container.Bind<TMonoBehaviour>().FromInstance(tMonoBehaviour).AsSingle();
        }
    }
}
