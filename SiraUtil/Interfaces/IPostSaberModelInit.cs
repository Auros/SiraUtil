using UnityEngine;

namespace SiraUtil.Interfaces
{
    /// <summary>
    /// Defines for something to be run after a saber model initializes.
    /// </summary>
    public interface IPostSaberModelInit
    {
        /// <summary>
        /// Runs after model initialization.
        /// </summary>
        /// <param name="parent">The parent of the saber model.</param>
        /// <param name="saber">The saber component associated with the current model.</param>
        void PostInit(Transform parent, Saber saber);
    }
}