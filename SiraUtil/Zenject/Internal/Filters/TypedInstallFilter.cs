using ModestTree;
using System;
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

        public bool ShouldInstall(ContextBinding binding)
        {
            return _installerType == binding.installerType;
        }
    }
}