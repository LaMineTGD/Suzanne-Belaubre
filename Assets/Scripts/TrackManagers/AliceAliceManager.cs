using System.Collections;
using extOSC;
using UnityEngine;

public class AliceAliceManager : ITrackManager
{
    private IEnumerator _changeRateCoroutine;
    private IEnumerator _changeCircleCoroutine;
    private IEnumerator _changeValue2Coroutine;
    private IEnumerator _changeParticleSpeedCoroutine;
    private IEnumerator _changeRadiusCoroutine;
    private IEnumerator _tududuCoroutine;

    private bool _isRefrainStarted = false;

    protected override void Start()
    {
        base.Start();
        base.ApplyDefaultEffects();
        generateOSCReceveier();
    }

    private void generateOSCReceveier()
    {
        ShowManager.m_Instance.OSCReceiver.Bind("/Begin", OnBegin);
        ShowManager.m_Instance.OSCReceiver.Bind("/percu_start", OnPercuStart);
        ShowManager.m_Instance.OSCReceiver.Bind("/Chant_start", OnChantStart);
        ShowManager.m_Instance.OSCReceiver.Bind("/Alice_Prologue", OnPrologue);
        ShowManager.m_Instance.OSCReceiver.Bind("/RefrainDebut", OnRefrainDebut);
        ShowManager.m_Instance.OSCReceiver.Bind("/RefrainFin", OnRefrainFin);
        ShowManager.m_Instance.OSCReceiver.Bind("/Alice_FadeOut", OnFadeOut);
        ShowManager.m_Instance.OSCReceiver.Bind("/Alice_TududuDebut", OnTududuDebut);
        ShowManager.m_Instance.OSCReceiver.Bind("/Alice_TududuFin", OnTududuFin);
        ShowManager.m_Instance.OSCReceiver.Bind("/End", OnEnd);
    }

    public void OnBegin()
    {
        OnBegin(null);
    }

    public void OnBegin(OSCMessage message)
    {
        //Make the line disappear
        HideLineVFX();

        //Change the sky color
        var currentTrackData = ShowManager.m_Instance.GetCurrentTrack();
        SetSkyColor(currentTrackData._MainColorList[0],
         currentTrackData._MainColorList[1],
         currentTrackData._MainColorList[2]);

        //Reduce LineVFX to min circle
        Vector2 startCircle = new Vector2(1f, 2f);
        SetLineVFXAspectCircle(startCircle);
        SetLineVFXRadius(0.05f);
    }

    private void FadeOutLine()
    {
        if(_changeRadiusCoroutine != null)
        {
            StopCoroutine(_changeRadiusCoroutine);
        }

        float startRadius = 0.05f;
        float targetRadius = 0.01f;
        float radiusDuration = 1f;
        _changeRadiusCoroutine = ChangeLineVFXRadiusCoroutine(startRadius, targetRadius, radiusDuration);
        ShowLineVFX();
        StartCoroutine(_changeRadiusCoroutine);
    }

    public void OnPercuStart()
    {
        OnPercuStart(null);
    }

    public void OnPercuStart(OSCMessage message)
    {
        
    }

    public void OnTududuDebut()
    {
        OnTududuDebut(null);
    }

    public void OnTududuDebut(OSCMessage message)
    {
        if(_tududuCoroutine != null)
        {
            StopCoroutine(_tududuCoroutine);
        }

        _tududuCoroutine = TududuCoroutine();
        StartCoroutine(_tududuCoroutine);
    }

    private IEnumerator TududuCoroutine()
    {
        Vector2 startCircle = GetLineVFXCircle();
        Vector2 targetCircle = startCircle + Vector2.up * 0.1f;
        SetLineVFXAspectCircle(targetCircle);

        if(!_isRefrainStarted)
        {
            FadeOutLine();
        }
        yield return new WaitForSeconds(0.2f);

        OnTududuDebut();
    }

    public void OnTududuFin()
    {
        OnTududuFin(null);
    }

    public void OnTududuFin(OSCMessage message)
    {
        if(_tududuCoroutine != null)
        {
            StopCoroutine(_tududuCoroutine);
        }
    }

    public void OnPrologue()
    {
        OnPrologue(null);
    }


    public void OnPrologue(OSCMessage message)
    {

    }

    public void OnRefrainDebut()
    {
        OnRefrainDebut(null);
    }

    public void OnRefrainDebut(OSCMessage message)
    {
        _isRefrainStarted = true;
        float targetFOV = 150f;
        float lerpDuration = 15;

        ChangeFOVLineVFX(targetFOV, lerpDuration);

        if(_changeRadiusCoroutine != null)
        {
            StopCoroutine(_changeRadiusCoroutine);
        }
        float startRadius = GetLineVFXRadius();
        float targetRadius = 0.2f;
        float radiusDuration = 15f;
        _changeRadiusCoroutine = ChangeLineVFXRadiusCoroutine(startRadius, targetRadius, radiusDuration);
        StartCoroutine(_changeRadiusCoroutine);

        if(_changeParticleSpeedCoroutine != null)
        {
            StopCoroutine(_changeParticleSpeedCoroutine);
        }
        float startPartSpeed = GetLineVFXParticleSpeed();
        float targetPartSpeed = 4f;
        float partSpeedDuration = 2f;
        _changeParticleSpeedCoroutine = ChangeLineVFXParticleSpeedCoroutine(startPartSpeed, targetPartSpeed, partSpeedDuration);
        StartCoroutine(_changeParticleSpeedCoroutine);

        if(_changeValue2Coroutine != null)
        {
            StopCoroutine(_changeValue2Coroutine);
        }
        Vector2 startValue2 = GetLineAspectValue2();
        Vector2 targetValue2 = startValue2 + Vector2.up * 4f;
        float value2Duration = 20f;
        _changeValue2Coroutine = ChangeLineVFXValue2Coroutine(targetValue2, value2Duration);
        StartCoroutine(_changeValue2Coroutine);
    }

    public void OnRefrainFin()
    {
        OnRefrainFin(null);
    }


    public void OnRefrainFin(OSCMessage message)
    {
        float targetFOV = 60f;
        float lerpDuration = 8f;

        ChangeFOVLineVFX(targetFOV, lerpDuration);

        if(_changeRadiusCoroutine != null)
        {
            StopCoroutine(_changeRadiusCoroutine);
        }
        float startRadius = GetLineVFXRadius();
        float targetRadius = 0.1f;
        float radiusDuration = 3f;
        _changeRadiusCoroutine = ChangeLineVFXRadiusCoroutine(startRadius, targetRadius, radiusDuration);
        StartCoroutine(_changeRadiusCoroutine);

        if(_changeParticleSpeedCoroutine != null)
        {
            StopCoroutine(_changeParticleSpeedCoroutine);
        }
        float startPartSpeed = GetLineVFXParticleSpeed();
        float targetPartSpeed = 2f;
        float partSpeedDuration = 2f;
        _changeParticleSpeedCoroutine = ChangeLineVFXParticleSpeedCoroutine(startPartSpeed, targetPartSpeed, partSpeedDuration);
        StartCoroutine(_changeParticleSpeedCoroutine);

        if(_changeValue2Coroutine != null)
        {
            StopCoroutine(_changeValue2Coroutine);
        }
    }

    public void OnChantStart()
    {
        OnChantStart(null);
    }
    
    public void OnChantStart(OSCMessage message)
    {
        float rotationSpeed = 1f;
        RotateLineVFX(rotationSpeed);

        if(_changeCircleCoroutine != null)
        {
            StopCoroutine(_changeCircleCoroutine);
        }

        Vector2 startCircle = GetLineVFXCircle();
        Vector2 targetCircle = GetLineVFXDefaultCircle();
        float circleDuration = 300f;
        _changeCircleCoroutine = ChangeLineVFXCircleCoroutine(startCircle, targetCircle, circleDuration);
        StartCoroutine(_changeCircleCoroutine);
    }

    public void OnProphetDebut()
    {
        OnProphetDebut(null);
    }

    
    public void OnProphetDebut(OSCMessage message)
    {
        if(_changeParticleSpeedCoroutine != null)
        {
            StopCoroutine(_changeParticleSpeedCoroutine);
        }
        float startPartSpeed = GetLineVFXParticleSpeed();
        float targetPartSpeed = 8f;
        float partSpeedDuration = 5f;
        _changeParticleSpeedCoroutine = ChangeLineVFXParticleSpeedCoroutine(startPartSpeed, targetPartSpeed, partSpeedDuration);
        StartCoroutine(_changeParticleSpeedCoroutine);

        if(_changeValue2Coroutine != null)
        {
            StopCoroutine(_changeValue2Coroutine);
        }
        Vector2 startValue2 = GetLineAspectValue2();
        Vector2 targetValue2 = startValue2 + Vector2.up * 8f;
        float value2Duration = 20f;
        _changeValue2Coroutine = ChangeLineVFXValue2Coroutine(targetValue2, value2Duration);
        StartCoroutine(_changeValue2Coroutine);
    }

    public void OnProphetFin()
    {
        OnProphetFin(null);
    }
    
    public void OnProphetFin(OSCMessage message)
    {
        if(_changeParticleSpeedCoroutine != null)
        {
            StopCoroutine(_changeParticleSpeedCoroutine);
        }
        float startPartSpeed = GetLineVFXParticleSpeed();
        float targetPartSpeed = 4f;
        float partSpeedDuration = 1f;
        _changeParticleSpeedCoroutine = ChangeLineVFXParticleSpeedCoroutine(startPartSpeed, targetPartSpeed, partSpeedDuration);
        StartCoroutine(_changeParticleSpeedCoroutine);

        if(_changeValue2Coroutine != null)
        {
            StopCoroutine(_changeValue2Coroutine);
        }
    }

    public void OnFadeOut()
    {
        OnFadeOut(null);
    }


    public void OnFadeOut(OSCMessage message)
    {
        float fadeOutDuration = 10f;

        //Reduce emission rate
        if(_changeRateCoroutine != null)
        {
            StopCoroutine(_changeRateCoroutine);
        }

        float startRate = GetLineVFXDefaultRate();
        float targetRate = 5000;
        _changeRateCoroutine = ChangeLineVFXRateCoroutine(startRate, targetRate, fadeOutDuration);
        StartCoroutine(_changeRateCoroutine);

        //Decrease ParticleSpeed to C'est Rien begin value 
        if(_changeParticleSpeedCoroutine != null)
        {
            StopCoroutine(_changeParticleSpeedCoroutine);
        }

        float startParticleSpeed = GetLineVFXParticleSpeed();
        float targetParticleSpeed = 18;
        _changeParticleSpeedCoroutine = ChangeLineVFXParticleSpeedCoroutine(startParticleSpeed, targetParticleSpeed, fadeOutDuration);
        StartCoroutine(_changeParticleSpeedCoroutine);

        //Zoom camera in to C'est Rien start value
        float targetFOV = 15f;
        ChangeFOVLineVFX(targetFOV, fadeOutDuration);
    }
    
    public void OnEnd()
    {
        OnEnd(null);
    }


    public void OnEnd(OSCMessage message)
    {
        Transition();
    }

    
}

