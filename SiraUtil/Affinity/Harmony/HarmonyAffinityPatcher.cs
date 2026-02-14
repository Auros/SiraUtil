using HarmonyLib;
using IPA.Loader;
using SiraUtil.Affinity.Harmony.Generator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SiraUtil.Affinity.Harmony
{
    internal class HarmonyAffinityPatcher : IAffinityPatcher
    {
        private readonly Dictionary<IAffinity, List<MethodInfo>> _patchCache = [];
        private readonly Dictionary<Assembly, DynamicHarmonyPatchGenerator> _patchGenerators = [];

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
                methods = [];
                _patchCache.Add(affinity, methods);
            }

            Type affinityType = affinity.GetType();
            MethodInfo[] affinityMethods = [.. affinityType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).Where(m => m.CustomAttributes.Any(ca => ca.AttributeType == typeof(AffinityPatchAttribute)))];
            if (affinityMethods.Length == 0)
            {
                Plugin.Log.Warn($"'{affinity.GetType().FullDescription()}' doesn't have any affinity patches! The IAffinity interface is unecessary.");
            }

            AffinityPatchAttribute? classAffinityPatch = affinityType.GetCustomAttribute<AffinityPatchAttribute>();

            foreach (MethodInfo affinityMethod in affinityMethods)
            {
                foreach (AffinityPatchAttribute attribute in affinityMethod.GetCustomAttributes<AffinityPatchAttribute>())
                {
                    AffinityPatchType patchType = AffinityPatchType.Postfix;

                    if (affinityMethod.GetCustomAttribute<AffinityPrefixAttribute>() is not null)
                    {
                        patchType = AffinityPatchType.Prefix;
                    }
                    else if (affinityMethod.GetCustomAttribute<AffinityTranspilerAttribute>() is not null)
                    {
                        patchType = AffinityPatchType.Transpiler;
                    }
                    else if (affinityMethod.GetCustomAttribute<AffinityFinalizerAttribute>() is not null)
                    {
                        patchType = AffinityPatchType.Finalizer;
                    }

                    string[]? after = null;
                    string[]? before = null;
                    int priority = -1;

                    AffinityAfterAttribute? afterAttribute = affinityMethod.GetCustomAttribute<AffinityAfterAttribute>();
                    AffinityBeforeAttribute? beforeAttribute = affinityMethod.GetCustomAttribute<AffinityBeforeAttribute>();
                    AffinityPriorityAttribute? priorityAttribute = affinityMethod.GetCustomAttribute<AffinityPriorityAttribute>();

                    after = afterAttribute?.After;
                    before = beforeAttribute?.Before;
                    priority = priorityAttribute?.Priority ?? -1;

                    if (!attribute.Complete && classAffinityPatch is null)
                    {
                        throw new AffinityException("No patches?? Could not find completed [AffinityPatch(...)] attribute for this method. Make sure that the method or the class that it inherits has a non-parameterless AffinityPatch attribute.");
                    }

                    try
                    {
                        MethodInfo contract = dynamicHarmonyPatchGenerator.Patch(affinity, affinityMethod, patchType, attribute.Complete ? attribute : classAffinityPatch, priority, before, after);
                        methods.Add(contract);
                    }
                    catch (Exception ex)
                    {
                        throw new AffinityException($"Failed to execute patch for method '{affinityMethod}' on '{affinityMethod.DeclaringType.FullName}' (assembly '{affinityMethod.DeclaringType.Assembly.GetName().Name}')", ex);
                    }
                }
            }
        }

        public void Unpatch(IAffinity affinity)
        {
            Assembly assembly = affinity.GetType().Assembly;
            if (!_patchCache.TryGetValue(affinity, out List<MethodInfo> methods))
            {
                Plugin.Log.Warn($"Could not find any patch registrations for this instance of '{affinity.GetType().Name}'. Unable to unpatch.");
                return;
            }
            if (!_patchGenerators.TryGetValue(assembly, out DynamicHarmonyPatchGenerator dynamicHarmonyPatchGenerator))
            {
                // The patches can only be generated through a generator. So, if the patches are found (which they were, above) but the generator is missing, something signficantly wrong has happened.
                Plugin.Log.Error($"The {nameof(DynamicHarmonyPatchGenerator)} could not be found for an assembly! This should NEVER happen and should be reported!");
                return;
            }
            foreach (MethodInfo affinityContract in methods)
            {
                dynamicHarmonyPatchGenerator.Unpatch(affinityContract);
            }
        }

        public void Dispose()
        {
            foreach (KeyValuePair<Assembly, DynamicHarmonyPatchGenerator> patcher in _patchGenerators)
            {
                patcher.Value.Dispose();
            }
        }
    }
}