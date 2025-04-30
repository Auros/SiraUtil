using UnityEngine;

namespace SiraUtil.Tools.FPFC
{
    // this class needs to stay around until this patch in Heck is removed: https://github.com/Aeroluna/Heck/blob/7a0a9cfcac617b1dc14d3d646d3fc1acf2248bf2/Heck/HarmonyPatches/SiraUtilHeadFinder.cs
    internal class GameTransformFPFCListener : IFPFCListener
    {
#pragma warning disable CS0414
        private readonly Transform? _originalHeadTransform = null;
#pragma warning restore CS0414

        public void Enabled()
        {
        }

        public void Disabled()
        {
        }
    }
}