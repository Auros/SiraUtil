using ModestTree;
using Zenject;

namespace SiraUtil.Zenject.Internal.Instructors
{
    internal class MonoInstructor : IInstructor
    {
        public void Install(InstallSet installSet, ContextBinding contextBinding)
        {
            Assert.DerivesFrom<MonoInstallerBase>(installSet.installerType);
            MonoInstaller monoInstaller = (contextBinding.context.gameObject.AddComponent(installSet.installerType) as MonoInstaller)!;
            if (monoInstaller != null)
                contextBinding.AddInstaller(monoInstaller);
        }
    }
}