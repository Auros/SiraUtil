using ModestTree;
using UnityEngine;

namespace SiraUtil.Zenject.Internal.Instructors
{
    internal class TypedInstructor : IInstructor
    {
        public void Install(InstallSet installSet, ContextBinding contextBinding)
        {
            Assert.That(!installSet.installerType.IsSubclassOf(typeof(Component)));
            contextBinding.AddInstaller(installSet.installerType);
        }
    }
}