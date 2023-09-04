using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System.Collections;
using System.Collections.Generic;
using extOSC;

public class SiffleManager : TrackTailorMadeManager
{
    TextureCurveParameter huevsHue;
    TextureCurveParameter hueVsSat;
    TextureCurveParameter master;
    Vector2 bound = Vector2.up;

    bool debutDeuxiemeHarmonique = false;

    TextureCurve defaultCurve;
    TextureCurve defaultMasterCurve;
    TextureCurve lightMasterCurve;
    TextureCurve colorCurveGreen;
    TextureCurve colorCurveOrange;
    TextureCurve greyCurve;
    [SerializeField] AnimationCurve evolutionCrescendoCurve;
    [SerializeField] AnimationCurve evolution2CrescendoCurve;
    [SerializeField] AnimationCurve transitionCurve;
    [SerializeField] float crescendoDuration;
    [SerializeField] float treeMaxRotationSpeed;

    [SerializeField] float diminuendoDuration;
    [SerializeField] protected VisualEffect m_Three2;
    [SerializeField] protected VisualEffect m_Three3;
    [SerializeField] protected VisualEffect m_Three4;
    [SerializeField] protected VisualEffect m_Three5;

    [SerializeField] protected VisualEffect m_Wind;
    [SerializeField] protected VisualEffect m_Wind2;
    [SerializeField] protected VisualEffect m_Wind3;
    int chantStartCount;

    private void defaultCurveCreation()
    {
        Vector2 bound = Vector2.up;
        Keyframe key1 = new Keyframe(0.2237762f, 0.5f);
        Keyframe key2 = new Keyframe(0.5850816f, 0.5f);
        Keyframe key3 = new Keyframe(0.609f, 0.5f);
        Keyframe key4 = new Keyframe(0.622f, 0.5f);
        Keyframe[] keys = new Keyframe[4];
        keys[0] = key1;
        keys[1] = key2;
        keys[2] = key3;
        keys[3] = key4;

        defaultCurve = new TextureCurve(keys, 0.5f, true, bound);
    }

    private void defaultMasterCurveCreation()
    {
        Vector2 bound = Vector2.up;
        Keyframe key1 = new Keyframe(0f, 0f, 0f, 1f);
        Keyframe key2 = new Keyframe(0.85f, 0.85f, 1f, 1f);
        Keyframe key3 = new Keyframe(1f, 1f, 1f, 0f);
        Keyframe[] keys = new Keyframe[3];
        keys[0] = key1;
        keys[1] = key2;
        keys[2] = key3;

        defaultMasterCurve = new TextureCurve(keys, 0.5f, false, bound);
    }

    private void lightMasterCurveCreation()
    {
        Vector2 bound = Vector2.up;
        Keyframe key1 = new Keyframe(0f, 0f, 0f, 1f);
        Keyframe key2 = new Keyframe(0.85f, 1f, 0f, 0f);
        Keyframe key3 = new Keyframe(1f, 1f, 0f, 0f);
        Keyframe[] keys = new Keyframe[3];
        keys[0] = key1;
        keys[1] = key2;
        keys[2] = key3;

        lightMasterCurve = new TextureCurve(keys, 0.5f, false, bound);
    }

    private void colorGreenCurveCreation()
    {
        Keyframe key1 = new Keyframe(0.2237762f, 0.5f, -1.201681f, -1.201681f);
        Keyframe key2 = new Keyframe(0.5850816f, 0.24f, 0, 0); //240

        Keyframe[] keys = new Keyframe[2];
        keys[0] = key1;
        keys[1] = key2;
        colorCurveGreen = new TextureCurve(keys, 0.5f, true, bound);
    }

    private void colorPurpleCurveCreation()
    {
        Keyframe key1 = new Keyframe(0.191f, 0.8756f, 0f, 0f);
        Keyframe key2 = new Keyframe(0.4033019f, 0.7761194f, -0.9897181f, -0.9897181f);
        Keyframe key3 = new Keyframe(0.6603774f, 0.5472637f, -0.7670738f, 0.7670738f); //240

        Keyframe[] keys = new Keyframe[3];
        keys[0] = key1;
        keys[1] = key2;
        keys[2] = key3;
        colorCurveOrange = new TextureCurve(keys, 0.5f, true, bound);
    }

    // private void colorOrangeCurveCreation()
    // {
    //     Keyframe key1 = new Keyframe(0.2237762f, 0.5f, 0.05145023f, 0.05145023f);
    //     Keyframe key2 = new Keyframe(0.5850816f, 0.925f, 0, 0); //240

    //     Keyframe[] keys = new Keyframe[2];
    //     keys[0] = key1;
    //     keys[1] = key2;
    //     colorCurveOrange = new TextureCurve(keys, 0.5f, true, bound);
    // }

    private void greyCurveCreation()
    {
        Keyframe key1 = new Keyframe(0.29f, 0.0f);
        Keyframe key2 = new Keyframe(0.313f, 0.5f);
        Keyframe key3 = new Keyframe(0.595f, 0.5f);
        Keyframe key4 = new Keyframe(0.600f, 0.0f);

        Keyframe[] keys = new Keyframe[4];
        keys[0] = key1;
        keys[1] = key2;
        keys[2] = key3;
        keys[3] = key4;
        greyCurve = new TextureCurve(keys, 0.5f, true, bound);
    }

    private TextureCurve ComputeIntermediateTextureCurve(TextureCurve begin, TextureCurve end, float time)
    {
        List<Keyframe> keyframes = new List<Keyframe>();
        int i = 0;
        int j = 0;
        float tmpTime;
        float tmpValue;
        Keyframe tmpKeyframe;
        while (i < begin.length && j < end.length)
        {
            if (j >= end.length || begin[i].time < end[j].time)
            {
                tmpTime = begin[i].time;
                tmpValue = end.Evaluate(tmpTime);
                tmpKeyframe = new Keyframe(tmpTime, Mathf.Lerp(begin[i].value, tmpValue, time));
                i++;
            }
            else if (i >= begin.length || begin[i].time > end[j].time)
            {
                tmpTime = end[j].time;
                tmpValue = begin.Evaluate(tmpTime);
                tmpKeyframe = new Keyframe(tmpTime, Mathf.Lerp(tmpValue, end[j].value, time));
                j++;
            }
            else
            {
                float value = Mathf.Lerp(begin[i].value, end[j].value, time);
                float inTangent = Mathf.Lerp(begin[i].inTangent, end[j].inTangent, time);
                float outTangent = Mathf.Lerp(begin[i].outTangent, end[j].outTangent, time);
                tmpKeyframe = new Keyframe(begin[i].time, value, inTangent, outTangent);
                i++;
                j++;
            }
            keyframes.Add(tmpKeyframe);
        }
        return new TextureCurve(keyframes.ToArray(), 0.5f, true, bound); ;
    }

    protected override void Start()
    {
        base.Start();
        base.ApplyDefaultEffects();
        chantStartCount = 0;
        generateOSCReceveier();
        if (m_PostProcessVolume.profile.TryGet<ColorCurves>(out ColorCurves colorCurves))
        {
            defaultCurveCreation();

            colorGreenCurveCreation();
            colorPurpleCurveCreation();

            greyCurveCreation();

            defaultMasterCurveCreation();
            lightMasterCurveCreation();

            huevsHue = colorCurves.hueVsHue;
            hueVsSat = colorCurves.hueVsSat;
            master = colorCurves.master;

            // Debug.Log("inTangent :" + huevsHue.value[0].inTangent.ToString());
            // Debug.Log("inWeight :" + huevsHue.value[0].inWeight.ToString());
            // Debug.Log("outTangent :" + huevsHue.value[0].outTangent.ToString());
            // Debug.Log("outWeight :" + huevsHue.value[0].outWeight.ToString());
            // Debug.Log("Time :" + huevsHue.value[0].time);
            // Debug.Log("Value :" + huevsHue.value[0].value);

            // Debug.Log("inTangent :" + huevsHue.value[1].inTangent.ToString());
            // Debug.Log("inWeight :" + huevsHue.value[1].inWeight.ToString());
            // Debug.Log("outTangent :" + huevsHue.value[1].outTangent.ToString());
            // Debug.Log("outWeight :" + huevsHue.value[1].outWeight.ToString());
            // Debug.Log("Time :" + huevsHue.value[1].time);
            // Debug.Log("Value :" + huevsHue.value[1].value);

            // Debug.Log("inTangent :" + huevsHue.value[2].inTangent.ToString());
            // Debug.Log("inWeight :" + huevsHue.value[2].inWeight.ToString());
            // Debug.Log("outTangent :" + huevsHue.value[2].outTangent.ToString());
            // Debug.Log("outWeight :" + huevsHue.value[2].outWeight.ToString());
            // Debug.Log("Time :" + huevsHue.value[2].time);
            // Debug.Log("Value :" + huevsHue.value[2].value);

            huevsHue.Interp(defaultCurve, colorCurveGreen, 0.0f);
        }
    }

    private void generateOSCReceveier()
    {
        ShowManager.m_Instance.OSCReceiver.Bind("/percu_start", PercuStart);
        ShowManager.m_Instance.OSCReceiver.Bind("/Chant_start", ChantStart);
        ShowManager.m_Instance.OSCReceiver.Bind("/crescendo_1", Crescendo_1);
        ShowManager.m_Instance.OSCReceiver.Bind("/diminuendo", Diminuendo);
        ShowManager.m_Instance.OSCReceiver.Bind("/crescendo_f", Crescendo_f);
        ShowManager.m_Instance.OSCReceiver.Bind("/debut_deuxieme_harmo", DebutDeuxiemeHarmonique);
        ShowManager.m_Instance.OSCReceiver.Bind("/fin_chant", FinChant);
        ShowManager.m_Instance.OSCReceiver.Bind("/fin_chant_2", FinChant2);
        ShowManager.m_Instance.OSCReceiver.Bind("/Transition", OnTransitionStart);
        ShowManager.m_Instance.OSCReceiver.Bind("/End", OnEnd);
    }

    public void PercuStart()
    {
        PercuStart(null);
    }

    public void PercuStart(OSCMessage message)
    {
        StartCoroutine(LaunchWind(m_Wind, 5.0f));
    }


    public void ChantStart()
    {
        ChantStart(null);
    }

    public void ChantStart(OSCMessage message)
    {
        Debug.Log("ChantStart");
        if (chantStartCount == 0)
            m_VFX.SendEvent("Grow");
        else if (chantStartCount == 1)
        {
            m_Three2.SendEvent("Grow");
            m_Three3.SendEvent("Grow");
        }
        chantStartCount++;
    }


    public void Crescendo_1()
    {
        Crescendo_1(null);
    }

    public void Crescendo_1(OSCMessage message)
    {
        Debug.Log("Crescendo_1");

        TextureCurve colorCurve = debutDeuxiemeHarmonique ? colorCurveOrange : colorCurveGreen;
        AnimationCurve timeCurve = debutDeuxiemeHarmonique ? evolution2CrescendoCurve : evolutionCrescendoCurve;
        StartCoroutine(InterpolatWithProgressionCurve(huevsHue, defaultCurve, colorCurve, crescendoDuration, timeCurve));
        StartCoroutine(InterpolatWithProgressionCurve(master, defaultMasterCurve, lightMasterCurve, crescendoDuration, timeCurve));
        StartCoroutine(InterpolatWithProgressionCurve(m_VFX, 5000, treeMaxRotationSpeed, crescendoDuration));
        StartCoroutine(InterpolatWithProgressionCurve(m_Three2, 5000, treeMaxRotationSpeed, crescendoDuration));
        StartCoroutine(InterpolatWithProgressionCurve(m_Three3, 5000, treeMaxRotationSpeed, crescendoDuration));
        StartCoroutine(InterpolatWithProgressionCurve(m_Three4, 5000, treeMaxRotationSpeed, crescendoDuration));
        StartCoroutine(InterpolatWithProgressionCurve(m_Three5, 5000, treeMaxRotationSpeed, crescendoDuration));
    }
    public void Diminuendo()
    {
        Diminuendo(null);
    }
    public void Diminuendo(OSCMessage message)
    {
        Debug.Log("Diminuendo");
        StartCoroutine(InterpolatSat(defaultCurve, greyCurve, diminuendoDuration));
        StartCoroutine(InterpolatWithProgressionCurve(master, defaultMasterCurve, lightMasterCurve, crescendoDuration, evolutionCrescendoCurve));
        StartCoroutine(Utils.Utils.InterpolatVfxFloatVisibility(true, "wind_speed", 100000, m_VFX, diminuendoDuration, 5000));
        StartCoroutine(Utils.Utils.InterpolatVfxFloatVisibility(true, "wind_speed", 100000, m_Three2, diminuendoDuration, 5000));
        StartCoroutine(Utils.Utils.InterpolatVfxFloatVisibility(true, "wind_speed", 100000, m_Three3, diminuendoDuration, 5000));
    }

    public void Crescendo_f()
    {
        Crescendo_f(null);
    }

    public void Crescendo_f(OSCMessage message)
    {
        Debug.Log("Crescendo_f");
        StartCoroutine(InterpolatSat(greyCurve, defaultCurve, 0.5f));
        StartCoroutine(Utils.Utils.InterpolatVfxFloatVisibility(false, "wind_speed", 100000, m_VFX, 1f, 5000));
        StartCoroutine(Utils.Utils.InterpolatVfxFloatVisibility(false, "wind_speed", 100000, m_Three2, 1f, 5000));
        StartCoroutine(Utils.Utils.InterpolatVfxFloatVisibility(false, "wind_speed", 100000, m_Three3, 1f, 5000));
        m_Three4.SendEvent("Grow");
        m_Three5.SendEvent("Grow");
    }

    public void DebutDeuxiemeHarmonique()
    {
        DebutDeuxiemeHarmonique(null);
    }

    public void DebutDeuxiemeHarmonique(OSCMessage message)
    {
        debutDeuxiemeHarmonique = !debutDeuxiemeHarmonique;
    }

    public void FinChant()
    {
        FinChant(null);
    }

    public void FinChant(OSCMessage message)
    {
        StartCoroutine(LaunchWind(m_Wind2, 5.0f));
    }

    public void FinChant2()
    {
        FinChant2(null);
    }

    public void FinChant2(OSCMessage message)
    {
        StartCoroutine(LaunchWind(m_Wind3, 5.0f));
    }

    public void OnTransitionStart()
    {
        OnTransitionStart(null);
    }

    public void OnTransitionStart(OSCMessage message)
    {
        TransitionColor();
    }

    public void OnEnd()
    {
        OnEnd(null);
    }

    public IEnumerator InterpolatWithProgressionCurve(TextureCurveParameter volume, TextureCurve begin, TextureCurve end, float duration, AnimationCurve timeCurve)
    {
        float elapsedTime = 0f;
        float tmp = 0f;

        while (elapsedTime < duration)
        {
            tmp = elapsedTime / duration;
            float time = evolutionCrescendoCurve.Evaluate(tmp);
            volume.Override(ComputeIntermediateTextureCurve(begin, end, time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return null;
    }

    public IEnumerator InterpolatWithProgressionCurve(VisualEffect visualEffect, float begin, float end, float duration)
    {
        float elapsedTime = 0f;
        float tmp = 0f;

        while (elapsedTime < duration)
        {
            tmp = elapsedTime / duration;
            float time = evolutionCrescendoCurve.Evaluate(tmp);
            float res = time * (end - begin) + begin;
            visualEffect.SetFloat("wind_speed", res);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return null;
    }

    public IEnumerator InterpolatSat(TextureCurve begin, TextureCurve end, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float time = elapsedTime / duration;
            hueVsSat.Override(ComputeIntermediateTextureCurve(begin, end, time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return null;
    }

    public IEnumerator LaunchWind(VisualEffect begin, float duration)
    {
        float elapsedTime = 0f;
        begin.SendEvent("OnWind");
        while (elapsedTime < duration)
        {
            float time = elapsedTime / duration;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        begin.SendEvent("StopWind");
        yield return null;
    }

    public void TransitionColor()
    {
        TextureCurve colorCurve =  colorCurveGreen;
        AnimationCurve timeCurve = transitionCurve;
        StartCoroutine(InterpolatWithProgressionCurve(huevsHue, defaultCurve, colorCurve, crescendoDuration, timeCurve));
        StartCoroutine(InterpolatWithProgressionCurve(master, defaultMasterCurve, lightMasterCurve, crescendoDuration, timeCurve));
    }

    public void OnEnd(OSCMessage message)
    {
        Transition();
    }
}
