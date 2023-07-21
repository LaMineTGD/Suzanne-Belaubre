using System.Collections;
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


    public void SetVisible(bool isVisible, float duration){
        StartCoroutine( Utils.Utils.InterpolatVolumeVisibility(isVisible,m_postProcessVolume,duration));
    }
}
