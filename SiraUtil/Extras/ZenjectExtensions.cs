using HMUI;
using IPA.Utilities;
using ModestTree;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using VRUIControls;
using Object = UnityEngine.Object;

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
            var go = new GameObject("ViewController");

            go.gameObject.SetActive(false);
            var canvas = go.AddComponent<Canvas>();
            canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.Normal;
            canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
            canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord2;
            canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.Tangent;

            var raycaster = go.AddComponent<DummyRaycaster>();
            var componentBinding = binder.FromNewComponentOn(go);
            raycaster.enabled = false;

            componentBinding.OnInstantiated((ctx, obj) =>
            {
                if (obj is ViewController vc)
                {
                    var newRaycaster = go.AddComponent<VRGraphicRaycaster>();
                    Object.Destroy(raycaster);
                    var cache = ctx.Container.Resolve<PhysicsRaycasterWithCache>();
                    newRaycaster.SetField("_physicsRaycaster", cache);
                    go.name = vc.GetType().Name;
                    var rt = vc.rectTransform;
                    rt.localEulerAngles = Vector3.zero;
                    rt.anchorMax = rt.localScale = Vector3.one;
                    rt.anchorMin = rt.sizeDelta = Vector2.zero;
                }
                onInstantiated?.Invoke(ctx, obj);
            });
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

        internal class DummyRaycaster : BaseRaycaster
        {
            public override Camera eventCamera => Camera.main;

            public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
            {

            }
        }
    }
}