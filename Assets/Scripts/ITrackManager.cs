using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class ITrackManager : MonoBehaviour
{
    protected int m_rotationStep = 15;
    protected float m_altitudeLerpDuration = 100f;

    protected IEnumerator m_altitudeCoroutine;
    protected IEnumerator m_cameraFOVCoroutine;
    protected Camera m_MainCamera;
    private float baseCameraFOV = 60f;
    private bool _isCameraDirty = false;
    private float m_RotationLerpDuration = 1000f;
    private float m_PulseLerpDuration = 0.1f;
    private IEnumerator m_EffilageEffectCoroutine;
    private IEnumerator m_PulseEffectCoroutine;

    //Transition variables
    private Volume _activeTailorMadeSky;
    private Volume _activeTailorMadePostProcess;
    private LineVFXManager lineVFXManager;
    private Vector4 _baseTone = new Vector4(-0.046f, 0f, 0f, 100f);

    protected virtual void Start()
    {
        m_MainCamera = Camera.main;
        // DisableVignette();
        
        lineVFXManager = ShowManager.m_Instance.GetLineVFXManager();
        if(ShowManager.m_Instance.GetCurrentTrack()._SceneName == "BonnesDesillusions" 
        || ShowManager.m_Instance.GetCurrentTrack()._SceneName == "Commencement"
        || ShowManager.m_Instance.GetCurrentTrack()._SceneName == "LesAlarmes"
        || ShowManager.m_Instance.GetCurrentTrack()._SceneName == "LeBruit")
        {
            lineVFXManager.StopLineVFX();
        }
        else
        {
            lineVFXManager.PlayLineVFX();
        }

        if(ShowManager.m_Instance.GetCurrentTrack()._SceneName == "Commencement")
        {
            StartCoroutine(ResetCameraFOVCoroutine(10f));
        }

        DisableGrainPP();
    }

    protected void Transition()
    {
        ShowManager.m_Instance.LoadNextTrack();
    }

    protected virtual void ApplyDefaultEffects()
    {
        if(!ShowManager.m_Instance.IsPreviousTrackTailorMade())
        {
            //These methods are called if there is no transition from a TailorMade scene required 
            SetLineVFXColor(GetDefaultLineVFXColor());
        }
    }

        private void Update()
    {
        ResetCameraFOV();
    }

    private void OnDisable()
    {
        DisableGrainPP();
    }

    // private void DisableVignette()
    // {
    //     VolumeProfile vignetteProfile = ShowManager.m_Instance.GetPostProcessVolumeManager()
    //     .GetComponent<Volume>().sharedProfile;

    //     if (vignetteProfile.TryGet<Vignette>(out var vignette))
    //     {
    //         vignette.active = false;
    //     }
    // }

    //Below are all the methods used in the default effects
    #region Default Effects
    protected Camera GetCamera()
    {
        return m_MainCamera;
    } 

    private IEnumerator SetLineVFXDefaultValues()
    {
        float elapsedTime = 0f;
        float duration = 3f;

        float currentRate = GetLineVFXRate();
        float currentRadius = GetLineVFXRadius();
        float currentParticleSpeed = GetLineVFXParticleSpeed();
        Vector2 currentValue1 = GetLineAspectValue1();
        Vector2 currentValue2 = GetLineAspectValue2();
        Vector2 currentCircle = GetLineVFXCircle();

        float radius;
        float rate;
        float particleSpeed;
        Vector2 value1;
        Vector2 value2;
        Vector2 circle;

        while (elapsedTime < duration)
        {
            radius = Mathf.SmoothStep(currentRadius, GetLineVFXDefaultRadius(), elapsedTime / duration);
            rate = Mathf.SmoothStep(currentRate, GetLineVFXDefaultRate(), elapsedTime / duration);
            particleSpeed = Mathf.SmoothStep(currentParticleSpeed, GetLineVFXDefaultParticleSpeed(), elapsedTime / duration);
            value1 = Vector2.Lerp(currentValue1, GetLineVFXDefaultValue1(), elapsedTime / duration);
            value2 = Vector2.Lerp(currentValue2, GetLineVFXDefaultValue2(), elapsedTime / duration);
            circle = Vector2.Lerp(currentCircle, GetLineVFXDefaultCircle(), elapsedTime / duration);

            SetLineVFXRadius(radius);
            SetLineVFXRate(rate);
            SetLineVFXParticleSpeed(particleSpeed);
            SetLineVFXAspectValue1(value1);
            SetLineVFXAspectValue2(value2);
            SetLineVFXAspectCircle(circle);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        yield return null;
    }

    //Reset the Camera FOC to its initial value if the next track is Tailor Made 
    protected virtual void ResetCameraFOV(float duration = 3f)
    {
        if(_isCameraDirty && ShowManager.m_Instance.GetCurrentTrack()._Type == ShowManager.TrackType.TailorMade)
        {
            if (m_cameraFOVCoroutine != null)
            {
                StopCoroutine(m_cameraFOVCoroutine);
            }

            StartCoroutine(ResetCameraFOVCoroutine(duration));
            _isCameraDirty = false;
        }
    }

    protected IEnumerator ResetCameraFOVCoroutine(float duration)
    {
        float elapsedTime = 0f;
        float currentCameraFOV = m_MainCamera.fieldOfView;
        while (elapsedTime < duration)
        {
            m_MainCamera.fieldOfView = Mathf.SmoothStep(currentCameraFOV, baseCameraFOV, elapsedTime / duration);

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
        Vector3 targetVector = Vector3.zero;
        while (elapsedTime < m_altitudeLerpDuration)
        {
            targetVector = new Vector3((float)(ShowManager.m_Instance.GetCurrentTrack()._Altitude * m_rotationStep), 0f, 0f);
            
            Quaternion targetRotation = Quaternion.Euler(targetVector);
            Quaternion initialRotation = m_MainCamera.transform.rotation;
            m_MainCamera.transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, elapsedTime / m_altitudeLerpDuration);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        yield return null;
    }

    //Zoom the camera out accross the different Default tracks, giving an overview of the LineVFX
    protected virtual void ChangeFOVLineVFX(float targetFOV = 100f, float duration = 100f)
    {
        if (m_cameraFOVCoroutine != null)
        {
            StopCoroutine(m_cameraFOVCoroutine);
        }

        m_cameraFOVCoroutine = ZoomLineVFXCoroutine(targetFOV, duration);
        StartCoroutine(m_cameraFOVCoroutine);
        _isCameraDirty = true;
    }

    private IEnumerator ZoomLineVFXCoroutine(float targetFOV, float duration)
    {
        float elapsedTime = 0f;
        float currentCameraFOV = m_MainCamera.fieldOfView;
        while (elapsedTime < duration)
        {
            m_MainCamera.fieldOfView = Mathf.SmoothStep(currentCameraFOV, targetFOV, elapsedTime / duration);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        yield return null;
    }

    //Change the color of the Gradient sky depending on colors defined in ShowManager for the track
    protected virtual void SetSkyColor(Color bottomColor, Color middleColor, Color topColor)
    {
        ShowManager.m_Instance.GetSkyFogManager().SetSkyColor(bottomColor, middleColor, topColor);
    }

    //Set the color of the LineVFX perticles 
    protected virtual void SetLineVFXColor(Color color)
    {
        lineVFXManager.SetColorOverLifetime(color);
    }

    //Adjust the vignette effect depending on the Location parameter in ShowManager
    protected virtual void SetLocation()
    {
        ShowManager.m_Instance.GetPostProcessVolumeManager().SetVignette(ShowManager.m_Instance.GetCurrentTrack()._Location);
    }
    
    //Make the VFX line pulse

    protected void RotateLineVFX(float speed = 1f)
    {
        if (m_EffilageEffectCoroutine != null)
        {
            StopCoroutine(m_EffilageEffectCoroutine);
        }

        m_EffilageEffectCoroutine = RotationEffectCoroutine(speed);
        StartCoroutine(m_EffilageEffectCoroutine);
    }

    protected void EndRotationLineVFX()
    {
        if (m_EffilageEffectCoroutine != null)
        {
            StopCoroutine(m_EffilageEffectCoroutine);
        }
    }

    protected void EnableGrainPP()
    {
        VolumeProfile PPVolumeProfile = ShowManager.m_Instance.GetPostProcessVolumeManager().GetComponent<Volume>().sharedProfile;
        if (PPVolumeProfile.TryGet<FilmGrain>(out var filmGrain))
        {
            filmGrain.active = true;
        }
    }

    protected void DisableGrainPP()
    {
        VolumeProfile PPVolumeProfile = ShowManager.m_Instance.GetPostProcessVolumeManager().GetComponent<Volume>().sharedProfile;
        if (PPVolumeProfile.TryGet<FilmGrain>(out var filmGrain))
        {
            filmGrain.active = false;
        }
    }

    protected IEnumerator DisableGrainPPCoroutine()
    {
        VolumeProfile PPVolumeProfile = ShowManager.m_Instance.GetPostProcessVolumeManager().GetComponent<Volume>().sharedProfile;
        if (PPVolumeProfile.TryGet<FilmGrain>(out var filmGrain))
        {
            filmGrain.intensity.overrideState = true;

            float elapsedTime = 0f;
            float duration = 2f;
            while (elapsedTime < duration)
            {
                filmGrain.intensity.value = Mathf.Lerp(1f, 0f, elapsedTime / duration);

                elapsedTime += Time.deltaTime;

                yield return null;
            }
            filmGrain.active = false;
            yield return null;
        }
    }

    protected void BoostTones()
    {
        VolumeProfile skyVolumeProfile = ShowManager.m_Instance.GetPostProcessVolumeManager().GetComponent<Volume>().sharedProfile;
        if (skyVolumeProfile.TryGet<ShadowsMidtonesHighlights>(out var tones))
        {
            tones.shadows.overrideState = true;
            tones.shadows.value = new Vector4(0.5f, 0f,0f, 100f);
        }
    }

    #endregion

    #region LineVFX setters
    protected void SetLineVFXColorOverLifetime(Gradient gradient)
    {
        lineVFXManager.SetColorOverLifetime(gradient);
    }

    protected void SetLineVFXRate(float rate)
    {
        lineVFXManager.SetRate(rate);
    }

    protected void SetLineVFXRadius(float lineRadius)
    {
        lineVFXManager.SetLineRadius(lineRadius);
    }

    protected void SetLineVFXParticleSpeed(float particleSpeed)
    {
        lineVFXManager.SetParticleSpeed(particleSpeed);
    }

    protected void SetLineVFXLifeTime(Vector2 lifetime)
    {
        lineVFXManager.SetLifeTime(lifetime);
    }

    protected void SetLineVFXAspectValue1(Vector2 value1)
    {
        lineVFXManager.SetLineAspectValue1(value1);
    }

    protected void SetLineVFXAspectValue2(Vector2 value2)
    {
        lineVFXManager.SetLineAspectValue2(value2);

    }

    protected void SetLineVFXAspectCircle(Vector2 circle)
    {
        lineVFXManager.SetLineAspectCircle(circle);
    }

    public void SetLineVFXPosition(Vector3 targetPosition)
    {
        lineVFXManager.SetLineVFXPosition(targetPosition);
    }

    public void HideLineVFX()
    {
        lineVFXManager.StopLineVFX();
    }

    public void ShowLineVFX()
    {
        lineVFXManager.PlayLineVFX();
    }
    #endregion

    #region LineVFX getters
    protected Vector3 GetLineVFXPosition()
    {
        return lineVFXManager.transform.position;
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

    protected float GetLineVFXDefaultRate()
    {
        return lineVFXManager.GetDefaultRate();
    }

    protected float GetLineVFXDefaultRadius()
    {
        return lineVFXManager.GetDefaultRadius();
    }

    protected float GetLineVFXDefaultParticleSpeed()
    {
        return lineVFXManager.GetDefaultParticleSpeed();
    }

    protected Vector2 GetLineVFXDefaultValue1()
    {
        return lineVFXManager.GetLineAspectDefaultValue1();
    }

    protected Vector2 GetLineVFXDefaultValue2()
    {
        return lineVFXManager.GetLineAspectDefaultValue2();
    }

    protected float GetLineVFXRate()
    {
        return lineVFXManager.GetRate();
    }

    protected float GetLineVFXParticleSpeed()
    {
        return lineVFXManager.GetParticleSpeed();
    }

    protected float GetLineVFXRadius()
    {
        return lineVFXManager.GetRadius();
    }

    protected Vector2 GetLineAspectValue1()
    {
        return lineVFXManager.GetLineAspectValue1();
    }

    protected Vector2 GetLineAspectValue2()
    {
        return lineVFXManager.GetLineAspectValue2();
    }

    protected Vector2 GetLineVFXCircle()
    {
        return lineVFXManager.GetLineAspectCircle();
    }

    protected Vector2 GetLineVFXDefaultCircle()
    {
        return lineVFXManager.GetLineAspectDefaultCircle();
    }

    #endregion

    #region LineVFX parameters' coroutines
    protected IEnumerator ChangeLineVFXRateCoroutine(float startRate, float targetRate, float duration)
    {
        float elapsedTime = 0f;
        float rate;
        while (elapsedTime < duration)
        {
            rate = Mathf.SmoothStep(startRate, targetRate, elapsedTime / duration);
            SetLineVFXRate(rate);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        yield return null;
    }

    protected IEnumerator ChangeLineVFXRadiusCoroutine(float startRadius, float targetRadius, float duration)
    {
        float elapsedTime = 0f;
        float radius;
        while (elapsedTime < duration)
        {
            radius = Mathf.SmoothStep(startRadius, targetRadius, elapsedTime / duration);
            SetLineVFXRadius(radius);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        yield return null;
    }

    protected IEnumerator ChangeLineVFXParticleSpeedCoroutine(float startParticleSpeed, float targetParticleSpeed, float duration)
    {
        float elapsedTime = 0f;
        float particleSpeed;
        while (elapsedTime < duration)
        {
            particleSpeed = Mathf.SmoothStep(startParticleSpeed, targetParticleSpeed, elapsedTime / duration);
            SetLineVFXParticleSpeed(particleSpeed);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        yield return null;
    }

    protected IEnumerator ChangeLineVFXCircleCoroutine(Vector2 startCircle, Vector2 targetCircle, float duration)
    {
        float elapsedTime = 0f;
        Vector2 circle;
        while (elapsedTime < duration)
        {
            circle = Vector2.Lerp(startCircle, targetCircle, elapsedTime / duration);
            SetLineVFXAspectCircle(circle);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        yield return null;
    }

    protected IEnumerator ChangeLineVFXValue1Coroutine(Vector2 targetValue1, float speed = 1f)
    {
        float elapsedTime = 0f;
        Vector2 movingValue1;
        Vector2 currentValue1 = GetLineAspectValue1();

        while((elapsedTime * speed) < m_RotationLerpDuration)
        {
            movingValue1 = Vector2.Lerp(currentValue1, targetValue1, (elapsedTime * speed) / m_RotationLerpDuration);
            SetLineVFXAspectValue1(movingValue1);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        yield return null;
    }

    protected IEnumerator ChangeLineVFXValue2Coroutine(Vector2 targetValue2, float speed = 1f)
    {
        float elapsedTime = 0f;
        Vector2 movingValue2;
        Vector2 currentValue2 = GetLineAspectValue2();

        while((elapsedTime * speed) < m_RotationLerpDuration)
        {
            movingValue2 = Vector2.Lerp(currentValue2, targetValue2, (elapsedTime * speed) / m_RotationLerpDuration);
            SetLineVFXAspectValue2(movingValue2);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        yield return null;
    }

    private IEnumerator RotationEffectCoroutine(float speed)
    {
        float elapsedTime = 0f;
        Vector2 movingValue1;
        Vector2 targetValue1 = new (30f, 2f);
        Vector2 currentValue1 = GetLineAspectValue1();
        if((currentValue1 - targetValue1).x < Mathf.Epsilon && (currentValue1 - targetValue1).y < Mathf.Epsilon)
        {
            //if traget value was already reached, rotate back to the other side
            targetValue1 = new(2f, 30f);
        }

        while((elapsedTime * speed) < m_RotationLerpDuration)
        {
            movingValue1 = Vector2.Lerp(currentValue1, targetValue1, (elapsedTime * speed) / m_RotationLerpDuration);
            SetLineVFXAspectValue1(movingValue1);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        yield return null;
    }

    protected void PulseLineVFX(float intensity = 1.5f)
    {
        if (m_PulseEffectCoroutine != null)
        {
            StopCoroutine(m_PulseEffectCoroutine);
            SetLineVFXRadius(GetLineVFXDefaultRadius());
        }

        m_PulseEffectCoroutine = PulseEffectCoroutine(intensity);
        StartCoroutine(m_PulseEffectCoroutine);
    }

    private IEnumerator PulseEffectCoroutine(float intensity)
    {
        float elapsedTime = 0f;
        float movingLineRadius;
        float targetLineRadius = GetLineVFXRadius() * intensity;
        float currentLineRadius = GetLineVFXRadius();

        while(elapsedTime < m_PulseLerpDuration)
        {
            movingLineRadius = Mathf.SmoothStep(currentLineRadius, targetLineRadius, elapsedTime / m_PulseLerpDuration);
            SetLineVFXRadius(movingLineRadius);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        while(elapsedTime > 0)
        {
            //Lerp going backwards
            movingLineRadius = Mathf.SmoothStep(currentLineRadius, targetLineRadius, elapsedTime / m_PulseLerpDuration);
            SetLineVFXRadius(movingLineRadius);

            elapsedTime -= Time.deltaTime;

            yield return null;
        }
        yield return null;
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
        SetLineVFXColor(GetDefaultLineVFXColor());
        TransitionSkyVolume(GetDefaultSkyColor());
        TransitionPostProcess();
    } 

    protected virtual void TransitionSkyVolume(Color color)
    {
        if (_activeTailorMadeSky == null)
        {
            return;
        }

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
        if(_activeTailorMadePostProcess == null)
        {
            return;
        }
        ShowManager.m_Instance.GetPostProcessVolumeManager().TransitionTailorMadePostProcess(_activeTailorMadePostProcess);
    }



    #endregion
}
