using ModestTree;
using Zenject;

namespace SiraUtil.Zenject.Internal.Instructors
{
    internal class MonoInstructor : IInstructor
    {
        public void Install(InstallSet installSet, Context context)
        {
            Assert.DerivesFrom<MonoInstallerBase>(installSet.InstallerType);
            MonoInstaller monoInstaller = (context.gameObject.AddComponent(installSet.InstallerType) as MonoInstaller)!;
            if (monoInstaller != null)
            {
                context._monoInstallers.Add(monoInstaller);
            }
        }
    }
}