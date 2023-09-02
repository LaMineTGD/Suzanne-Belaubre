using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;

public class TrackTailorMadeManager : ITrackManager
{
    [SerializeField] protected Volume m_SkyFogVolume;
    [SerializeField] protected Volume m_PostProcessVolume;
    [SerializeField] protected VisualEffect m_VFX;
    [SerializeField] protected Light m_Light;

    protected float base_rate_value = 0;

    protected string rate_name = "Rate";

    protected override void Start()
    {
        base.Start();
        ApplyDefaultEffects();
    }

    protected override void ApplyDefaultEffects()
    {
        if (ShowManager.m_Instance != null)
            base.ApplyDefaultEffects();
        float duration = ShowManager.m_Instance != null ? ShowManager.m_Instance.GetCurrentTrack()._Start_Transition_duration : 2;
        if (m_SkyFogVolume != null)
            m_SkyFogVolume.weight = 0;
        if (m_PostProcessVolume != null)
            m_PostProcessVolume.weight = 0;
        if (m_VFX != null)
        {
            base_rate_value = m_VFX.GetFloat(rate_name);
            m_VFX.SetFloat(rate_name, 0);
        }
        SkyTransition(true, duration);
        PostProcessTransition(true, duration);
        VFXTransition(true, duration);
        if (ShowManager.m_Instance != null)
            ShowManager.m_Instance.SetCurrentTailorTrack(this);
    }

    public void SetTransitionToVisibleOff(float duration)
    {
        SkyTransition(false, duration);
        PostProcessTransition(false, duration);
        VFXTransition(false, duration);
        LightOffTransition(duration);
    }

    protected void SkyTransition(bool isVisible, float duration)
    {
        if (m_SkyFogVolume != null)
            //StartCoroutine(Utils.Utils.InterpolatVolumeVisibility(isVisible, m_SkyFogVolume, duration));
            m_SkyFogVolume.weight = 1;
    }

    protected void PostProcessTransition(bool isVisible, float duration)
    {
        if (m_PostProcessVolume != null)
            //StartCoroutine(Utils.Utils.InterpolatVolumeVisibility(isVisible, m_PostProcessVolume, duration));
            m_PostProcessVolume.weight = 1;
    }

    protected void VFXTransition(bool isVisible, float duration)
    {
        if (m_VFX != null)
            StartCoroutine(Utils.Utils.InterpolatVfxFloatVisibility(isVisible, rate_name, base_rate_value, m_VFX, duration));
    }

    protected void LightOffTransition(float duration)
    {
        if(m_Light != null)
        {
            StartCoroutine(Utils.Utils.InterpolatLightOff(m_Light, duration));
        }
    }

    public Volume GetTailorMadeSkyVolume()
    {
        return m_SkyFogVolume;
    }

    public Volume GetTailorMadePostProcessVolume()
    {
        return m_PostProcessVolume;
    }
}
