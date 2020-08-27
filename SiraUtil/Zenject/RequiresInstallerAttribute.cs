using System;

namespace SiraUtil.Zenject
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RequiresInstallerAttribute : Attribute
    {
        public Type RequiredInstaller;

        public RequiresInstallerAttribute(Type requiredType)
        {
            RequiredInstaller = requiredType;
        } 
    }
}