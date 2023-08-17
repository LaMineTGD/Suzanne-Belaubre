using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class ITrackManager : MonoBehaviour
{
    [SerializeField] protected int m_rotationStep = 30;
    [SerializeField] protected float m_altitudeLerpDuration = 100f;
    [SerializeField] protected float m_FOVLerpDuration = 100f;
    [SerializeField] protected Tracks.TracksEnum m_trackName;

    protected IEnumerator m_altitudeCoroutine;
    protected IEnumerator m_cameraFOVCoroutine;
    protected Camera m_MainCamera;
    private float m_InitFOVLerpDuration = 3f;
    private Coroutine pulsingCoroutine;
    private WaitForSeconds pulseInterval;
    private float baseCameraFOV = 60;
    private float tempo;

    //Transition variables
    private Volume _activeTailorMadeSky;
    private Volume _activeTailorMadePostProcess;
    private Color _defaultSkyColor = Color.black;

    protected virtual void Start()
    {
        tempo = ShowManager.m_Instance.GetCurrentTrack()._Tempo;
        pulseInterval = new WaitForSeconds(tempo);
        m_MainCamera = Camera.main;
    }

    protected virtual void ApplyDefaultEffects()
    {
        if(!ShowManager.m_Instance.IsPreviousTrackTailorMade())
        {
            SetAltitude();
            SetSkyColor();
            SetLineVFXColor();
            SetLocation();
            SetCameraFOV();
        }
        else
        {
            ApplyTransitionEffects();
        }
    }

    protected virtual void ApplyTransitionEffects()
    {
        _activeTailorMadeSky = ShowManager.m_Instance.GetPreviousTailorVolumes()[0];
        _activeTailorMadePostProcess = ShowManager.m_Instance.GetPreviousTailorVolumes()[1];

        SetAltitude();
        SetLocation();
        SetCameraFOV();
        SetLineVFXColor();
        TransitionSkyVolume();
        TransitionPostProcess();
    } 

    private void Update()
    {
        if(ShowManager.m_Instance.GetCurrentTrack()._Type == ShowManager.TrackType.TailorMade)
        {
            if (m_cameraFOVCoroutine != null)
            {
                StopCoroutine(m_cameraFOVCoroutine);
            }

            StartCoroutine(IntializeCameraFOVCoroutine());
        }
    }

    #region Default Effects
    protected virtual void SetAltitude()
    {
        if (m_altitudeCoroutine != null)
        {
            StopCoroutine(m_altitudeCoroutine);
        }

        m_altitudeCoroutine = SetAltitudeCoroutine();
        StartCoroutine(m_altitudeCoroutine);
    }

    private IEnumerator SetAltitudeCoroutine()
    {
        float elapsedTime = 0f;
        while (elapsedTime < m_altitudeLerpDuration)
        {
            var targetVector = new Vector3((float)(ShowManager.m_Instance.GetCurrentTrack()._Altitude * m_rotationStep), 0f, 0f);
            Quaternion targetRotation = Quaternion.Euler(targetVector);
            Quaternion initialRotation = m_MainCamera.transform.rotation;
            m_MainCamera.transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, elapsedTime / m_altitudeLerpDuration);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        yield return null;
    }

    protected virtual void SetCameraFOV()
    {
        if (m_cameraFOVCoroutine != null)
        {
            StopCoroutine(m_cameraFOVCoroutine);
        }

        m_cameraFOVCoroutine = SetCameraFOVCoroutine();
        StartCoroutine(m_cameraFOVCoroutine);
    }

    private IEnumerator SetCameraFOVCoroutine()
    {
        float elapsedTime = 0f;
        float currentCameraFOV = m_MainCamera.fieldOfView;
        while (elapsedTime < m_FOVLerpDuration)
        {
            var targetFOV = 100f;
            m_MainCamera.fieldOfView = Mathf.SmoothStep(currentCameraFOV, targetFOV, elapsedTime / m_FOVLerpDuration);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        yield return null;
    }

    protected IEnumerator IntializeCameraFOVCoroutine()
    {
        float elapsedTime = 0f;
        float currentCameraFOV = m_MainCamera.fieldOfView;
        while (elapsedTime < m_InitFOVLerpDuration)
        {
            m_MainCamera.fieldOfView = Mathf.SmoothStep(currentCameraFOV, baseCameraFOV, elapsedTime / m_InitFOVLerpDuration);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        yield return null;
    }

    protected virtual void SetSkyColor()
    {
        _defaultSkyColor = GetDefaultSkyColor();
        ShowManager.m_Instance.GetSkyFogManager().SetSkyColor(SkyFogManager.SkyLevel.Middle, _defaultSkyColor);
    }

    private Color GetDefaultSkyColor()
    {
        Color color = Color.black;
        if (ShowManager.m_Instance.GetCurrentTrack()._MainColorList != null && ShowManager.m_Instance.GetCurrentTrack()._MainColorList.Count != 0)
        {
            //sets the middle color of the sky to a darker version of the first main color
            color = ShowManager.m_Instance.GetCurrentTrack()._MainColorList[0];

            //reduce intensity by 2
            float intensity = -2f;
            for (int i = 0; i < 3; i++)
            {
                color[i] *= (float)Math.Pow(2f, intensity);
            }
        }

        return color;
    }

    protected virtual void SetLineVFXColor()
    {
        Color color = Color.magenta;
        if (ShowManager.m_Instance.GetCurrentTrack()._MainColorList != null && ShowManager.m_Instance.GetCurrentTrack()._MainColorList.Count > 1)
        {
            color = ShowManager.m_Instance.GetCurrentTrack()._MainColorList[1];
        }
        else if (ShowManager.m_Instance.GetCurrentTrack()._SecondaryColorList != null && ShowManager.m_Instance.GetCurrentTrack()._SecondaryColorList.Count != 0)
        {
            color = ShowManager.m_Instance.GetCurrentTrack()._SecondaryColorList[0];
        }

        ShowManager.m_Instance.GetLineVFXManager().SetColorOverLifetime(color);

    }

    protected virtual void SetLocation()
    {
        ShowManager.m_Instance.GetPostProcessVolumeManager().SetVignette(ShowManager.m_Instance.GetCurrentTrack()._Location);
    }
    
    private IEnumerator PulsingCoroutine()
    {
        ShowManager.m_Instance.GetLineVFXManager().PulseEffect();
        yield return pulseInterval;
        Pulse();
    }

    public void Pulse()
    {
        if(ShowManager.m_Instance.GetCurrentTrack()._Type == ShowManager.TrackType.TailorMade)
        {
            return;
        }

        if(pulsingCoroutine != null)
        {
            StopCoroutine(pulsingCoroutine);
        }

        pulsingCoroutine = StartCoroutine(PulsingCoroutine());
    }

    public void Effilage()
    {
        if(ShowManager.m_Instance.GetCurrentTrack()._Type == ShowManager.TrackType.TailorMade)
        {
            return;
        }

        ShowManager.m_Instance.GetLineVFXManager().EffilageEffect(tempo);
    }
    #endregion

    #region Transition Effects
    protected virtual void TransitionSkyVolume()
    {
        VolumeProfile activeSkyProfile = _activeTailorMadeSky.sharedProfile;
        Color activeSkyColor;
        if (activeSkyProfile.TryGet<VisualEnvironment>(out var activeVisualEnvironment))
        {
            if (activeVisualEnvironment.skyType == (int)SkyType.PhysicallyBased)
            {
                //Active VisualEnvironment is PhysicallyBasedSky
                if (activeSkyProfile.TryGet<PhysicallyBasedSky>(out var activePhysicallyBasedSky))
                {
                    activeSkyColor = (Color)activePhysicallyBasedSky.groundTint;
                    _defaultSkyColor = GetDefaultSkyColor();
                    ShowManager.m_Instance.GetSkyFogManager().TransitionTailorMadeSkyColor(SkyFogManager.SkyLevel.All, activeSkyColor, _defaultSkyColor);
                }
            }
        }
        else
        {
            Debug.LogWarning("There is no VisualEnvironment in the TailorMade scene to transition from");
        }
    }

    protected virtual void TransitionPostProcess()
    {
        ShowManager.m_Instance.GetPostProcessVolumeManager().TransitionTailorMadePostProcess(_activeTailorMadePostProcess);
    }



    #endregion
}
