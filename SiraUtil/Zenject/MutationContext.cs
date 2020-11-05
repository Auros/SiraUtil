using Zenject;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace SiraUtil.Zenject
{
    public class MutationContext
    {
        private readonly SceneDecoratorContext _decoratorContext;
        private readonly List<MonoBehaviour> _monoBehaviourList;

        public DiContainer Container { get; }

        internal MutationContext(DiContainer container, SceneDecoratorContext decoratorContext, List<MonoBehaviour> monoBehaviourList)
        {
            Container = container;
            _decoratorContext = decoratorContext;
            if (decoratorContext != null)
            {
                _monoBehaviourList = Accessors.Injectables(ref decoratorContext);
            }
            else
            {
                _monoBehaviourList = monoBehaviourList;
            }
        }

        public void AddInjectable(MonoBehaviour behaviour)
        {
            if (_decoratorContext != null)
            {
                var dec = _decoratorContext;
                Accessors.Injectables(ref dec).Add(behaviour);
            }
            else
            {
                Plugin.Log.Warn($"Cannot inject {behaviour.GetType().FullName}. There is no decorator associated with this mutation context.");
            }
        }

        public T GetInjected<T>() where T : MonoBehaviour
        {
            return (T)_monoBehaviourList.FirstOrDefault(x => x.GetType() == typeof(T));
        }
    }
}