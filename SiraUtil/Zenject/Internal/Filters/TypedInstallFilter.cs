using ModestTree;
using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace SiraUtil.Zenject.Internal.Filters
{
    internal class TypedInstallFilter : IInstallFilter
    {
        private readonly Type _installerType;

        public TypedInstallFilter(Type installerType)
        {
            Assert.DerivesFrom<IInstaller>(installerType);
            _installerType = installerType;
        }

        public bool ShouldInstall(Context context, IEnumerable<Type> bindings)
        {
            return bindings.Any(b => b == _installerType);
        }
    }
}