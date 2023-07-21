   
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

   namespace Utils
   {
    class Utils{
        public static IEnumerator InterpolatVolumeVisibility(bool isVisible, Volume volume,float duration)
        {
            volume.enabled=true;
            float elapsedTime = 0f;
            float tpm = 0f;
            while(volume.weight>0 && !isVisible || isVisible && volume.weight<1)
            //while(elapsedTime<duration)
            {
                tpm = elapsedTime / duration;
                volume.weight = Mathf.Min(Mathf.Max(isVisible ? tpm: 1.0f-tpm,0.0f),1.0f);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            volume.enabled=isVisible;
            yield return null;
        }
    }
   }