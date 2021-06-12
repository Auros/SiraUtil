using HMUI;
using IPA.Utilities;
using SiraUtil.Objects;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using VRUIControls;
using Zenject;
using Object = UnityEngine.Object;

namespace SiraUtil.Extras
{
    /// <summary>
    /// Some public extensions.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Registers a redecorator for the object API.
        /// </summary>
        /// <remarks>
        /// This cannot be called on the App scene, please only call this as you're installing your game related bindings.
        /// </remarks>
        /// <typeparam name="TRegistrator"></typeparam>
        /// <param name="container"></param>
        /// <param name="registrator"></param>
        public static void RegisterRedecorator<TRegistrator>(this DiContainer container, TRegistrator registrator) where TRegistrator : RedecoratorRegistration
        {
            container.AncestorContainers[0].Bind<RedecoratorRegistration>().FromInstance(registrator).AsCached();
        }

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
                    rt.localScale = rt.anchorMax = Vector3.one;
                    rt.anchorMin = rt.sizeDelta = Vector2.zero;
                }
                onInstantiated?.Invoke(ctx, obj);
            });
            return componentBinding;
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