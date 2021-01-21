using System;
using Zenject;
using ModestTree;
using System.Collections.Generic;

namespace SiraUtil.Zenject
{
    /// <summary>
    /// Standardization for containerizing upgraded objects.
    /// </summary>
    /// <typeparam name="U"></typeparam>
    public class Containerized<U>
    {
        /// <summary>
        /// The component.
        /// </summary>
        protected U Self;

        /// <summary>
        /// The container in this containerized component.
        /// </summary>
        [Inject]
        protected DiContainer Container;

        private readonly Dictionary<Type, object> _resolveCache = new Dictionary<Type, object>();
        private readonly Dictionary<object, object> _resolveIdCache = new Dictionary<object, object>();

        /// <summary>
        /// Resolves and cached data from a containerized component.
        /// </summary>
        /// <typeparam name="T">The type of the object to resolve.</typeparam>
        /// <returns>The resolved object.</returns>
        protected virtual T Resolve<T>()
        {
            Type type = typeof(T);
            if (!_resolveCache.ContainsKey(type))
            {
                Assert.IsNotNull(Container);
                T value = Container.Resolve<T>();

                Assert.IsNotNull(value);
                _resolveCache.Add(type, value);
                return value;
            }
            return (T)_resolveCache[type];
        }

        /// <summary>
        /// Resolves and cached data from a containerized component based on an identifier.
        /// </summary>
        /// <typeparam name="T">The type of the object to resolve.</typeparam>
        /// <param name="id">The identifier to resolve by.</param>
        /// <returns>The resolved object.</returns>
        protected virtual T ResolveId<T>(object id)
        {
            if (!_resolveIdCache.ContainsKey(id))
            {
                Assert.IsNotNull(Container);
                T value = Container.ResolveId<T>(id);

                Assert.IsNotNull(value);
                _resolveIdCache.Add(id, value);
                return value;
            }
            return (T)_resolveIdCache[id];
        }
    }
}