using Zenject;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace SiraUtil.Zenject
{
    /// <summary>
    /// A context provider for mutating Zenject object.
    /// </summary>
    public class MutationContext
    {
        private readonly SceneDecoratorContext _decoratorContext;
        private readonly List<MonoBehaviour> _monoBehaviourList;

        /// <summary>
        /// The container used in the active mutation.
        /// </summary>
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

        /// <summary>
        /// Adds an injectable to the injectable pool.
        /// </summary>
        /// <param name="behaviour">The pool of injectables.</param>
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

        /// <summary>
        /// Get an injected object.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <returns>The first result in the injectable list.</returns>
        public T GetInjected<T>() where T : MonoBehaviour
        {
            return (T)_monoBehaviourList.FirstOrDefault(x => x.GetType() == typeof(T));
        }
    }
}