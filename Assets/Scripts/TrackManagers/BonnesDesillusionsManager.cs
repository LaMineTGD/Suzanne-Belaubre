using System.Collections;
using UnityEngine;

public class BonnesDesillusionsManager : ITrackManager
{
    private IEnumerator _crescendoCoroutine;
    private IEnumerator _pulseCoroutine;
    private WaitForSeconds _waitForTempo;
    private float _tempo = 1f;

    private bool _isZoomedOut = false;

    private void Awake()
    {

        _waitForTempo = new(_tempo);
    }

    protected override void Start()
    {
        base.Start();
        base.ApplyDefaultEffects();
    }

    public void OnPercuStart()
    {

    }

    public void OnChantStart()
    {
        float rotationSpeed = 1f;
        RotateLineVFX(rotationSpeed);
    }

    public void OnDebutDuo()
    {

    }

    public void OnDebutNanana()
    {
        if(_pulseCoroutine != null)
        {
            StopCoroutine(_pulseCoroutine);
        }

        _pulseCoroutine = PulseController();
        StartCoroutine(_pulseCoroutine);
    }

    public void OnFinNanana()
    {
        if(_pulseCoroutine != null)
        {
            StopCoroutine(_pulseCoroutine);
        }
    }

    public void OnCorne()
    {
        float targetFOV;
        float lerpDuration;

        if(!_isZoomedOut)
        {
            targetFOV = 75f;
            lerpDuration = 2f;
        }
        else
        {
            targetFOV = 60f;
            lerpDuration = 2f;
        }
            _isZoomedOut = !_isZoomedOut;

            ChangeFOVLineVFX(targetFOV, lerpDuration);
    }

    public void OnCorneRevers()
    {
        OnCorne();
    }

    public void OnFinChant()
    {

    }

    public void OnCrescendoFin()
    {
        if(_crescendoCoroutine != null)
        {
            StopCoroutine(_crescendoCoroutine);
        }

        float startRate = GetLineVFXRate();
        float targetRate = GetLineVFXDefaultRate();
        float duration = 1f;
        _crescendoCoroutine = CrescendoCoroutine(startRate, targetRate, duration);
        StartCoroutine(_crescendoCoroutine);
    }

    public void OnCrescendo1()
    {
        if(_crescendoCoroutine != null)
        {
            StopCoroutine(_crescendoCoroutine);
        }

        float startRate = GetLineVFXDefaultRate();
        float targetRate = 5000;
        float duration = 25f;
        _crescendoCoroutine = CrescendoCoroutine(startRate, targetRate, duration);
        StartCoroutine(_crescendoCoroutine);
    }

    public void OnCrescendo2()
    {
        OnCrescendo1();
    }

    private IEnumerator CrescendoCoroutine(float startRate, float targetRate, float duration)
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

    private IEnumerator PulseController()
    {
        PulseLineVFX();
        yield return _waitForTempo;
        OnDebutNanana();
    }
}



