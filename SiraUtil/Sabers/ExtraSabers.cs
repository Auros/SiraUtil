using System;

namespace SiraUtil.Sabers
{
    [Obsolete("No longer necessary. Done automatically.")]
    public static class ExtraSabers
    {
        /// <summary>
        /// Registers your assembly for using ExtraSabers. This enables the ability to request the SiraSaber.Factory in Zenject.
        /// </summary>
        public static void Touch()
        {
            
        }

        /// <summary>
        /// Unregisters your assembly for using ExtraSabers. Make sure to do this when you disable your mod.
        /// </summary>
        public static void Untouch()
        {
            
        }
    }
}