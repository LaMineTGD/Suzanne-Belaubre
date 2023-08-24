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
    TextureCurveParameter huevshue;
    TextureCurveParameter hueVsSat;
    Vector2 bound = Vector2.up;
    TextureCurve defaultCurve;
    TextureCurve tryCurve;
    [SerializeField] AnimationCurve evolutionCrescendoCurve;
    [SerializeField] float crescendoDuration;

    private void defaultCurveCreation()
    {
        Vector2 bound = Vector2.up;
        Keyframe key1 = new Keyframe(0.2237762f, 0.5f, 0, 0);
        Keyframe key2 = new Keyframe(0.5850816f, 0.5f, 0, 0);

        Keyframe[] keys = new Keyframe[2];
        keys[0] = key1;
        keys[1] = key2;

        defaultCurve = new TextureCurve(keys, 0.5f, true, bound);
    }

    private void tryCurveCreation()
    {
        Keyframe key1 = new Keyframe(0.2237762f, 0.5f, -1.201681f, -1.201681f);
        Keyframe key2 = new Keyframe(0.5850816f, 0.240f, 0, 0);

        Keyframe[] keys = new Keyframe[2];
        keys[0] = key1;
        keys[1] = key2;
        tryCurve = new TextureCurve(keys, 0.5f, true, bound);
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
        if (m_PostProcessVolume.profile.TryGet<ColorCurves>(out ColorCurves colorCurves))
        {
            defaultCurveCreation();

            tryCurveCreation();

            huevshue = colorCurves.hueVsHue;
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


            huevshue.Interp(defaultCurve, tryCurve, 0.0f);
        }
    }

    public void PercuStart()
    {

    }

    public void ChantStart()
    {
        Debug.Log("ChantStart");
        m_VFX.SendEvent("Grow");
    }

    public void Crescendo_1()
    {
        Debug.Log("Crescendo_1");
        StartCoroutine(InterpolatHue(crescendoDuration));
    }

    public IEnumerator InterpolatHue(float duration)
    {
        float elapsedTime = 0f;
        float tmp = 0f;

        while (elapsedTime < duration)
        {
            tmp = elapsedTime / duration;
            float time = evolutionCrescendoCurve.Evaluate(tmp);
            ComputeIntermediateTextureCurve(defaultCurve, tryCurve, time);
            huevshue.Override(ComputeIntermediateTextureCurve(defaultCurve, tryCurve, time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return null;
    }
}
