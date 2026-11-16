using System;
using System.Collections.Generic;
using Zenject;

namespace SiraUtil.Zenject.Internal.Filters
{
    internal class ContextInstallFilter<T> : IInstallFilter where T : Context
    {
        public virtual bool ShouldInstall(Context context, IEnumerable<Type> installerBindings)
        {
            return context.GetType() == typeof(T);
        }
    }
}
