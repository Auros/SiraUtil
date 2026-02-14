using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace SiraUtil.Extras
{
    /// <summary>
    /// A collection of utilities used in SiraUtil.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Check if the following code instructions starting from a given index match a list of opcodes.
        /// </summary>
        /// <param name="codes">A list of code instructions to check</param>
        /// <param name="toCheck">A list of op codes that is expected to match</param>
        /// <param name="startIndex">Index to start checking from (inclusive)</param>
        /// <returns>Whether or not the op codes found in the code instructions match.</returns>
        public static bool OpCodeSequence(List<CodeInstruction> codes, List<OpCode> toCheck, int startIndex)
        {
            for (int i = 0; i < toCheck.Count; i++)
            {
                if (codes[startIndex + i].opcode != toCheck[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// A simple Task which awaits for 100ms. Mainly used to make UI feel more "natural" and for testing purposes.
        /// </summary>
        public static Task PauseChamp => AwaitSleep(100);

        /// <summary>
        /// Returns a task which awaits for a time in milliseconds.
        /// </summary>
        /// <param name="ms">The time in milliseconds to await for.</param>
        /// <returns></returns>
        public static Task AwaitSleep(int ms)
        {
            return Task.Run(() => Thread.Sleep(ms));
        }

        /// <summary>
        /// Gets the Embedded Asset at a specific path in an Assembly.
        /// </summary>
        /// <param name="assembly">The Assembly that contains the resource.</param>
        /// <param name="resource">The path to the resource in the assembly.</param>
        /// <returns></returns>
        public static string GetResourceContent(Assembly assembly, string resource)
        {
            using Stream stream = assembly.GetManifestResourceStream(resource);
            using StreamReader? reader = new(stream);
            return reader.ReadToEnd();
        }

        /// <summary>
        /// Gets an assembly from a path.
        /// </summary>
        public static void AssemblyFromPath(string inputPath, out Assembly assembly, out string path)
        {
            string[] parameters = inputPath.Split(':');
            switch (parameters.Length)
            {
                case 1:
                    path = parameters[0];
                    assembly = Assembly.Load(path[..path.IndexOf('.')]);
                    break;
                case 2:
                    path = parameters[1];
                    assembly = Assembly.Load(parameters[0]);
                    break;
                default:
                    throw new Exception($"Could not process resource path {inputPath}");
            }
        }

        /// <summary>
        /// Gets the raw resource from an Assembly.
        /// </summary>
        /// <param name="asm">The assembly that contains the resource.</param>
        /// <param name="ResourceName">The path to the resource in the assembly.</param>
        /// <returns>The raw byte data of the resource.</returns>
        public static byte[] GetResource(Assembly asm, string ResourceName)
        {
            using Stream stream = asm.GetManifestResourceStream(ResourceName);
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, (int)stream.Length);
            return data;
        }

        internal static string FullDescription(this Context context) => $"{context.GetType().Name} '{GetTransformPath(context)}'";

        internal static string GetTransformPath(this GameObject gameObject) => GetTransformPath(gameObject.transform);

        internal static string GetTransformPath(this Component component)
        {
            if (component is not Transform transform)
            {
                transform = component.transform;
            }

            return GetTransformPath(transform);
        }

        internal static string GetTransformPath(this Transform transform)
        {
            string sceneName = transform.gameObject.scene.name;
            List<string> parts = [];

            while (transform != null)
            {
                parts.Add(transform.name);
                transform = transform.parent;
            }

            parts.Reverse();

            return $"{sceneName}/{string.Join("/", parts)}";
        }
    }
}