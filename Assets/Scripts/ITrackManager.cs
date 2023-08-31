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
    private float baseCameraFOV = 60;
    private bool _isCameraDirty = false;
    private float m_RotationLerpDuration = 1000f;
    private float m_PulseLerpDuration = 0.1f;
    private IEnumerator m_EffilageEffectCoroutine;
    private IEnumerator m_PulseEffectCoroutine;

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
            StartCoroutine(SetLineVFXDefaultValues());
            StartCoroutine(ResetCameraFOVCoroutine(2f));
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

    private IEnumerator SetLineVFXDefaultValues()
    {
        float elapsedTime = 0f;
        float duration = 3f;

        float currentRate = GetLineVFXRate();
        float currentRadius = GetLineVFXRadius();
        Vector2 currentValue1 = GetLineAspectValue1();
        Vector2 currentCircle = GetLineVFXCircle();

        float radius;
        float rate;
        Vector2 value1;
        Vector2 circle;

        while (elapsedTime < duration)
        {
            radius = Mathf.SmoothStep(currentRadius, GetLineVFXDefaultRadius(), elapsedTime / duration);
            rate = Mathf.SmoothStep(currentRate, GetLineVFXDefaultRate(), elapsedTime / duration);
            value1 = Vector2.Lerp(currentValue1, GetLineVFXDefaultValue1(), elapsedTime / duration);
            circle = Vector2.Lerp(currentCircle, GetLineVFXDefaultCircle(), elapsedTime / duration);

            SetLineVFXRadius(radius);
            SetLineVFXRate(rate);
            SetLineVFXAspectValue1(value1);
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

    //Change the color of the Gradient sky depending on a color defined in ShowManager for the track
    protected virtual void SetSkyColor(Color color)
    {
        ShowManager.m_Instance.GetSkyFogManager().SetSkyColor(SkyFogManager.SkyLevel.Middle, color);
    }

    //Set the color of the LineVFX perticles 
    protected virtual void SetLineVFXColor(Color color)
    {
        ShowManager.m_Instance.GetLineVFXManager().SetColorOverLifetime(color);
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

    #region LineVFX getters
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
        return ShowManager.m_Instance.GetLineVFXManager().GetDefaultRate();
    }

    protected float GetLineVFXDefaultRadius()
    {
        return ShowManager.m_Instance.GetLineVFXManager().GetDefaultRadius();
    }

    protected Vector2 GetLineVFXDefaultValue1()
    {
        return ShowManager.m_Instance.GetLineVFXManager().GetLineAspectDefaultValue1();
    }

    protected float GetLineVFXRate()
    {
        return ShowManager.m_Instance.GetLineVFXManager().GetRate();
    }

    protected float GetLineVFXRadius()
    {
        return ShowManager.m_Instance.GetLineVFXManager().GetRadius();
    }

    protected Vector2 GetLineAspectValue1()
    {
        return ShowManager.m_Instance.GetLineVFXManager().GetLineAspectValue1();
    }

    protected Vector2 GetLineVFXCircle()
    {
        return ShowManager.m_Instance.GetLineVFXManager().GetLineAspectCircle();
    }

    protected Vector2 GetLineVFXDefaultCircle()
    {
        return ShowManager.m_Instance.GetLineVFXManager().GetLineAspectDefaultCircle();
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

    private IEnumerator RotationEffectCoroutine(float speed)
    {
        float elapsedTime = 0f;
        Vector2 movingValue1 = GetLineVFXDefaultValue1();
        Vector2 targetValue1 = new (30f, 2f);
        Vector2 currentValue1 = GetLineAspectValue1();
        Vector2 baseValue1 = GetLineVFXDefaultValue1();
        if((currentValue1 - targetValue1).x < Mathf.Epsilon && (currentValue1 - targetValue1).y < Mathf.Epsilon)
        {
            //if traget value was already reached, rotate back to the other side
            targetValue1 = new(2f, 30f);
        }

        while((elapsedTime * speed) < m_RotationLerpDuration)
        {
            movingValue1 = Vector2.Lerp(baseValue1, targetValue1, (elapsedTime * speed) / m_RotationLerpDuration);
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
