using IPA.Loader;
using SiraUtil.Affinity.Harmony.Generator;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SiraUtil.Affinity.Harmony
{
    internal class HarmonyAffinityPatcher : IAffinityPatcher
    {
        private readonly Dictionary<IAffinity, List<MethodInfo>> _patchCache = new();
        private readonly Dictionary<Assembly, DynamicHarmonyPatchGenerator> _patchGenerators = new();

        public void Patch(IAffinity affinity)
        {
            Assembly assembly = affinity.GetType().Assembly;
            if (!_patchGenerators.TryGetValue(assembly, out DynamicHarmonyPatchGenerator dynamicHarmonyPatchGenerator))
            {
                Assembly affinityAssembly = affinity.GetType().Assembly;
                PluginMetadata? metadata = PluginManager.EnabledPlugins.FirstOrDefault(f => f.Assembly == affinityAssembly);
                if (metadata is null)
                {
                    Plugin.Log.Warn($"Could not find an active plugin assembly for '{affinity.GetType().Name}'. Unable to create patch.");
                    return;
                }
                dynamicHarmonyPatchGenerator = new DynamicHarmonyPatchGenerator(metadata);
                _patchGenerators.Add(assembly, dynamicHarmonyPatchGenerator);
            }
            if (!_patchCache.TryGetValue(affinity, out List<MethodInfo> methods))
            {
                _patchCache.Add(affinity, new List<MethodInfo>());
            }

            MethodInfo[] affinityMethods = affinity.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).Where(m => m.CustomAttributes.Any(ca => ca.AttributeType == typeof(AffinityPatchAttribute))).ToArray();
            if (affinityMethods.Length == 0)
            {
                Plugin.Log.Warn($"'{affinity.GetType().Name}' doesn't have any affinity patches! The IAffinity interface is unecessary.");
            }


            foreach (var affinityMethod in affinityMethods)
            {
                AffinityPatchAttribute attribute = affinityMethod.GetCustomAttribute<AffinityPatchAttribute>();
                AffinityPatchType patchType = AffinityPatchType.Postfix;

                if (affinityMethod.GetCustomAttribute<AffinityPrefixAttribute>() is not null)
                    patchType = AffinityPatchType.Prefix;
                else if (affinityMethod.GetCustomAttribute<AffinityTranspilerAttribute>() is not null)
                    patchType = AffinityPatchType.Transpiler;
                else if (affinityMethod.GetCustomAttribute<AffinityFinalizerAttribute>() is not null)
                    patchType = AffinityPatchType.Finalizer;

                string[]? after = null;
                string[]? before = null;

                AffinityAfterAttribute? afterAttribute = affinityMethod.GetCustomAttribute<AffinityAfterAttribute>();
                AffinityBeforeAttribute? beforeAttribute = affinityMethod.GetCustomAttribute<AffinityBeforeAttribute>();

                after = afterAttribute?.After;
                before = beforeAttribute?.Before;

                dynamicHarmonyPatchGenerator.Patch(affinity, affinityMethod, patchType, attribute, before, after);
            }
        }

        public void Unpatch(IAffinity affinity)
        {
            Assembly assembly = affinity.GetType().Assembly;
            if (!_patchCache.TryGetValue(affinity, out List<MethodInfo> methods))
            {
                Plugin.Log.Warn($"Could not find any patch registrations for this instance of '{affinity.GetType().Name}'. Unable to create patch.");
                return;
            }
            if (!_patchGenerators.TryGetValue(assembly, out DynamicHarmonyPatchGenerator dynamicHarmonyPatchGenerator))
            {
                // The patches can only be generated through a generator. So, if the patches are found (which they were, above) but the generator is missing, something signficantly wrong has happened.
                Plugin.Log.Error($"The {nameof(DynamicHarmonyPatchGenerator)} could not be found for an assembly! This should NEVER happen and should be reported!");
                return;
            }
            foreach (var affinityContract in methods)
            {
                dynamicHarmonyPatchGenerator.Unpatch(affinityContract);
            }
        }
    }
}
