   
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.VFX;

   namespace Utils
   {
    class Utils{
        public static IEnumerator InterpolatVolumeVisibility(bool isVisible, Volume volume,float duration)
        {
            volume.enabled=true;
            float elapsedTime = 0f;
            float tpm = 0f;
            while(volume.weight>0 && !isVisible || isVisible && volume.weight<1)
            {
                tpm = elapsedTime / duration;
                volume.weight = Mathf.Min(Mathf.Max(isVisible ? tpm: 1.0f-tpm,0.0f),1.0f);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            volume.enabled=isVisible;
            yield return null;
        }

        public static IEnumerator InterpolatVfxIntVisibility(bool isVisible,string parameter_name,int final_value, VisualEffect vfx,float duration)
        {
            vfx.enabled=true;
            float elapsedTime = 0f;
            float tpm = 0f;
            float current_value = vfx.GetInt(parameter_name);
            
            while(current_value>0 && !isVisible || isVisible && current_value<final_value)
            {
                tpm = elapsedTime / duration;
                int value =Mathf.RoundToInt(Mathf.Min(Mathf.Max(isVisible ? tpm: 1.0f-tpm,0.0f),1.0f)*final_value);
                vfx.SetInt(parameter_name,value);
                current_value=value;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            vfx.enabled=isVisible;
            yield return null;
        }
    }
   }