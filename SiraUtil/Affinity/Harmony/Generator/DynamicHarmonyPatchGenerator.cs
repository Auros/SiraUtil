using HarmonyLib;
using IPA.Loader;
using IPA.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace SiraUtil.Affinity.Harmony.Generator
{
    internal class DynamicHarmonyPatchGenerator : IDisposable
    {
        private readonly string _id;
        private readonly AssemblyName _assemblyName;
        private readonly HarmonyLib.Harmony _harmony;
        private readonly ModuleBuilder _moduleBuilder;
        private readonly AssemblyBuilder _assemblyBuilder;
        private readonly Dictionary<MethodInfo, MethodBase> _patchCache = new();

        public DynamicHarmonyPatchGenerator(PluginMetadata pluginMetadata)
        { 
            _id = $"com.{(string.IsNullOrEmpty(pluginMetadata.Author) ? "unknown" : pluginMetadata.Author)}.{pluginMetadata.Name}.affinity".ToLower();
            _harmony = new HarmonyLib.Harmony(_id);

            _assemblyName = new($"{pluginMetadata.Assembly.GetName().Name}.AffinityPatched");
            _assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(_assemblyName,
#if DEBUG
                AssemblyBuilderAccess.RunAndSave
#else
                AssemblyBuilderAccess.Run
#endif
            );
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule(_assemblyName.Name, $"{_assemblyName.Name}.dll");
        }

        public MethodInfo Patch(IAffinity affinity, MethodInfo affinityMethod, AffinityPatchType patchType, AffinityPatchAttribute patch, int priority = -1, string[]? before = null, string[]? after = null)
        {
            const string patchName = "Patch";
            const string invokeName = "Invoke";
            const string delegateName = "_delegate";

            TypeBuilder typeBuilder = _moduleBuilder.DefineType($"{patch.DeclaringType}_{patch.MethodName}_{affinityMethod.Name}_{Guid.NewGuid().ToString().Replace("-", "_")}", TypeAttributes.Public);

            Type[]? types = null;
            if (patch.ArgumentTypes is not null && patch.ArgumentTypes.Length != 0)
                types = patch.ArgumentTypes;

            MethodInfo originalMethod = AccessTools.Method(patch.DeclaringType, patch.MethodName, types);

            // Create the delegate used to invoke the affinity instance method.
            var delegateType = CreateDelegateType(affinityMethod);
            FieldBuilder affinityDelegate = typeBuilder.DefineField(delegateName, delegateType, FieldAttributes.Public | FieldAttributes.Static);

            // Create the method and its parameters.
            ParameterInfo[] parameters = affinityMethod.GetParameters();
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(patchName, MethodAttributes.Public | MethodAttributes.Static, affinityMethod.ReturnType, parameters.Select(f => f.ParameterType).ToArray());
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameter = parameters[i];
                methodBuilder.DefineParameter(i + 1, parameter.Attributes, parameter.Name);
            }

            // Generate the IL to call the delegate from the method.
            ILGenerator ilg = methodBuilder.GetILGenerator();
            ilg.Emit(OpCodes.Nop);
            ilg.Emit(OpCodes.Ldsfld, affinityDelegate);
            for (int i = 0; i < affinityMethod.GetParameters().Length; i++)
                ilg.Emit(OpCodes.Ldarg_S, i);
            ilg.Emit(OpCodes.Callvirt, delegateType.GetMethod(invokeName));
            
            if (affinityMethod.ReturnType == typeof(void))
                ilg.Emit(OpCodes.Nop);
            ilg.Emit(OpCodes.Ret);

            // Construct the type and assign the delegate.
            Type finalizedType = typeBuilder.CreateType();
            MethodInfo constructedPatchMethod = finalizedType.GetMethod(patchName);
            finalizedType.GetField(delegateName).SetValue(null, Delegate.CreateDelegate(delegateType, affinity, affinityMethod));

            HarmonyMethod? prefix = null;
            HarmonyMethod? postfix = null;
            HarmonyMethod? transpiler = null;
            HarmonyMethod? finalizer = null;

            HarmonyMethod @base = new(constructedPatchMethod, priority, before, after);

            // Patch!
            switch (patchType)
            {
                case AffinityPatchType.Prefix:
                    prefix = @base;
                    break;
                case AffinityPatchType.Postfix:
                    postfix = @base;
                    break;
                case AffinityPatchType.Transpiler:
                    transpiler = @base;
                    break;
                case AffinityPatchType.Finalizer:
                    finalizer = @base;
                    break;
            }

            _harmony.Patch(originalMethod, prefix, postfix, transpiler, finalizer);
            _patchCache.Add(constructedPatchMethod, originalMethod);

            return constructedPatchMethod;
        }

        public void Unpatch(MethodInfo contract)
        {
            if (_patchCache.TryGetValue(contract, out MethodBase original))
            {
                _harmony.Unpatch(original, contract);
                _patchCache.Remove(contract);
            }
        }

        // https://stackoverflow.com/questions/9505117/creating-delegates-dynamically-with-parameter-names
        private Type CreateDelegateType(MethodInfo method)
        {
            string nameBase = string.Format("{0}{1}", method.DeclaringType.Name, method.Name);
            string name = GetUniqueName(nameBase);

            var typeBuilder = _moduleBuilder.DefineType(
                name, TypeAttributes.Sealed | TypeAttributes.Public, typeof(MulticastDelegate));

            var constructor = typeBuilder.DefineConstructor(
                MethodAttributes.RTSpecialName | MethodAttributes.HideBySig | MethodAttributes.Public,
                CallingConventions.Standard, new[] { typeof(object), typeof(IntPtr) });
            constructor.SetImplementationFlags(MethodImplAttributes.CodeTypeMask);

            var parameters = method.GetParameters();

            var invokeMethod = typeBuilder.DefineMethod(
                "Invoke", MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Public,
                method.ReturnType, parameters.Select(p => p.ParameterType).ToArray());
            invokeMethod.SetImplementationFlags(MethodImplAttributes.CodeTypeMask);

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                invokeMethod.DefineParameter(i + 1, ParameterAttributes.None, parameter.Name);
            }

            return typeBuilder.CreateType();
        }

        private string GetUniqueName(string nameBase)
        {
            int number = 2;
            string name = nameBase;
            while (_moduleBuilder.GetType(name) != null)
                name = nameBase + number++;
            return name;
        }

        public void Dispose()
        {
#if DEBUG
            string pathName = Path.Combine(UnityGame.InstallPath, "SiraUtil Affinity Assemblies");
            Directory.CreateDirectory(pathName);
            string fileName = $"{_assemblyName.Name}.dll";
            string endFile = Path.Combine(pathName, fileName);
            _assemblyBuilder.Save($"{_assemblyName.Name}.dll");
            File.Delete(endFile);
            File.Move(fileName, Path.Combine(pathName, fileName));
#endif
        }
    }
}
