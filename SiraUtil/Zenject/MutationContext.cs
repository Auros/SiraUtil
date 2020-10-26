using Zenject;
using UnityEngine;
using System.Linq;

namespace SiraUtil.Zenject
{
    public class MutationContext
    {
        private readonly SceneDecoratorContext _decoratorContext;

        public DiContainer Container { get; }

        internal MutationContext(DiContainer container, SceneDecoratorContext decoratorContext)
        {
            Container = container;
            _decoratorContext = decoratorContext;
        }

        public void AddInjectable(MonoBehaviour behaviour)
        {
            var dec = _decoratorContext;
            Accessors.Injectables(ref dec).Add(behaviour);
        }

        public T GetInjected<T>() where T : MonoBehaviour
        {
            var dec = _decoratorContext;
            return (T)Accessors.Injectables(ref dec).FirstOrDefault(x => x.GetType() == typeof(T));
        }
    }
}