using ModestTree;
using UnityEngine;
using Zenject;

namespace SiraUtil.Zenject.Internal.Instructors
{
    internal class TypedInstructor : IInstructor
    {
        public void Install(InstallSet installSet, Context context)
        {
            Assert.That(!installSet.InstallerType.IsSubclassOf(typeof(Component)));
            context._normalInstallerTypes.Add(installSet.InstallerType);
        }
    }
}