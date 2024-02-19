using HarmonyLib;
using IPA.Loader;
using IPA.Utilities;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using TypeAttributes = System.Reflection.TypeAttributes;
using FieldAttributes = System.Reflection.FieldAttributes;
using MethodAttributes = System.Reflection.MethodAttributes;
using ParameterAttributes = System.Reflection.ParameterAttributes;
using MethodImplAttributes = System.Reflection.MethodImplAttributes;

namespace SiraUtil.Affinity.Harmony.Generator
{
    internal class DynamicHarmonyPatchGenerator : IDisposable
    {
        private readonly string _id;
        private readonly string _name;
        private readonly HarmonyLib.Harmony _harmony;
        private readonly ModuleBuilder _moduleBuilder;
        private static AssemblyBuilder _assemblyBuilder = null!;
        private static readonly AssemblyName _assemblyName = new($"SiraUtil.Affinity");
        private readonly Dictionary<MethodInfo, (MethodBase, FieldInfo)> _patchCache = new();

        public DynamicHarmonyPatchGenerator(PluginMetadata pluginMetadata)
        {
            _id = $"com.{(string.IsNullOrEmpty(pluginMetadata.Author) ? "unknown" : pluginMetadata.Author)}.{pluginMetadata.Name}.affinity".ToLower();
            _name = pluginMetadata.Assembly.GetName().Name + "_(Generated)";
            _harmony = new HarmonyLib.Harmony(_id);

            if (_assemblyBuilder == null)
            {
                _assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(_assemblyName,
#if DEBUG
                AssemblyBuilderAccess.RunAndSave
#else
                AssemblyBuilderAccess.RunAndCollect
#endif
                );
            }

            ModuleBuilder module = _assemblyBuilder.GetDynamicModule(_name);
            if (module == null)
            {
#if DEBUG
                _moduleBuilder = _assemblyBuilder.DefineDynamicModule(_name, $"{_name}.Affinity.dll");
#else
                _moduleBuilder = _assemblyBuilder.DefineDynamicModule(_name);
#endif
            }
            else
            {
                _moduleBuilder = module;
            }
        }

        public MethodInfo Patch(IAffinity affinity, MethodInfo affinityMethod, AffinityPatchType patchType, AffinityPatchAttribute patch, int priority = -1, string[]? before = null, string[]? after = null)
        {
            const string patchName = "Patch";
            const string invokeName = "Invoke";
            const string delegateName = "_delegate";

            string typeName = $"_{patch.MethodName}_{affinityMethod.Name}_{Guid.NewGuid().ToString().Replace("-", "_")}";
            TypeBuilder typeBuilder = _moduleBuilder.DefineType(typeName, TypeAttributes.Public);

            Type[]? types = null;
            if (patch.ArgumentTypes is not null && patch.ArgumentTypes.Length != 0)
            {
                types = patch.ArgumentTypes;
                ParseSpecialArguments(types, patch.ArgumentVariations);
            }

            MethodBase originalMethod = patch.MethodType switch
            {
                // For a normal method, it'll first check the delcared type for the method. If it can't find it, it'll look to the types in the bases,
                MethodType.Normal => AccessTools.DeclaredMethod(patch.DeclaringType, patch.MethodName, types) ?? AccessTools.Method(patch.DeclaringType, patch.MethodName, types),
                MethodType.Getter => AccessTools.PropertyGetter(patch.DeclaringType, patch.MethodName),
                MethodType.Setter => AccessTools.PropertySetter(patch.DeclaringType, patch.MethodName),
                MethodType.Constructor => AccessTools.Constructor(patch.DeclaringType, types, false),
                MethodType.StaticConstructor => AccessTools.Constructor(patch.DeclaringType, types, true),
                _ => throw new NotImplementedException($"MethodType '{patch.MethodType}' is unrecognized.")
            };

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
            FieldInfo delegateField = finalizedType.GetField(delegateName);
            delegateField.SetValue(null, Delegate.CreateDelegate(delegateType, affinity, affinityMethod));

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

            _harmony.Patch(originalMethod, prefix, postfix, transpiler, finalizer, null);
            _patchCache.Add(constructedPatchMethod, (originalMethod, delegateField));
            return constructedPatchMethod;
        }

        public void Unpatch(MethodInfo contract)
        {
            if (_patchCache.TryGetValue(contract, out (MethodBase, FieldInfo) original))
            {
                _harmony.Unpatch(original.Item1, contract);
                original.Item2.SetValue(null, null);
                _patchCache.Remove(contract);
            }
        }

        // adapted from https://github.com/pardeike/Harmony/blob/77d37bee5bffd053681b34ba70a650d6d2d45486/Harmony/Public/Attributes.cs#L335-L366
        private void ParseSpecialArguments(Type[] argumentTypes, ArgumentType[]? argumentVariations)
        {
            if (argumentVariations is null || argumentVariations.Length == 0)
            {
                return;
            }

            if (argumentTypes.Length < argumentVariations.Length)
            {
                throw new ArgumentException("argumentVariations contains more elements than argumentTypes", nameof(argumentVariations));
            }

            for (var i = 0; i < argumentTypes.Length; i++)
            {
                var type = argumentTypes[i];
                switch (argumentVariations[i])
                {
                    case ArgumentType.Ref:
                    case ArgumentType.Out:
                        argumentTypes[i] = type.MakeByRefType();
                        break;
                    case ArgumentType.Pointer:
                        argumentTypes[i] = type.MakePointerType();
                        break;
                }
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
            foreach (var contract in _patchCache)
            {
                _harmony.Unpatch(contract.Value.Item1, contract.Key);
                contract.Value.Item2.SetValue(null, null);
            }
            _patchCache.Clear();
        }

#if DEBUG
        public static void Save()
        {
            string pathName = Path.Combine(UnityGame.InstallPath, "SiraUtil Affinity Assemblies");
            Directory.CreateDirectory(pathName);
            string fileName = $"IGNORE_ME.dll";
            _assemblyBuilder.Save(fileName);
            File.Delete(fileName);

            foreach (var file in new DirectoryInfo(UnityGame.InstallPath).EnumerateFiles())
            {
                if (file.Name.Contains("_(Generated).Affinity"))
                {
                    string newSavePath = Path.Combine(pathName, file.Name.Replace("_(Generated).Affinity", string.Empty));
                    File.Delete(newSavePath);
                    File.Move(file.FullName, newSavePath);
                }
            }
        }
#endif
    }
}