using System;
using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class PostProcessVolumeManager : MonoBehaviour
{
    [SerializeField] private float m_interpDuration = 100f;
    [SerializeField] private float m_interiorVignetteIntensity = .4f;

    private Volume m_postProcessVolume;
    private VolumeProfile m_postProcessVolumeProfile;
    private Vignette m_vignette;
    private IEnumerator m_InterpCoroutine;

    private Bloom m_bloom;
    private MotionBlur m_motionBlur;
    private Tonemapping m_tonemapping;
    private float m_interpOffDuration = 5f;

    private void Awake()
    {
        m_postProcessVolume = GetComponent<Volume>();
        m_postProcessVolumeProfile = m_postProcessVolume.sharedProfile;
        
        if(m_postProcessVolumeProfile.TryGet<Vignette>(out var vignette))
        {
            m_vignette = vignette;
        }
        else
        {
            Debug.LogWarning("Missing Vignette component");
        }
    }

    public void SetVignette(ShowManager.Location location)
    {
        if(m_InterpCoroutine != null)
        {
            StopCoroutine(m_InterpCoroutine);
        }

        m_InterpCoroutine = InterpolateVignette(location);
        StartCoroutine(m_InterpCoroutine);
    }

    private IEnumerator InterpolateVignette(ShowManager.Location location)
    {
        float elapsedTime = 0f;
        float currentIntensity = (float)m_vignette.intensity;
        
        while(elapsedTime < m_interpDuration)
        {
            switch(location)
            {
                case(ShowManager.Location.Interior):
                    m_vignette.intensity.value =  Mathf.Lerp(currentIntensity, m_interiorVignetteIntensity, elapsedTime / m_interpDuration);
                    break;

                case(ShowManager.Location.Exterior):
                    m_vignette.intensity.value =  Mathf.Lerp(currentIntensity, 0f, elapsedTime / m_interpDuration);
                    break;
                
                case(ShowManager.Location.Both):
                    break;
                case(ShowManager.Location.Neither):
                    m_vignette.intensity.value =  Mathf.Lerp(currentIntensity, 0f, elapsedTime / m_interpDuration);
                    break;
                case(ShowManager.Location.InBetween):
                    break;
            }

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        yield return null;
    }

    public void TransitionTailorMadePostProcess(Volume postProcessVolume)
    {
        if(postProcessVolume.sharedProfile.TryGet<Bloom>(out var bloom))
        {
            if(bloom.IsActive())
            {
                if(m_postProcessVolumeProfile.TryGet<Bloom>(out var spectacleBloom))
                {
                    spectacleBloom.active = true;
                    spectacleBloom.quality.overrideState = bloom.quality.overrideState;
                    spectacleBloom.quality.value = bloom.quality.value;
                    spectacleBloom.threshold.overrideState = bloom.threshold.overrideState;
                    spectacleBloom.threshold.value = bloom.threshold.value;
                    spectacleBloom.intensity.overrideState = bloom.intensity.overrideState;
                    spectacleBloom.intensity.value = bloom.intensity.value;
                    m_bloom = spectacleBloom;
                }
                StartCoroutine(InterpolateTailorBloomOff());
            }
        } 

        if(postProcessVolume.sharedProfile.TryGet<MotionBlur>(out var motionBlur))
        {
            if(motionBlur.IsActive())
            {
                if(m_postProcessVolumeProfile.TryGet<MotionBlur>(out var spectacleMotionBlur))
                {
                    spectacleMotionBlur.active = true;
                    spectacleMotionBlur.intensity.overrideState = motionBlur.intensity.overrideState;
                    spectacleMotionBlur.intensity.value = motionBlur.intensity.value;
                    m_motionBlur = spectacleMotionBlur;
                }
                StartCoroutine(InterpolateTailorMotionBlurOff());
            }
        }

        // if(postProcessVolume.sharedProfile.TryGet<Tonemapping>(out var tonemapping))
        // {
        //     if(tonemapping.IsActive())
        //     {
        //         if(m_postProcessVolumeProfile.TryGet<Tonemapping>(out var spectacleTonemapping))
        //         {
        //             spectacleTonemapping.active = true;
        //             spectacleTonemapping.mode.overrideState = tonemapping.mode.overrideState;
        //             spectacleTonemapping.mode.value = tonemapping.mode.value;
        //             m_tonemapping = spectacleTonemapping;
        //         }
        //     }
        // } 
    }

    private IEnumerator InterpolateTailorBloomOff()
    {
        float elapsedTime = 0f;
        float fromIntensity = (float)m_bloom.intensity.value;
        
        while(elapsedTime < m_interpOffDuration)
        {
            m_bloom.intensity.value =  Mathf.Lerp(fromIntensity, 0f, elapsedTime / m_interpOffDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        m_bloom.active = false;
        yield return null;
    }

    private IEnumerator InterpolateTailorMotionBlurOff()
    {
        float elapsedTime = 0f;
        float fromIntensity = (float)m_motionBlur.intensity.value;
        
        while(elapsedTime < m_interpOffDuration)
        {
            m_motionBlur.intensity.value =  Mathf.Lerp(fromIntensity, 0f, elapsedTime / m_interpOffDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        m_motionBlur.active = false;
        yield return null;
    }



    public void SetVisible(bool isVisible, float duration){
        StartCoroutine( Utils.Utils.InterpolatVolumeVisibility(isVisible,m_postProcessVolume,duration));
    }
}
