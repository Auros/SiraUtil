using UnityEngine;

namespace SiraUtil.Interfaces
{
    /// <summary>
    /// Defines for something to be run before a saber model initializes.
    /// </summary>
    public interface IPreSaberModelInit
    {
        /// <summary>
        /// Runs before model initialization.
        /// </summary>
        /// <param name="parent">The parent of the saber model.</param>
        /// <param name="saber">The saber component associated with the current model.</param>
        /// <returns>Do you want the original Init to run?</returns>
        bool PreInit(Transform parent, Saber saber);
    }
}