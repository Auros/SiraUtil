using HMUI;
using System;
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

        // From Extenject https://github.com/Mathijs-Bakker/Extenject/blob/1e2b6fc88fed215ade79aa914887fef115d3328e/UnityProject/Assets/Plugins/Zenject/Source/Binding/Binders/FromBinders/FromBinder.cs#L295
        /// <summary>
        /// Creates a new component on a new GameObject
        /// </summary>
        /// <param name="fromBinder"></param>
        /// <returns></returns>
        [Obsolete("This is now provided directly by Zenject.")]
        public static NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder FromNewComponentOnNewGameObject(this FromBinder fromBinder)
        {
            return FromNewComponentOnNewGameObject(fromBinder, new GameObjectCreationParameters());
        }

        // From Extenject https://github.com/Mathijs-Bakker/Extenject/blob/1e2b6fc88fed215ade79aa914887fef115d3328e/UnityProject/Assets/Plugins/Zenject/Source/Binding/Binders/FromBinders/FromBinder.cs#L300
        private static NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder FromNewComponentOnNewGameObject(FromBinder fromBinder, GameObjectCreationParameters gameObjectInfo)
        {
            var concreteTypes = fromBinder.ConcreteTypes;
            BindingUtil.AssertIsComponent(concreteTypes);
            BindingUtil.AssertIsNotAbstract(concreteTypes);

            fromBinder.BindInfo.RequireExplicitScope = true;
            fromBinder.SubFinalizer = new ScopableBindingFinalizer(
                fromBinder.BindInfo,
                (container, type) => new AddToNewGameObjectComponentProvider(
                    container,
                    type,
                    fromBinder.BindInfo.Arguments,
                    gameObjectInfo, fromBinder.BindInfo.ConcreteIdentifier, fromBinder.BindInfo.InstantiatedCallback));

            return new NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder(fromBinder.BindInfo, gameObjectInfo);
        }
    }
}