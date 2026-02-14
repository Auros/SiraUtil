using UnityEngine;
using Zenject;

namespace SiraUtil.Zenject.Internal.Instructors
{
    internal class InstructorManager
    {
        private readonly IInstructor _monoInstructor = new MonoInstructor();
        private readonly IInstructor _typedInstructor = new TypedInstructor();
        private readonly IInstructor _parameterizedInstructor = new ParameterizedInstructor();

        public IInstructor? InstructorForSet(InstallSet installSet)
        {
            // If it's not a component AND it has parameters, its a paramertized instructor.
            if (!installSet.installerType.IsSubclassOf(typeof(Component)) && installSet.initialParameters is not null)
            {
                return _parameterizedInstructor;
            }

            // If it inherits MonoInstallerBase, it's a mono instructor.
            if (installSet.installerType.IsSubclassOf(typeof(MonoInstallerBase)))
            {
                return _monoInstructor;
            }

            // If it's not a component, its a typed instructor.
            if (!installSet.installerType.IsSubclassOf(typeof(Component)))
            {
                return _typedInstructor;
            }

            // We don't know what it is!
            return null;
        }
    }
}