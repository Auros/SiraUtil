using System;

namespace SiraUtil.Zenject
{
    [Obsolete("The new Zenjector system will automatically restart the game if the project installer did not go off.")]
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