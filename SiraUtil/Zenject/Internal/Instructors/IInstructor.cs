using Zenject;

namespace SiraUtil.Zenject.Internal.Instructors
{
    internal interface IInstructor
    {
        void Install(InstallSet installSet, Context context);
    }
}