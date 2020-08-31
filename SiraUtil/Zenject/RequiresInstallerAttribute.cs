using System;

namespace SiraUtil.Zenject
{
    /// <summary>
    /// Prevents an installer from being installed if the installer in this attribute has not been installed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RequiresInstallerAttribute : Attribute
    {
        /// <summary>
        /// The type of the required installer.
        /// </summary>
        public Type RequiredInstaller;

        /// <summary>
        /// Prevents an installer from being installed if the installer in this attribute has not been installed.
        /// </summary>
        /// <param name="requiredType">The type of the required installer.</param>
        public RequiresInstallerAttribute(Type requiredType)
        {
            RequiredInstaller = requiredType;
        } 
    }
}