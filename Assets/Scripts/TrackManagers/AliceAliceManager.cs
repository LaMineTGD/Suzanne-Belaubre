using System.Collections;
using UnityEngine;

public class AliceAliceManager : ITrackManager
{
    private IEnumerator _changeRateCoroutine;
    private IEnumerator _changeCircleCoroutine;

    protected override void Start()
    {
        base.Start();
        base.ApplyDefaultEffects();
    }

    public void OnPercuStart()
    {
        
    }

    public void OnTududuDebut()
    {
        PulseLineVFX(1.2f);
    }

    public void OnTududu2()
    {
        PulseLineVFX(1.2f);
    }

    public void OnTududu3()
    {
        PulseLineVFX(1.2f);
    }

    public void OnTududu4()
    {
        PulseLineVFX(1.2f);
    }

    public void OnTududu5()
    {
        PulseLineVFX(1.2f);
    }

    public void OnTududu6()
    {
        PulseLineVFX(1.2f);
    }

    public void OnTududuFin()
    {

    }

    public void OnPrologue()
    {
        if(_changeCircleCoroutine != null)
        {
            StopCoroutine(_changeCircleCoroutine);
        }

        Vector2 startCircle = GetLineVFXDefaultCircle();
        Vector2 targetCircle = new(500f, 2f);
        float duration = 15000f;
        _changeCircleCoroutine = ChangeLineVFXCircleCoroutine(startCircle, targetCircle, duration);
        StartCoroutine(_changeCircleCoroutine);
    }

    public void OnRefrainDebut()
    {
        float targetFOV = 150f;
        float lerpDuration = 5;

        ChangeFOVLineVFX(targetFOV, lerpDuration);
    }

    public void OnRefrainFin()
    {
        float targetFOV = 60f;
        float lerpDuration = 8f;

        ChangeFOVLineVFX(targetFOV, lerpDuration);
    }
    
    public void OnChantStart()
    {
        float rotationSpeed = 1f;
        RotateLineVFX(rotationSpeed);
    }
    
    public void OnProphetDebut()
    {
        
    }

    
    public void OnProphetFin()
    {
        
    }

    public void OnFadeOut()
    {
        if(_changeRateCoroutine != null)
        {
            StopCoroutine(_changeRateCoroutine);
        }

        float startRate = GetLineVFXDefaultRate();
        float targetRate = 5000;
        float duration = 5f;
        _changeRateCoroutine = ChangeLineVFXRateCoroutine(startRate, targetRate, duration);
        StartCoroutine(_changeRateCoroutine);
    }
    
    public void OnEnd()
    {
        
    }

    
}

