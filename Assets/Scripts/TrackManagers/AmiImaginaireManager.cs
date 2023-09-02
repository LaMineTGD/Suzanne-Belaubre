using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using extOSC;
using System.Collections;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;

public class AmiImaginaireManager : TrackTailorMadeManager
{
    [SerializeField] private Color transTopColor;
    [SerializeField] private Color transMidColor;
    [SerializeField] private Color transBotColor;
    [SerializeField] private Volume _gradientSkyVolume;
    [SerializeField] private Volume _postproVolume;

    private const string DROP_SIGNAL = "DropSignalTime"; // Nom de la propriété exposée pour le rayon de la sphère de conformité.

    private const string SDF_RATIO = "SDF_ratio"; // Nom de la propriété exposée pour le rayon de la sphère de conformité.
    private const string Drop = "drop_"; // Nom de la propriété exposée pour le rayon de la sphère de conformité.
    private float start_time = 0;
    private bool isFinal = false;
    private GradientSky _gradientSky;
    private Bloom _bloom;
    private FilmGrain _filmGrain;
    private ColorAdjustments _colorAdj;
    private ColorCurves _colorCurves;

    public void Awake()
    {

        Debug.Log("Awake:" + Time.time);
    }

    protected override void Start()
    {
        Debug.Log("Start:" + Time.time);
        start_time = Time.time;
        generateOSCReceveier();
        base.Start();
        base.ApplyDefaultEffects();

        VolumeProfile _gradientSkyProfile = _gradientSkyVolume.GetComponent<Volume>().sharedProfile;
        
        if(_gradientSkyProfile.TryGet<GradientSky>(out var gradientSky))
        {
            _gradientSky = gradientSky;
        }
        InitPostPro();
    }

    private void generateOSCReceveier()
    {
        ShowManager.m_Instance.OSCReceiver.Bind("/goute_eau", WaterDropSignal);
        ShowManager.m_Instance.OSCReceiver.Bind("/End", End);
        ShowManager.m_Instance.OSCReceiver.Bind("/crescendo_1", Crescendo_1);
        ShowManager.m_Instance.OSCReceiver.Bind("/crescendo_2", Crescendo_2);
        ShowManager.m_Instance.OSCReceiver.Bind("/diminuendo", Diminuendo);
        ShowManager.m_Instance.OSCReceiver.Bind("/crescendo_f", Crescendo_f);
    }

    private void ChangeDropLocation(Vector2 location, int drop)
    {
        string name = Drop + drop.ToString();
        m_VFX.SetVector2(name, location);
    }

    public void WaterDropSignal()
    {
        WaterDropSignal(null);
    }

    public void WaterDropSignal(OSCMessage message)
    {
        float signalTime = Time.time - start_time - 1.0f;
        Debug.Log("WaterDropSignal :" + signalTime);
        m_VFX.SetFloat(DROP_SIGNAL, signalTime);
        for (int i = 1; i <= 3; i++)
        {
            float random_x = Random.Range(-2.2f, 2.2f);
            float random_y = Random.Range(-1.5f, 1.5f);
            ChangeDropLocation(new Vector2(random_x, random_y), i);
        }
    }

    public void Crescendo_1()
    {
        Crescendo_1(null);
    }

    public void Crescendo_1(OSCMessage message)
    {
        Debug.Log("Crescendo_1");
        m_VFX.SetFloat(SDF_RATIO, 0.55f);
    }

    public void Crescendo_2()
    {
        Crescendo_2(null);
    }

    public void Crescendo_2(OSCMessage message)
    {
        Debug.Log("Crescendo_2");
        m_VFX.SetFloat(SDF_RATIO, 0.35f);
    }

    public void Crescendo_f()
    {
        Crescendo_f(null);
    }

    public void Crescendo_f(OSCMessage message)
    {
        Debug.Log("Crescendo_f");
        isFinal = true;
        m_VFX.SetFloat(SDF_RATIO, 0f);
    }

    public void Diminuendo()
    {
        Diminuendo(null);
    }

    public void Diminuendo(OSCMessage message)
    {
        Debug.Log("Diminuendo");
        if (isFinal)
            StartCoroutine(Utils.Utils.InterpolatVfxFloatVisibility(true, SDF_RATIO, 0.9f, m_VFX, 10f));
        else
            m_VFX.SetFloat(SDF_RATIO, 0.9f);
    }

    public void End()
    {
        End(null);
    }

    public void End(OSCMessage message)
    {
        m_VFX.SetFloat(rate_name, 0f);
        StartCoroutine(TransitionPostProcessToSiffle(5f));
        StartCoroutine(TransitionToSiffleSky(5f));
        Debug.Log("End");
    }

    public void OnTransition()
    {
        Transition();
    }

    private IEnumerator TransitionToSiffleSky(float duration)
    {
        float elapsedTime = 0f;

        _gradientSky.skyIntensityMode.overrideState = true;
        _gradientSky.skyIntensityMode.value = SkyIntensityMode.Exposure;

        _gradientSky.updateMode.overrideState = true;
        _gradientSky.updateMode.value = EnvironmentUpdateMode.OnChanged; 

        while(elapsedTime < duration)
        {

            _gradientSky.bottom.overrideState = true;
            Color fromBotColor = _gradientSky.bottom.value;
            _gradientSky.bottom.Interp(fromBotColor, transBotColor, elapsedTime / duration);
                    
            _gradientSky.middle.overrideState = true;
            Color fromMidColor = _gradientSky.middle.value;
            _gradientSky.middle.Interp(fromMidColor, transMidColor, elapsedTime / duration);
                    
            _gradientSky.top.overrideState = true;
            Color fromTopColor = _gradientSky.top.value;
            _gradientSky.top.Interp(fromTopColor, transTopColor, elapsedTime / duration);

            _gradientSky.gradientDiffusion.overrideState = true;
            _gradientSky.gradientDiffusion.value = Mathf.Lerp(1f, 1.32f, duration);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        yield return null;
    }

    private IEnumerator TransitionPostProcessToSiffle(float duration)
    {
        VolumeProfile ppVolumeProfile = _postproVolume.sharedProfile;
        if(ppVolumeProfile.TryGet<Bloom>(out var bloom))
        {
            _bloom = bloom;
        }

        if(ppVolumeProfile.TryGet<FilmGrain>(out var filmGrain))
        {
            _filmGrain = filmGrain;
        }

        if(ppVolumeProfile.TryGet<ColorAdjustments>(out var colorAdj))
        {
            _colorAdj = colorAdj;
        }

        if(ppVolumeProfile.TryGet<ColorCurves>(out var colorCurves))
        {
            _colorCurves = colorCurves;
        }

        _colorCurves.active = true;

        float elapsedTime = 0f;

        while(elapsedTime < duration)
        {
            float time = elapsedTime / duration;
            _colorAdj.postExposure.overrideState = true;
            _colorAdj.postExposure.value = Mathf.Lerp(3f, 1f, time);
            _colorAdj.contrast.overrideState = true;
            _colorAdj.contrast.value = Mathf.Lerp(15f, 30f, time);

            _filmGrain.intensity.overrideState = true;
            _filmGrain.intensity.value = Mathf.Lerp(0.51f, 0.35f, time);
            _filmGrain.response.overrideState = true;
            _filmGrain.response.value = Mathf.Lerp(0.54f, 0.5f, time);

            _bloom.intensity.overrideState = true;
            _bloom.intensity.value = Mathf.Lerp(0.2f, 0.5f, time);
            _bloom.scatter.overrideState = true;
            _bloom.scatter.value = Mathf.Lerp(0.2f, 0.4f, time);
            _bloom.tint.overrideState = true;

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        yield return null;
    }

    private void InitPostPro()
    {
        VolumeProfile ppVolumeProfile = _postproVolume.sharedProfile;
        if(ppVolumeProfile.TryGet<Bloom>(out var bloom))
        {
            _bloom = bloom;
        }

        if(ppVolumeProfile.TryGet<FilmGrain>(out var filmGrain))
        {
            _filmGrain = filmGrain;
        }

        if(ppVolumeProfile.TryGet<ColorAdjustments>(out var colorAdj))
        {
            _colorAdj = colorAdj;
        }

        if(ppVolumeProfile.TryGet<ColorCurves>(out var colorCurves))
        {
            _colorCurves = colorCurves;
        }

        _colorCurves.active = false;

        _colorAdj.postExposure.overrideState = true;
        _colorAdj.postExposure.value = 3f;
        _colorAdj.contrast.overrideState = true;
        _colorAdj.contrast.value = 15f;

        _filmGrain.intensity.overrideState = true;
        _filmGrain.intensity.value = 0.51f;
        _filmGrain.response.overrideState = true;
        _filmGrain.response.value = 0.54f;

        _bloom.intensity.overrideState = true;
        _bloom.intensity.value = 0.2f;
        _bloom.scatter.overrideState = true;
        _bloom.scatter.value = 0.2f;
        _bloom.tint.overrideState = true;
    }



}
