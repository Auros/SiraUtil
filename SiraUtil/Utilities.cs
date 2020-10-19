using HMUI;
using System;
using Zenject;
using System.IO;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using System.Collections;
using SiraUtil.Interfaces;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace SiraUtil
{
    public static class Utilities
    {
        public const string ASSERTHIT = "(Nice Assert Hit, Ding Dong)";

		public static Task PauseChamp => AwaitSleep(100);

		public static Task AwaitSleep(int ms)
		{
			return Task.Run(() => Thread.Sleep(ms));
		}

		[Obsolete("This will be removed very soon. Please don't let Zenject instantiate the view controller. Create the View Controller manually (for example, through BSML).")]
        public static void SetupViewController(InjectContext context, object source)
        {
            if (source is ViewController viewController)
            {
                viewController.rectTransform.anchorMin = new Vector2(0f, 0f);
                viewController.rectTransform.anchorMax = new Vector2(1f, 1f);
                viewController.rectTransform.sizeDelta = new Vector2(0f, 0f);
                viewController.rectTransform.anchoredPosition = new Vector2(0f, 0f);
            }
        }

        internal static IEnumerator ChangeColorCoroutine(Saber saber, Color color, float time = 0.05f)
        {
            if (time != 0)
            {
                yield return new WaitForSeconds(time);
            }
            SaberModelController modelController = saber.gameObject.GetComponentInChildren<SaberModelController>(true);
            if (modelController is IColorable colorable)
            {
                colorable.SetColor(color);
            }
            if (modelController is SaberModelController smc)
            {
                Color tintColor = Accessors.ModelInitData(ref smc).trailTintColor;
                SetSaberGlowColor[] setSaberGlowColors = Accessors.SaberGlowColor(ref smc);
                SetSaberFakeGlowColor[] setSaberFakeGlowColors = Accessors.FakeSaberGlowColor(ref smc);
                TubeBloomPrePassLight light = Accessors.SaberLight(ref smc);
                saber.ChangeColor(color, smc, tintColor, setSaberGlowColors, setSaberFakeGlowColors, light);
            }
        }

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

        public static string GetResourceContent(Assembly assembly, string resource)
        {
            using (Stream stream = assembly.GetManifestResourceStream(resource))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static void AssemblyFromPath(string inputPath, out Assembly assembly, out string path)
        {
            string[] parameters = inputPath.Split(':');
            switch (parameters.Length)
            {
                case 1:
                    path = parameters[0];
                    assembly = Assembly.Load(path.Substring(0, path.IndexOf('.')));
                    break;
                case 2:
                    path = parameters[1];
                    assembly = Assembly.Load(parameters[0]);
                    break;
                default:
                    throw new Exception($"Could not process resource path {inputPath}");
            }
        }

        public static byte[] GetResource(Assembly asm, string ResourceName)
        {
            Stream stream = asm.GetManifestResourceStream(ResourceName);
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, (int)stream.Length);
            return data;
        }
    }
}