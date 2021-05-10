using IPA.Utilities;
using ModestTree;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace SiraUtil.Zenject.Internal.Exposers
{
    internal class ExposerManager
    {
        private static readonly FieldAccessor<SceneDecoratorContext, List<MonoBehaviour>>.Accessor SceneDecoratorInjectables = FieldAccessor<SceneDecoratorContext, List<MonoBehaviour>>.GetAccessor("_injectableMonoBehaviours");

        public void Install(ExposeSet exposeSet, Context context, ref IEnumerable<MonoBehaviour>? iterList)
        {
            Assert.DerivesFromOrEqual<SceneDecoratorContext>(context.GetType());
            SceneDecoratorContext sceneDecoratorContext = (context as SceneDecoratorContext)!;

            if (exposeSet.locationContractName != sceneDecoratorContext.DecoratedContractName ||
                string.IsNullOrEmpty(sceneDecoratorContext.DecoratedContractName) ||
                string.IsNullOrWhiteSpace(exposeSet.locationContractName))
                return;

            if (iterList is null)
            {
                List<MonoBehaviour> injectableList = new();
                injectableList.AddRange(SceneDecoratorInjectables(ref sceneDecoratorContext));               
                iterList = injectableList;
            }
            MonoBehaviour toExpose = iterList.FirstOrDefault(i => i.GetType() == exposeSet.typeToExpose);
            if (toExpose != null && !context.Container.HasBinding(exposeSet.typeToExpose))
            {
                context.Container.Bind(exposeSet.typeToExpose).FromInstance(toExpose).AsSingle();
            }
            else
            {
                Plugin.Log.Warn($"Could not expose {exposeSet.typeToExpose.Name}. {(toExpose == null ? "It could not be found in the SceneContextDecorator" : "It is already binded.")}");
            }
        }
    }
}