using System.Collections;
using UnityEngine;

public class CestRienManager : ITrackManager
{
    private IEnumerator _changeRateCoroutine;
    private IEnumerator _changeRadiusCoroutine;
    private IEnumerator _changeCircleCoroutine;
    private IEnumerator _changeValue2Coroutine;
    private IEnumerator _rotationRefrainCoroutine;
    private IEnumerator _changeParticleSpeedCoroutine;
    private float m_RotationLerpDuration = 1000f;
    private WaitForSeconds _refrainWaitTime;

    private void Awake()
    {
        _refrainWaitTime = new(.3f);
    }

    protected override void Start()
    {
        base.Start();
        base.ApplyDefaultEffects();
    }

    public void OnBegin()
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
        //Zoom FOV out until Outro 
        float targetFOV = 100f;
        float timeToOutro = 200f;
        ChangeFOVLineVFX(targetFOV, timeToOutro);
    }

    public void OnPercuStart()
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

    public void OnEcho()
    {

    }

    public void OnRefrainDebut()
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
        StopCoroutine(_changeValue2Coroutine);
    }

    public void OnRefrainFin()
    {
        StopCoroutine(_rotationRefrainCoroutine);
    }

    public void OnOutro()
    {
        if(_changeRateCoroutine != null)
        {
            StopCoroutine(_changeRateCoroutine);
        }

        float targetFOV = 10f;
        float lerpDuration = 10f;
        ChangeFOVLineVFX(targetFOV, lerpDuration);

        float startRate = GetLineVFXDefaultRate();
        float targetRate = 5000;
        float duration = 10f;
        _changeRateCoroutine = ChangeLineVFXRateCoroutine(startRate, targetRate, duration);
        StartCoroutine(_changeRateCoroutine);
    }

    public void OnEnd()
    {
        HideLineVFX();
        Transition();
    }

    private IEnumerator RefrainCoroutine(Vector2 targetValue1, float speed = 1f)
    {
        float elapsedTime = 0f;
        float timeSincePause = 0f;
        float pauseInterval = 1f;
        Vector2 movingValue1;
        Vector2 currentValue1 = GetLineAspectValue1();

        while((elapsedTime * speed) < m_RotationLerpDuration)
        {
            if(timeSincePause >= pauseInterval)
            {
                timeSincePause = 0f;
                yield return _refrainWaitTime;
            }

            movingValue1 = Vector2.Lerp(currentValue1, targetValue1, (elapsedTime * speed) / m_RotationLerpDuration);
            SetLineVFXAspectValue1(movingValue1);

            elapsedTime += Time.deltaTime;
            timeSincePause += Time.deltaTime;

            yield return null;
        }
        yield return null;
    }

}
