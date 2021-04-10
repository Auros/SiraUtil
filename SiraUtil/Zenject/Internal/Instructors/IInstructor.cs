using System;

namespace SiraUtil.Zenject.Internal.Instructors
{
    internal interface IInstructor
    {
        void Install(InstallSet installSet, ContextBinding contextBinding);
    }
}