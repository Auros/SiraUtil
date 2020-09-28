using HMUI;
using System;
using Zenject;
using System.IO;
using HarmonyLib;
using UnityEngine;
using SiraUtil.Sabers;
using System.Reflection;
using System.Collections;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace SiraUtil
{
	public static class Utilities
    {
        public const string ASSERTHIT = "(Nice Assert Hit, Ding Dong)";

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
            ISaberModelController modelController = saber.gameObject.GetComponentInChildren<ISaberModelController>(true);
            if (modelController is BasicSaberModelController bsmc)
            {
                Color tintColor = Accessors.ModelInitData(ref bsmc).trailTintColor;
                SetSaberGlowColor[] setSaberGlowColors = Accessors.SaberGlowColor(ref bsmc);
                SetSaberFakeGlowColor[] setSaberFakeGlowColors = Accessors.FakeSaberGlowColor(ref bsmc);
                Light light = Accessors.SaberLight(ref bsmc);
                saber.ChangeColor(color, bsmc, tintColor, setSaberGlowColors, setSaberFakeGlowColors, light);
            }
            else if (modelController is IColorable colorable)
            {
                colorable.SetColor(color);
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