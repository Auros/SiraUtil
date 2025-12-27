using ModestTree;
using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace SiraUtil.Zenject.Internal.Filters
{
    internal class MultiTypedInstallFilter : IInstallFilter
    {
        private readonly IEnumerable<Type> _installerTypes;

        public MultiTypedInstallFilter(IEnumerable<Type> installerTypes)
        {
            // Make sure that all types being registered inside here are actually installers,
            // then make a copy of the enumerable.
            foreach (var type in installerTypes)
                Assert.DerivesFrom<IInstaller>(type);
            _installerTypes = installerTypes.ToArray();
        }

        public bool ShouldInstall(Context context, IEnumerable<Type> installerBindings)
        {
            return _installerTypes.Intersect(installerBindings).Any();
        }
    }
}