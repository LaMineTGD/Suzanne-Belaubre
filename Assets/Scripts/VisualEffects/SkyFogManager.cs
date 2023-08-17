using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using Utils;

public class SkyFogManager : MonoBehaviour
{
    public enum SkyLevel { Top, Middle, Bottom, All };

    private float m_colorInterpDuration = 100f;
    private float m_TailorTranstionInterpDuration = 20f;
    private Volume m_SkyFog;
    private VolumeProfile m_SkyFogProfile;
    private GradientSky m_GradientSky;
    private IEnumerator m_InterpCoroutine;

    private void Awake()
    {
        m_SkyFog = GetComponent<Volume>();
        m_SkyFogProfile = m_SkyFog.sharedProfile;
        
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
                case(SkyLevel.All):
                    m_GradientSky.bottom.overrideState = true;
                    Color fromBotColor = m_GradientSky.bottom.value;
                    m_GradientSky.bottom.Interp(fromBotColor, Color.white, elapsedTime / m_colorInterpDuration);
                    
                    m_GradientSky.middle.overrideState = true;
                    Color fromMidColor = m_GradientSky.middle.value;
                    m_GradientSky.middle.Interp(fromMidColor, toColor, elapsedTime / m_colorInterpDuration);
                    
                    m_GradientSky.top.overrideState = true;
                    Color fromTopColor = m_GradientSky.top.value;
                    m_GradientSky.top.Interp(fromTopColor, Color.white, elapsedTime / m_colorInterpDuration);
                    break;

            }

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        yield return null;
    }

    public void TransitionTailorMadeSkyColor(SkyLevel level, Color fromColor, Color toColor)
    {
        if(m_InterpCoroutine != null)
        {
            StopCoroutine(m_InterpCoroutine);
        }

        m_InterpCoroutine = InterpolateTailorMadeSkyColor(level, fromColor, toColor);
        StartCoroutine(m_InterpCoroutine);
    }

    private IEnumerator InterpolateTailorMadeSkyColor(SkyLevel level, Color fromColor, Color toColor)
    {
        float elapsedTime = 0f;

        m_GradientSky.top.value = fromColor;
        m_GradientSky.middle.value = fromColor;
        m_GradientSky.bottom.value = fromColor;

        while(elapsedTime < m_TailorTranstionInterpDuration)
        {
            switch(level)
            {
                case(SkyLevel.Top):
                    m_GradientSky.top.overrideState = true;
                    m_GradientSky.top.Interp(fromColor, toColor, elapsedTime / m_TailorTranstionInterpDuration);
                    break;

                case(SkyLevel.Middle):
                    m_GradientSky.middle.overrideState = true;
                    m_GradientSky.middle.Interp(fromColor, toColor, elapsedTime / m_TailorTranstionInterpDuration);
                    break;
                
                case(SkyLevel.Bottom):
                    m_GradientSky.bottom.overrideState = true;
                    m_GradientSky.bottom.Interp(fromColor, toColor, elapsedTime / m_TailorTranstionInterpDuration);
                    break;
                case(SkyLevel.All):
                    m_GradientSky.bottom.overrideState = true;
                    m_GradientSky.bottom.Interp(fromColor, Color.white, elapsedTime / m_TailorTranstionInterpDuration);
                    
                    m_GradientSky.middle.overrideState = true;
                    m_GradientSky.middle.Interp(fromColor, toColor, elapsedTime / m_TailorTranstionInterpDuration);

                    m_GradientSky.top.overrideState = true;
                    m_GradientSky.top.Interp(fromColor, Color.white, elapsedTime / m_TailorTranstionInterpDuration);
                    break;
            }

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        yield return null;
    }

    public void SetVisible(bool isVisible, float duration)
    {
        StartCoroutine(Utils.Utils.InterpolatVolumeVisibility(isVisible,m_SkyFog,duration));
    }


}
