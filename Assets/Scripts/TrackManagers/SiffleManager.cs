using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System.Collections;
using System;
using System.Collections.Generic;

public class SiffleManager : TrackTailorMadeManager
{
    TextureCurveParameter huevsHue;
    TextureCurveParameter hueVsSat;
    Vector2 bound = Vector2.up;
    TextureCurve defaultCurve;
    TextureCurve colorCurve;

    TextureCurve greyCurve;
    [SerializeField] AnimationCurve evolutionCrescendoCurve;
    [SerializeField] float crescendoDuration;
    [SerializeField] float diminuendoDuration;
    [SerializeField] protected VisualEffect m_VFX2;
    [SerializeField] protected VisualEffect m_VFX3;
    [SerializeField] protected VisualEffect m_VFX4;
    [SerializeField] protected VisualEffect m_VFX5;

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

    private void colorCurveCreation()
    {
        Keyframe key1 = new Keyframe(0.2237762f, 0.5f, -1.201681f, -1.201681f);
        Keyframe key2 = new Keyframe(0.5850816f, 0.18f, 0, 0); //240

        Keyframe[] keys = new Keyframe[2];
        keys[0] = key1;
        keys[1] = key2;
        colorCurve = new TextureCurve(keys, 0.5f, true, bound);
    }

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
        if (m_PostProcessVolume.profile.TryGet<ColorCurves>(out ColorCurves colorCurves))
        {
            defaultCurveCreation();

            colorCurveCreation();

            greyCurveCreation();

            huevsHue = colorCurves.hueVsHue;
            hueVsSat = colorCurves.hueVsSat;
            // Debug.Log("inTangent :" + huevshue.value[0].inTangent.ToString());
            // Debug.Log("inWeight :" + huevshue.value[0].inWeight.ToString());
            // Debug.Log("outTangent :" + huevshue.value[0].outTangent.ToString());
            // Debug.Log("outWeight :" + huevshue.value[0].outWeight.ToString());
            // Debug.Log("Time :" + huevshue.value[0].time);
            // Debug.Log("Value :" + huevshue.value[0].value);

            // Debug.Log("inTangent :" + huevshue.value[1].inTangent.ToString());
            // Debug.Log("inWeight :" + huevshue.value[1].inWeight.ToString());
            // Debug.Log("outTangent :" + huevshue.value[1].outTangent.ToString());
            // Debug.Log("outWeight :" + huevshue.value[1].outWeight.ToString());
            // Debug.Log("Time :" + huevshue.value[1].time);
            // Debug.Log("Value :" + huevshue.value[1].value);


            huevsHue.Interp(defaultCurve, colorCurve, 0.0f);
        }
    }

    public void PercuStart()
    {

    }

    public void ChantStart()
    {
        Debug.Log("ChantStart");
        if (chantStartCount == 0)
            m_VFX.SendEvent("Grow");
        else if (chantStartCount == 1)
        {
            m_VFX2.SendEvent("Grow");
            m_VFX3.SendEvent("Grow");
        }
        chantStartCount++;
    }

    public void Crescendo_1()
    {
        Debug.Log("Crescendo_1");
        StartCoroutine(InterpolatHue(defaultCurve, colorCurve, crescendoDuration));
    }

    public void Diminuendo()
    {
        Debug.Log("Diminuendo");
        StartCoroutine(InterpolatSat(defaultCurve, greyCurve, diminuendoDuration));
        StartCoroutine(Utils.Utils.InterpolatVfxFloatVisibility(true, "wind_speed", 50000, m_VFX, diminuendoDuration, 5000));
        StartCoroutine(Utils.Utils.InterpolatVfxFloatVisibility(true, "wind_speed", 50000, m_VFX2, diminuendoDuration, 5000));
        StartCoroutine(Utils.Utils.InterpolatVfxFloatVisibility(true, "wind_speed", 50000, m_VFX3, diminuendoDuration, 5000));
    }

    public void Crescendo_f()
    {
        Debug.Log("Crescendo_f");
        StartCoroutine(InterpolatSat(greyCurve, defaultCurve, 0.5f));
        StartCoroutine(Utils.Utils.InterpolatVfxFloatVisibility(false, "wind_speed", 50000, m_VFX, 1f, 5000));
        StartCoroutine(Utils.Utils.InterpolatVfxFloatVisibility(false, "wind_speed", 50000, m_VFX2, 1f, 5000));
        StartCoroutine(Utils.Utils.InterpolatVfxFloatVisibility(false, "wind_speed", 50000, m_VFX3, 1f, 5000));
        m_VFX4.SendEvent("Grow");
        m_VFX5.SendEvent("Grow");
    }

    public IEnumerator InterpolatHue(TextureCurve begin, TextureCurve end, float duration)
    {
        float elapsedTime = 0f;
        float tmp = 0f;

        while (elapsedTime < duration)
        {
            tmp = elapsedTime / duration;
            float time = evolutionCrescendoCurve.Evaluate(tmp);
            huevsHue.Override(ComputeIntermediateTextureCurve(begin, end, time));
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
}
