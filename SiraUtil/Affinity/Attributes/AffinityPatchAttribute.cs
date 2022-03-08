using HarmonyLib;
using System;
using System.Linq;

namespace SiraUtil.Affinity
{
    /// <summary>
    /// An attribute for defining Affinity patch data.
    /// </summary>
    [JetBrains.Annotations.MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class AffinityPatchAttribute : Attribute
    {
        internal Type? DeclaringType { get; }
        internal string? MethodName { get; }
        internal Type[]? ArgumentTypes { get; }
        internal MethodType MethodType { get; }
        internal ArgumentType[]? ArgumentVariations { get; }

        internal bool Complete => DeclaringType != null && MethodName != null;

        /// <summary>
        /// The constructor for an Affinity patch.
        /// </summary>
        /// <param name="declaringType">The type that the method is on.</param>
        /// <param name="methodName">The name of the method.</param>
        /// <param name="methodType">The type of the method.</param>
        /// <param name="argumentVariations">The argument variations of the method.</param>
        /// <param name="argumentTypes">The argument types of the method (for overloads).</param>
        public AffinityPatchAttribute(Type declaringType, string methodName, AffinityMethodType methodType = AffinityMethodType.Normal, AffinityArgumentType[]? argumentVariations = null, params Type[]? argumentTypes)
        {
            MethodName = methodName;
            DeclaringType = declaringType;
            MethodType = (MethodType)methodType;

            if (argumentVariations is not null)
            {
                ArgumentVariations = argumentVariations.Select(aat => (ArgumentType)aat).ToArray();
            }

            if (argumentTypes is not null)
            {
                ArgumentTypes = argumentTypes;
            }
        }

        /// <summary>
        /// Marks this as an affinity patch.
        /// </summary>
        public AffinityPatchAttribute()
        {

        }
    }
}