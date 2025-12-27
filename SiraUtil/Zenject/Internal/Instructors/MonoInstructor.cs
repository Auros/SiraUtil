using ModestTree;
using Zenject;

namespace SiraUtil.Zenject.Internal.Instructors
{
    internal class MonoInstructor : IInstructor
    {
        public void Install(InstallSet installSet, Context context)
        {
            Assert.DerivesFrom<MonoInstallerBase>(installSet.installerType);
            MonoInstaller monoInstaller = (context.gameObject.AddComponent(installSet.installerType) as MonoInstaller)!;
            if (monoInstaller != null)
                context._monoInstallers.Add(monoInstaller);
        }
    }
}