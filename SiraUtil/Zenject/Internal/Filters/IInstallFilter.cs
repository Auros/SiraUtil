using System;
using System.Collections.Generic;
using Zenject;

namespace SiraUtil.Zenject.Internal.Filters
{
    internal interface IInstallFilter
    {
        bool ShouldInstall(Context context, IEnumerable<Type> bindings);
    }
}   