using ModestTree;
using UnityEngine;
using Zenject;

namespace SiraUtil.Zenject.Internal.Instructors
{
    internal class ParameterizedInstructor : IInstructor
    {
        public void Install(InstallSet installSet, ContextBinding contextBinding)
        {
            Assert.That(!installSet.installerType.IsSubclassOf(typeof(Component)));
            InstallerBase? installerBase = contextBinding.context.Container.Instantiate(installSet.installerType, installSet.initialParameters) as InstallerBase;
            if (installerBase is not null)
                contextBinding.AddInstaller(installerBase);
        }
    }
}