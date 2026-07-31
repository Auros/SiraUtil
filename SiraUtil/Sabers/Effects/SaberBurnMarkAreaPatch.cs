using HarmonyLib;
using UnityEngine;

namespace SiraUtil.Sabers.Effects
{
    [HarmonyPatch(typeof(SaberBurnMarkArea))]
    internal class SaberBurnMarkAreaPatch
    {
        [HarmonyPrefix]
        [HarmonyPriority(Priority.Last)]
        [HarmonyPatch(nameof(SaberBurnMarkArea.LateUpdate))]
        internal static bool SaberBurnMarkArea_LateUpdate(SaberBurnMarkArea __instance)
        {
            if (!__instance._sabers[0])
            {
                return false;
            }

            Transform transform = __instance.transform;
            Plane plane = new(transform.up, transform.position);

            int idx = 0;
            bool first = true;
            bool any = false;

            for (int i = 0; i < __instance._sabers.Length; i++)
            {
                Saber saber = __instance._sabers[i];
                Vector3 burnMarkPos = Vector3.zero;
                bool valid = saber.isActiveAndEnabled && SaberBurnMarkArea.GetBurnMarkPos(transform, in __instance._bounds, in plane, saber.saberBladeBottomPosForVisualEffects, saber.saberBladeTopPosForVisualEffects, out burnMarkPos);
                Vector2 pos = valid ? __instance.WorldToNormalized(burnMarkPos) : Vector3.zero;
                if (valid && __instance._prevBurnMarkPosValid[i])
                {
                    Vector2 prevPos = __instance._prevBurnMarkPos[i];
                    __instance._fadeOutMaterial.SetVector(SaberBurnMarkArea._segShaderPropertyIDs[idx], new Vector4(prevPos.x, prevPos.y, pos.x, pos.y));
                    __instance._fadeOutMaterial.SetColor(SaberBurnMarkArea._segColorShaderPropertyIDs[idx], __instance._saberColors[i]);
                    any = true;
                    ++idx;
                }

                __instance._prevBurnMarkPos[i] = pos;
                __instance._prevBurnMarkPosValid[i] = valid;

                if (idx == 2)
                {
                    idx = 0;
                    Blit(__instance, first);
                    first = false;
                }
            }

            if (any)
            {
                __instance._disableBlitTimer = 0;
            }
            else
            {
                __instance._disableBlitTimer += Time.deltaTime;
            }

            if (__instance._disableBlitTimer < 5f)
            {
                if (idx == 0)
                {
                    __instance._fadeOutMaterial.SetColor(SaberBurnMarkArea._segColorShaderPropertyIDs[0], Color.clear);
                }

                __instance._fadeOutMaterial.SetColor(SaberBurnMarkArea._segColorShaderPropertyIDs[1], Color.clear);

                Blit(__instance, first);
            }

            __instance._renderMaterial.mainTexture = __instance._renderTextures[0];

            return false;
        }

        private static void Blit(SaberBurnMarkArea __instance, bool first)
        {
            // only fade the first blit, just write the marks for any subsequent ones
            float value = first ? Mathf.Max(0f, 1f - (Time.deltaTime * __instance._burnMarksFadeOutStrength)) : 0;
            __instance._fadeOutMaterial.SetFloat(SaberBurnMarkArea._fadeOutStrengthShaderPropertyID, value);

            RenderTexture[] renderTextures = __instance._renderTextures;

            Graphics.Blit(renderTextures[0], renderTextures[1], __instance._fadeOutMaterial);

            (renderTextures[0], renderTextures[1]) = (renderTextures[1], renderTextures[0]);
        }
    }
}