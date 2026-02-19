using ModestTree;
using UnityEngine;
using Zenject;

namespace SiraUtil.Zenject.Internal.Instructors
{
    internal class ParameterizedInstructor : IInstructor
    {
        public void Install(InstallSet installSet, Context context)
        {
            Assert.That(!installSet.InstallerType.IsSubclassOf(typeof(Component)));
            InstallerBase? installerBase = context.Container.Instantiate(installSet.InstallerType, installSet.InitialParameters) as InstallerBase;
            if (installerBase is not null)
            {
                context._normalInstallers.Add(installerBase);
            }
        }
    }
}