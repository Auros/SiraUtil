using Zenject;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

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

        /// <summary>
        /// The [potential] decorator context for this mutation.
        /// </summary>
        public SceneDecoratorContext Decorator => _decoratorContext;

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

        /// <summary>
        /// Get an injected object.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="func">The function to search it via.</param>
        /// <returns>The injected object.</returns>
        public T GetInjected<T>(Func<T, bool> func)
        {
            return _monoBehaviourList.Where(u => u.GetType().IsAssignableFrom(typeof(T))).Cast<T>().FirstOrDefault(func);
        }
    }
}