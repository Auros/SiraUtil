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
    internal struct AutobindInstruction
    {
        private readonly Type[]? _contracts;
        private readonly Type[] _majorContract;
        private readonly BindAttribute _bindAttribute;
        private static readonly Type[] _autoTypes = new Type[]
        {
            typeof(IInitializable),
            typeof(ITickable),
            typeof(IFixedTickable),
            typeof(ILateTickable),
            typeof(IDisposable),
            typeof(ILateDisposable),
            typeof(IAsyncInitializable),
            typeof(IAffinity)
        };

        public Location Location => _bindAttribute.Location;

        public AutobindInstruction(Type majorContract, BindAttribute bindAttribute)
        {
            _bindAttribute = bindAttribute;
            _majorContract = new Type[] { majorContract };

            if (_bindAttribute.Contracts is null)
            {
                List<Type> contracts = new();
                foreach (var type in _autoTypes)
                    if (majorContract.DerivesFrom(type))
                        contracts.Add(type);
                if (contracts.Count > 0)
                {
                    contracts.Add(majorContract);
                    _contracts = contracts.ToArray();
                }
                else
                    _contracts = null;
            }
            else
            {
                _contracts = _bindAttribute.Contracts;
            }
        }

        public void Bind(DiContainer Container)
        {
            var contractBind = Container.Bind(_contracts ?? _majorContract).To(_majorContract);
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