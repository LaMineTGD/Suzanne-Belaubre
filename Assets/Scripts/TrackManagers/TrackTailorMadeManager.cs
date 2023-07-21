using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;

public class TrackTailorMadeManager : ITrackManager
{
    [SerializeField] private Volume m_SkyFogVolume;
    [SerializeField] private Volume m_postProcessVolume;
    [SerializeField] private VisualEffect m_VFX;

    protected override void Start()
    {
        base.Start();
        ApplyDefaultEffects();
    }

    protected override void ApplyDefaultEffects() {
        base.ApplyDefaultEffects();
        float duration =ShowManager.m_Instance.GetCurrentTrack()._Start_Transition_duration;
        m_SkyFogVolume.weight = 0;
        m_postProcessVolume.weight = 0;
        if (m_SkyFogVolume != null)
            StartCoroutine(Utils.Utils.InterpolatVolumeVisibility(true,m_SkyFogVolume,duration));
        if (m_postProcessVolume != null)
            StartCoroutine(Utils.Utils.InterpolatVolumeVisibility(true,m_postProcessVolume,duration));
        ShowManager.m_Instance.SetCurrentTailorTrack(this);
    }

    public void SetTransitionToVisibleOff(float duration){
        if (m_SkyFogVolume != null)
            StartCoroutine(Utils.Utils.InterpolatVolumeVisibility(false,m_SkyFogVolume,duration));
        if (m_postProcessVolume != null)
            StartCoroutine(Utils.Utils.InterpolatVolumeVisibility(false,m_postProcessVolume,duration));
    }
}
