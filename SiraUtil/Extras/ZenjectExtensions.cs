using HMUI;
using IPA.Utilities;
using ModestTree;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRUIControls;

namespace Zenject
{
    /// <summary>
    /// Contains extensions for zenject related things.
    /// </summary>
    public static class ZenjectExtensions
    {
        /// <summary>
        /// Binds a view controller to the container.
        /// </summary>
        /// <param name="binder">The binder.</param>
        /// <param name="onInstantiated">The callback when the view controller is instantiated.</param>
        /// <returns></returns>
        public static ScopeConcreteIdArgConditionCopyNonLazyBinder FromNewComponentAsViewController(this FromBinder binder, Action<InjectContext, object> onInstantiated = null!)
        {
            var go = new GameObject(binder.ConcreteTypes.FirstOrDefault(t => typeof(ViewController).IsAssignableFrom(t))?.Name)
            {
                layer = 5,
            };

            go.SetActive(false);

            RectTransform rt = go.AddComponent<RectTransform>();
            rt.localEulerAngles = Vector3.zero;
            rt.anchorMax = rt.localScale = Vector3.one;
            rt.anchorMin = rt.sizeDelta = Vector2.zero;

            var canvas = go.AddComponent<Canvas>();
            canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord2;

            binder.BindContainer.QueueForInject(go.AddComponent<VRGraphicRaycaster>());
            var componentBinding = binder.FromNewComponentOn(go);

            componentBinding.OnInstantiated(onInstantiated);
            return componentBinding;
        }

        private static readonly PropertyAccessor<FromBinder, IBindingFinalizer>.Setter FromBinder_SubFinalizer = PropertyAccessor<FromBinder, IBindingFinalizer>.GetSetter("SubFinalizer");
        private static readonly PropertyAccessor<FromBinder, IEnumerable<Type>>.Getter FromBinder_ConcreteTypes = PropertyAccessor<FromBinder, IEnumerable<Type>>.GetGetter("ConcreteTypes");

        // From Extenject https://github.com/Mathijs-Bakker/Extenject/blob/1e2b6fc88fed215ade79aa914887fef115d3328e/UnityProject/Assets/Plugins/Zenject/Source/Binding/Binders/FromBinders/FromBinder.cs#L295
        /// <summary>
        /// Creates a new component on a new GameObject
        /// </summary>
        /// <param name="fromBinder"></param>
        /// <returns></returns>
        public static NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder FromNewComponentOnNewGameObject(this FromBinder fromBinder)
        {
            return FromNewComponentOnNewGameObject(fromBinder, new GameObjectCreationParameters());
        }

        // From Extenject https://github.com/Mathijs-Bakker/Extenject/blob/1e2b6fc88fed215ade79aa914887fef115d3328e/UnityProject/Assets/Plugins/Zenject/Source/Binding/Binders/FromBinders/FromBinder.cs#L300
        private static NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder FromNewComponentOnNewGameObject(FromBinder fromBinder, GameObjectCreationParameters gameObjectInfo)
        {
            var concreteTypes = FromBinder_ConcreteTypes(ref fromBinder);
            foreach (Type type in concreteTypes)
            {
                Assert.That(type.DerivesFrom(typeof(Component)), "Invalid type given during bind command.  Expected type '{0}' to derive from UnityEngine.Component", type);
                Assert.That(!type.IsAbstract(), "Invalid type given during bind command.  Expected type '{0}' to not be abstract.", type);
            }

            fromBinder.BindInfo.RequireExplicitScope = true;
            var finalizer = new ScopableBindingFinalizer(
                fromBinder.BindInfo,
                (container, type) => new AddToNewGameObjectComponentProvider(
                    container,
                    type,
                    fromBinder.BindInfo.Arguments,
                    gameObjectInfo, fromBinder.BindInfo.ConcreteIdentifier, fromBinder.BindInfo.InstantiatedCallback));
            FromBinder_SubFinalizer(ref fromBinder, finalizer);

            return new NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder(fromBinder.BindInfo, gameObjectInfo);
        }
    }
}