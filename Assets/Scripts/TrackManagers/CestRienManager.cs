using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using extOSC;

public class CestRienManager : ITrackManager
{
    private IEnumerator _changeRateCoroutine;
    private IEnumerator _changeRadiusCoroutine;
    private IEnumerator _changeCircleCoroutine;
    private IEnumerator _changeValue2Coroutine;
    private IEnumerator _rotationRefrainCoroutine;
    private IEnumerator _changeParticleSpeedCoroutine;
    private IEnumerator _changeLineVFXPositionCoroutine;

    private float rotationLerpDuration = 1000f;
    private WaitForSeconds _refrainWaitTime;

    private void Awake()
    {
        _refrainWaitTime = new(.3f);
    }

    protected override void Start()
    {
        base.Start();
        base.ApplyDefaultEffects();

        //Change the sky color
        var currentTrackData = ShowManager.m_Instance.GetCurrentTrack();
        SetSkyColor(currentTrackData._MainColorList[0],
         currentTrackData._MainColorList[1],
         currentTrackData._MainColorList[2]);

        Vector3 startPosition = GetLineVFXPosition();
        Vector3 targetPosition = new Vector3(3f, 0f, 0f);
        float positionDuration = 3f;
        _changeLineVFXPositionCoroutine = ChangeLineVFXPositionCoroutine(startPosition, targetPosition, positionDuration);
        StartCoroutine(_changeLineVFXPositionCoroutine);

        SetLineVFXColor(Color.magenta);
        generateOSCReceveier();

        OnBegin();

        Init();
    }

    private void generateOSCReceveier()
    {
        // ShowManager.m_Instance.OSCReceiver.Bind("/Begin", OnBegin);
        ShowManager.m_Instance.OSCReceiver.Bind("/percu_start", OnPercuStart);
        ShowManager.m_Instance.OSCReceiver.Bind("/Chant_start", OnChantStart);
        ShowManager.m_Instance.OSCReceiver.Bind("/RefrainDebut", OnRefrainDebut);
        ShowManager.m_Instance.OSCReceiver.Bind("/Drill", OnDrill);
        ShowManager.m_Instance.OSCReceiver.Bind("/RefrainFin", OnRefrainFin);
        ShowManager.m_Instance.OSCReceiver.Bind("/TututuDebut", OnTutuDebut);
        ShowManager.m_Instance.OSCReceiver.Bind("/TututuFin", OnTutuFin);
        ShowManager.m_Instance.OSCReceiver.Bind("/Outro", OnOutro);
        ShowManager.m_Instance.OSCReceiver.Bind("/End", OnEnd);
    }

    private void Init()
    {
        // GetCamera().fieldOfView = 15f;
        // SetLineVFXRate(5000f);
        // SetLineVFXParticleSpeed(18f);
        // SetLineVFXAspectValue1(new Vector2(4.558506f, 4.536615f));
        // SetLineVFXAspectValue2(new Vector2(-2.33f, 8.861389f));
        SetLineVFXAspectCircle(new Vector2(1.249678f, 800f)); 
    }

    // private void OnDisable()
    // {
    //     VolumeProfile vignetteProfile = ShowManager.m_Instance.GetPostProcessVolumeManager()
    //     .GetComponent<Volume>().sharedProfile;

    //     if (vignetteProfile.TryGet<Vignette>(out var vignette))
    //     {
    //         vignette.active = false;
    //     }
    // }

    public void OnBegin()
    {
        OnBegin(null);
    }

    public void OnBegin(OSCMessage message)
    {
        float timeToChantStart = 30f;

        //Reduce noise - from AliceAlice final particleSpeed, 
        //get to the base particleSpeed at ChantStart
        if(_changeParticleSpeedCoroutine != null)
        {
            StopCoroutine(_changeParticleSpeedCoroutine);
        }

        float startParticleSpeed = GetLineVFXParticleSpeed();
        float targetParticleSpeed = GetLineVFXDefaultParticleSpeed();
        _changeParticleSpeedCoroutine = ChangeLineVFXParticleSpeedCoroutine(startParticleSpeed, targetParticleSpeed, timeToChantStart);
        StartCoroutine(_changeParticleSpeedCoroutine);
    }

    public void OnChantStart()
    {
        OnChantStart(null);
    }

    public void OnChantStart(OSCMessage message)
    {
        //Zoom FOV out until Outro 
        float targetFOV = 80f;
        float timeToOutro = 200f;
        ChangeFOVLineVFX(targetFOV, timeToOutro);

        if(_changeLineVFXPositionCoroutine != null)
        {
            StopCoroutine(_changeLineVFXPositionCoroutine);
        }

        Vector3 startPosition = GetLineVFXPosition();
        Vector3 targetPosition = new Vector3(3f, -4.5f, 0f);
        float positionDuration = 30f;
        _changeLineVFXPositionCoroutine = ChangeLineVFXPositionCoroutine(startPosition, targetPosition, positionDuration);
        StartCoroutine(_changeLineVFXPositionCoroutine);

        // StartCoroutine(VignetteCoroutine(0.5f));
    }

    public void OnPercuStart()
    {
        OnPercuStart(null);
    }

    public void OnPercuStart(OSCMessage message)
    {
        //Increase LineRadius until RefrainStart
        float timeToRefrainStart = 30f;

        if(_changeRadiusCoroutine != null)
        {
            StopCoroutine(_changeRadiusCoroutine);
        }

        float startRadius = GetLineVFXRadius();
        float targetRadius = .425f;
        _changeRadiusCoroutine = ChangeLineVFXRadiusCoroutine(startRadius, targetRadius, timeToRefrainStart);
        StartCoroutine(_changeRadiusCoroutine);
    }

    public void OnDrill()
    {
        OnDrill(null);
    }

    public void OnDrill(OSCMessage message)
    {
        if(_changeCircleCoroutine != null)
        {
            StopCoroutine(_changeCircleCoroutine);
        }

        Vector2 startCircle = GetLineVFXCircle();
        Vector2 targetCircle = startCircle + Vector2.up * 10f;
        float duration = .5f;
        _changeCircleCoroutine = ChangeLineVFXCircleCoroutine(startCircle, targetCircle, duration);
        StartCoroutine(_changeCircleCoroutine);
    }

    // public void OnEcho()
    // {

    // }

    // public void OnRefrainDebut()
    // {
    //     OnRefrainDebut(null);
    // }

    public void OnRefrainDebut(OSCMessage message)
    {
        if(_rotationRefrainCoroutine != null)
        {
            StopCoroutine(_rotationRefrainCoroutine);
        }

        Vector2 targetValue1 = new(30, 2f);
        float speed = 1f;
        _rotationRefrainCoroutine = RefrainCoroutine(targetValue1, speed);
        StartCoroutine(_rotationRefrainCoroutine);
    }

    public void OnTutuDebut()
    {
        OnTutuDebut(null);
    }

    public void OnTutuDebut(OSCMessage message)
    {
        if(_changeValue2Coroutine != null)
        {
            StopCoroutine(_changeValue2Coroutine);
        }

        Vector2 targetValue2 = new(-10, 10);
        float duration = 10f;
        _changeValue2Coroutine = ChangeLineVFXValue2Coroutine(targetValue2, duration);
        StartCoroutine(_changeValue2Coroutine);
    }

    public void OnTutuFin()
    {
        OnTutuFin(null);
    }

    public void OnTutuFin(OSCMessage message)
    {
        if(_changeValue2Coroutine != null)
            StopCoroutine(_changeValue2Coroutine);
    }

    public void OnRefrainFin()
    {
        OnRefrainFin(null);
    }

    public void OnRefrainFin(OSCMessage message)
    {
        if(_rotationRefrainCoroutine != null)
            StopCoroutine(_rotationRefrainCoroutine);
    }

    public void OnOutro()
    {
        OnOutro(null);
    }

    public void OnOutro(OSCMessage message)
    {
        float targetFOV = 60f;
        float lerpDuration = 20f;
        ChangeFOVLineVFX(targetFOV, lerpDuration);
        
        
        if(_changeRateCoroutine != null)
        {
            StopCoroutine(_changeRateCoroutine);
        }


        float startRate = GetLineVFXDefaultRate();
        float targetRate = 10000;
        float duration = 10f;
        _changeRateCoroutine = ChangeLineVFXRateCoroutine(startRate, targetRate, duration);
        StartCoroutine(_changeRateCoroutine);

        //Decrease ParticleSpeed to C'est Rien begin value 
        if(_changeParticleSpeedCoroutine != null)
        {
            StopCoroutine(_changeParticleSpeedCoroutine);
        }

        float fadeOutDuration = 10f;
        float startParticleSpeed = GetLineVFXParticleSpeed();
        float targetParticleSpeed = 8f;
        _changeParticleSpeedCoroutine = ChangeLineVFXParticleSpeedCoroutine(startParticleSpeed, targetParticleSpeed, fadeOutDuration);
        StartCoroutine(_changeParticleSpeedCoroutine);

        // StartCoroutine(VignetteCoroutine(0.678f, 0.5f, 10f));
    }

    public void OnEnd()
    {
        OnEnd(null);
    }

    public void OnEnd(OSCMessage message)
    {
        HideLineVFX();
        // VolumeProfile vignetteProfile = ShowManager.m_Instance.GetPostProcessVolumeManager()
        // .GetComponent<Volume>().sharedProfile;

        // if (vignetteProfile.TryGet<Vignette>(out var vignette))
        // {
        //     vignette.active = false;
        // }
        Transition();
    }

    private IEnumerator RefrainCoroutine(Vector2 targetValue1, float speed = 1f)
    {
        float elapsedTime = 0f;
        float timeSincePause = 0f;
        float pauseInterval = 1f;
        Vector2 movingValue1;
        Vector2 currentValue1 = GetLineAspectValue1();

        while((elapsedTime * speed) < rotationLerpDuration)
        {
            if(timeSincePause >= pauseInterval)
            {
                timeSincePause = 0f;
                yield return _refrainWaitTime;
            }

            movingValue1 = Vector2.Lerp(currentValue1, targetValue1, (elapsedTime * speed) / rotationLerpDuration);
            SetLineVFXAspectValue1(movingValue1);

            elapsedTime += Time.deltaTime;
            timeSincePause += Time.deltaTime;

            yield return null;
        }
        yield return null;
    }

    private IEnumerator ChangeLineVFXPositionCoroutine(Vector3 startPosition, Vector3 targetPosition, float duration)
    {
        float elapsedTime = 0f;
        Vector3 position;
        while (elapsedTime < duration)
        {
            position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            SetLineVFXPosition(position);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        yield return null;
    }

    // private IEnumerator VignetteCoroutine(float startIntensity, float targetIntensity = 0.678f, float duration = 20f)
    // {
    //     VolumeProfile vignetteProfile = ShowManager.m_Instance.GetPostProcessVolumeManager()
    //     .GetComponent<Volume>().sharedProfile;

    //     if(vignetteProfile.TryGet<Vignette>(out var vignette))
    //     {
    //         vignette.intensity.overrideState = true;
    //         vignette.intensity.value = 0f;
    //         vignette.active = true;

    //         float elpasedTime = 0f;

    //         while(elpasedTime < duration)
    //         {
    //             vignette.intensity.value = Mathf.Lerp(startIntensity, targetIntensity, elpasedTime / duration);

    //             elpasedTime += Time.deltaTime;
    //             yield return null;
    //         }
    //         yield return null;
    //     }
    // }

}
