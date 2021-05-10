using System;
using Zenject;

namespace SiraUtil.Zenject.Internal
{
    internal class DelegateWrapper
    {
        public Action<SceneDecoratorContext, object>? actionObj;

        public DelegateWrapper Wrap<T, U>(Action<T, U> callback) where T : SceneDecoratorContext
        {
            actionObj = delegate (SceneDecoratorContext context, object obj)
            {
                callback((T)context, (U)obj);
            };
            return this;
        }
    }
}