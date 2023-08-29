using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class ITrackManager : MonoBehaviour
{
    protected int m_rotationStep = 15;
    protected float m_altitudeLerpDuration = 100f;
    protected float m_FOVLerpDuration = 100f;

    protected IEnumerator m_altitudeCoroutine;
    protected IEnumerator m_cameraFOVCoroutine;
    protected Camera m_MainCamera;
    private float m_InitFOVLerpDuration = 3f;
    private float baseCameraFOV = 60;
    private bool _isCameraDirty = false;

    //Transition variables
    private Volume _activeTailorMadeSky;
    private Volume _activeTailorMadePostProcess;

    protected virtual void Start()
    {
        m_MainCamera = Camera.main;
    }

    protected virtual void ApplyDefaultEffects()
    {
        if(!ShowManager.m_Instance.IsPreviousTrackTailorMade())
        {
            //These methods are called if there is no transition from a TailorMade scene required 
            SetAltitude();
            SetSkyColor(GetDefaultSkyColor());
            SetLineVFXColor(GetDefaultLineVFXColor());
            SetLocation();
            ZoomOutLineVFX();
        }
        else
        {
            //This method is called if the previous scene is a TailorMade script
            ApplyTransitionEffects();
        }
    }

        private void Update()
    {
        ResetCameraFOV();
    }

    //Below are all the methods used in the default effects
    #region Default Effects

    //Reset the Camera FOC to its initial value if the next track is Tailor Made 
    protected virtual void ResetCameraFOV()
    {
        if(_isCameraDirty && ShowManager.m_Instance.GetCurrentTrack()._Type == ShowManager.TrackType.TailorMade)
        {
            if (m_cameraFOVCoroutine != null)
            {
                StopCoroutine(m_cameraFOVCoroutine);
            }

            StartCoroutine(ResetCameraFOVCoroutine());
            _isCameraDirty = false;
        }
    }

    protected IEnumerator ResetCameraFOVCoroutine()
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

    //Rotate the camera up or down depending on the altitude parameter in ShowManager
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

    //Zoom the camera out accross the different Default tracks, giving an overview of the LineVFX
    protected virtual void ZoomOutLineVFX()
    {
        if (m_cameraFOVCoroutine != null)
        {
            StopCoroutine(m_cameraFOVCoroutine);
        }

        m_cameraFOVCoroutine = ZoomOutLineVFXCoroutine();
        StartCoroutine(m_cameraFOVCoroutine);
        _isCameraDirty = true;
    }

    private IEnumerator ZoomOutLineVFXCoroutine()
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

    //Change the color of the Gradient sky depending on a color defined in ShowManager for the track
    protected virtual void SetSkyColor(Color color)
    {
        ShowManager.m_Instance.GetSkyFogManager().SetSkyColor(SkyFogManager.SkyLevel.Middle, color);
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

    //Set the color of the LineVFX perticles 
    protected virtual void SetLineVFXColor(Color color)
    {
        ShowManager.m_Instance.GetLineVFXManager().SetColorOverLifetime(color);
    }

    private Color GetDefaultLineVFXColor()
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

        return color;
    }

    //Adjust the vignette effect depending on the Location parameter in ShowManager
    protected virtual void SetLocation()
    {
        ShowManager.m_Instance.GetPostProcessVolumeManager().SetVignette(ShowManager.m_Instance.GetCurrentTrack()._Location);
    }
    
    //Make the VFX line pulse
    public void PulseLineVFX()
    {
        ShowManager.m_Instance.GetLineVFXManager().PulseEffect();
    }

    //Make the Line VFX rotate at a given speed
    public void RotateLineVFX(float effilageSpeed)
    {
        ShowManager.m_Instance.GetLineVFXManager().EffilageEffect(effilageSpeed);
    }

    #endregion

    #region LineVFX setters
    protected void SetLineVFXColorOverLifetime(Gradient gradient)
    {
        ShowManager.m_Instance.GetLineVFXManager().SetColorOverLifetime(gradient);
    }

    protected void SetLineVFXRate(float rate)
    {
        ShowManager.m_Instance.GetLineVFXManager().SetRate(rate);
    }

    protected void SetLineVFXRadius(float lineRadius)
    {
        ShowManager.m_Instance.GetLineVFXManager().SetLineRadius(lineRadius);
    }

    protected void SetLineVFXParticleSpeed(float particleSpeed)
    {
        ShowManager.m_Instance.GetLineVFXManager().SetParticleSpeed(particleSpeed);
    }

    protected void SetLineVFXLifeTime(Vector2 lifetime)
    {
        ShowManager.m_Instance.GetLineVFXManager().SetLifeTime(lifetime);
    }

    protected void SetLineVFXAspectValue1(Vector2 value1)
    {
        ShowManager.m_Instance.GetLineVFXManager().SetLineAspectValue1(value1);
    }

    protected void SetLineVFXAspectValue2(Vector2 value2)
    {
        ShowManager.m_Instance.GetLineVFXManager().SetLineAspectValue2(value2);

    }

    protected void SetLineVFXAspectCircle(Vector2 circle)
    {
        ShowManager.m_Instance.GetLineVFXManager().SetLineAspectCircle(circle);
    }
    #endregion

    #region Transition Effects

    protected virtual void ApplyTransitionEffects()
    {
        Debug.LogWarning("Transitioning from a TailorMade scene to a default scene");
        _activeTailorMadeSky = ShowManager.m_Instance.GetPreviousTailorVolumes()[0];
        _activeTailorMadePostProcess = ShowManager.m_Instance.GetPreviousTailorVolumes()[1];

        SetAltitude();
        SetLocation();
        ZoomOutLineVFX();
        SetLineVFXColor(GetDefaultLineVFXColor());
        TransitionSkyVolume(GetDefaultSkyColor());
        TransitionPostProcess();
    } 

    protected virtual void TransitionSkyVolume(Color color)
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
                    ShowManager.m_Instance.GetSkyFogManager().TransitionTailorMadeSkyColor(SkyFogManager.SkyLevel.All, activeSkyColor, color);
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
