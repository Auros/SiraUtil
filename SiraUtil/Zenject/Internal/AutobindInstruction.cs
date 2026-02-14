using HMUI;
using ModestTree;
using SiraUtil.Affinity;
using SiraUtil.Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SiraUtil.Zenject.Internal
{
    internal readonly struct AutobindInstruction
    {
        private readonly Type[]? _contracts;
        private readonly Type[] _majorContract;
        private readonly BindAttribute _bindAttribute;
        private static readonly Type[] _autoTypes =
        [
            typeof(IInitializable),
            typeof(ITickable),
            typeof(IFixedTickable),
            typeof(ILateTickable),
            typeof(IDisposable),
            typeof(ILateDisposable),
            typeof(IAsyncInitializable),
            typeof(IAffinity)
        ];

        public readonly Location Location => _bindAttribute.Location;

        public AutobindInstruction(Type majorContract, BindAttribute bindAttribute)
        {
            _bindAttribute = bindAttribute;
            _majorContract = [majorContract];

            if (_bindAttribute.Contracts is null)
            {
                List<Type> contracts = [];
                foreach (Type type in _autoTypes)
                    if (majorContract.DerivesFrom(type))
                        contracts.Add(type);
                if (contracts.Count > 0)
                {
                    contracts.Add(majorContract);
                    _contracts = [.. contracts];
                }
                else
                    _contracts = null;
            }
            else
            {
                _contracts = _bindAttribute.Contracts;
            }
        }

        public readonly void Bind(DiContainer Container)
        {
            FromBinderNonGeneric contractBind = Container.Bind(_contracts ?? _majorContract).To(_majorContract);
            if (_majorContract[0].DerivesFrom<ViewController>())
            {
                _ = contractBind.FromNewComponentAsViewController();
            }
            else if (_majorContract[0].DerivesFrom<MonoBehaviour>())
            {
                _ = contractBind.FromNewComponentOnNewGameObject();
            }
            ConcreteIdArgConditionCopyNonLazyBinder typeBind = _bindAttribute.BindType switch
            {
                BindType.Transient => contractBind.AsTransient(),
                BindType.Cached => contractBind.AsCached(),
                _ => contractBind.AsSingle(),
            };
            if (_bindAttribute.NonLazy)
            {
                typeBind.NonLazy();
            }
        }
    }
}