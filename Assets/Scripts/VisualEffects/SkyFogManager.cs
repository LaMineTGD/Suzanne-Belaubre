using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class SkyFogManager : MonoBehaviour
{
    public enum SkyLevel { Top, Middle, Bottom };

    [SerializeField] private float m_colorInterpDuration = 100f;

    private VolumeProfile m_SkyFogProfile;
    private GradientSky m_GradientSky;
    private IEnumerator m_InterpCoroutine;

    private void Awake()
    {
        m_SkyFogProfile = GetComponent<Volume>().sharedProfile;
        
        if(m_SkyFogProfile.TryGet<GradientSky>(out var gradientSky))
        {
            m_GradientSky = gradientSky;
        }
        else
        {
            Debug.LogWarning("Missing GradientSky component");
        }
    }

    public void SetSkyColor(SkyLevel level, Color color)
    {
        if(m_InterpCoroutine != null)
        {
            StopCoroutine(m_InterpCoroutine);
        }

        m_InterpCoroutine = InterpolateSkyColor(level, color);
        StartCoroutine(m_InterpCoroutine);
    }

    private IEnumerator InterpolateSkyColor(SkyLevel level, Color toColor)
    {
        float elapsedTime = 0f;
        while(elapsedTime < m_colorInterpDuration)
        {
            switch(level)
            {
                case(SkyLevel.Top):
                    m_GradientSky.top.overrideState = true;
                    Color currentTopColor = m_GradientSky.top.value;
                    m_GradientSky.top.Interp(currentTopColor, toColor, elapsedTime / m_colorInterpDuration);
                    break;

                case(SkyLevel.Middle):
                    m_GradientSky.middle.overrideState = true;
                    Color currentMidColor = m_GradientSky.middle.value;
                    m_GradientSky.middle.Interp(currentMidColor, toColor, elapsedTime / m_colorInterpDuration);
                    break;
                
                case(SkyLevel.Bottom):
                    m_GradientSky.bottom.overrideState = true;
                    Color currentBotColor = m_GradientSky.bottom.value;
                    m_GradientSky.bottom.Interp(currentBotColor, toColor, elapsedTime / m_colorInterpDuration);
                    break;
            }

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        yield return null;
    }
}
