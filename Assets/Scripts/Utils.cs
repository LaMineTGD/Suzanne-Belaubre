
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.VFX;

namespace Utils
{
    public enum TailoredPostProcess
    {
        Vignette,
        Bloom,
        Tonemapping,
        MotionBlur
    }

    class Utils
    {
        public static IEnumerator InterpolatVolumeVisibility(bool isVisible, Volume volume, float duration)
        {
            volume.enabled = true;
            float elapsedTime = 0f;
            float tpm = 0f;
            while (volume.weight > 0 && !isVisible || isVisible && volume.weight < 1)
            {
                tpm = elapsedTime / duration;
                volume.weight = Mathf.Min(Mathf.Max(isVisible ? tpm : 1.0f - tpm, 0.0f), 1.0f);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            volume.enabled = isVisible;
            yield return null;
        }

        public static IEnumerator InterpolatVfxIntVisibility(bool isVisible, string parameter_name, int final_value, VisualEffect vfx, float duration)
        {
            vfx.enabled = true;
            float elapsedTime = 0f;
            float tpm = 0f;
            float current_value = vfx.GetInt(parameter_name);

            while (current_value > 0 && !isVisible || isVisible && current_value < final_value)
            {
                tpm = elapsedTime / duration;
                int value = Mathf.RoundToInt(Mathf.Min(Mathf.Max(isVisible ? tpm : 1.0f - tpm, 0.0f), 1.0f) * final_value);
                vfx.SetInt(parameter_name, value);
                current_value = value;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            vfx.enabled = isVisible;
            yield return null;
        }

        public static IEnumerator InterpolatVfxFloatVisibility(bool ToMax, string parameter_name, float max_value, VisualEffect vfx, float duration, float min_value = 0)
        {
            vfx.enabled = true;
            float elapsedTime = 0f;
            float tpm = 0f;
            float current_value = vfx.GetFloat(parameter_name);

            while (current_value > min_value && !ToMax || ToMax && current_value < max_value)
            {
                tpm = elapsedTime / duration;
                float value = Mathf.Min(Mathf.Max(ToMax ? tpm : 1.0f - tpm, 0.0f), 1.0f) * (max_value - min_value) + min_value;
                vfx.SetFloat(parameter_name, value);
                current_value = value;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            vfx.enabled = ToMax || min_value > 0;
            yield return null;
        }

        public static IEnumerator InterpolatLightOff(Light light, float duration)
        {
            light.enabled = true;
            float elapsedTime = 0f;
            float tpm = 0f;
            float fromIntensity = light.intensity;

            while (elapsedTime < duration)
            {
                tpm = elapsedTime / duration;
                light.intensity = Mathf.Lerp(fromIntensity, 0f, tpm);

                elapsedTime += Time.deltaTime;
                yield return null;
            }
            yield return null;
            light.enabled = false;
        }
    }
}