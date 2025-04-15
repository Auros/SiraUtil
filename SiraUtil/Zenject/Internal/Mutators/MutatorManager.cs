using ModestTree;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace SiraUtil.Zenject.Internal.Mutators
{
    internal class MutatorManager
    {
        public void Install(MutateSet mutateSet, Context context, ref IEnumerable<MonoBehaviour>? iterList)
        {
            Assert.DerivesFromOrEqual<SceneDecoratorContext>(context.GetType());
            SceneDecoratorContext sceneDecoratorContext = (context as SceneDecoratorContext)!;

            if (mutateSet.locationContractName != sceneDecoratorContext.DecoratedContractName ||
                string.IsNullOrEmpty(sceneDecoratorContext.DecoratedContractName) ||
                string.IsNullOrWhiteSpace(mutateSet.locationContractName))
                return;

            if (iterList is null)
            {
                List<MonoBehaviour> injectableList = new();
                injectableList.AddRange(sceneDecoratorContext._injectableMonoBehaviours);
                iterList = injectableList;
            }
            MonoBehaviour? toMutate = iterList.FirstOrDefault(il => il.GetType() == mutateSet.typeToMutate);
            if (toMutate is not null)
            {
                mutateSet.onMutate.actionObj?.Invoke(sceneDecoratorContext, toMutate);
            }
            else
            {
                Plugin.Log.Warn($"Could not find {mutateSet.typeToMutate.Name} in {mutateSet.locationContractName}.");
            }
        }
    }
}