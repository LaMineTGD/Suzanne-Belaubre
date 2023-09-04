using System.Collections;
using System.Collections.Generic;
using extOSC;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class LeBruitManager : TrackTailorMadeManager
{
    [SerializeField] AnimationCurve evolutionCrescendoCurve;

    TextureCurveParameter huevsHue;
    TextureCurveParameter hueVsSat;
    TextureCurveParameter master;
    Vector2 bound = Vector2.up;
    TextureCurve defaultCurve;
    TextureCurve defaultMasterCurve;
    TextureCurve lightMasterCurve;
    TextureCurve colorCurveGreen;
    float crescendoDuration = 8f;

    protected override void Start()
    {
        base.Start();
        base.ApplyDefaultEffects();

        // //Change the sky color
        // var currentTrackData = ShowManager.m_Instance.GetCurrentTrack();
        // SetSkyColor(currentTrackData._MainColorList[0],
        //  currentTrackData._MainColorList[1],
        //  currentTrackData._MainColorList[2]);

        if (m_PostProcessVolume.profile.TryGet<ColorCurves>(out ColorCurves colorCurves))
        {
            defaultCurveCreation();

            colorGreenCurveCreation();

            defaultMasterCurveCreation();
            lightMasterCurveCreation();

            huevsHue = colorCurves.hueVsHue;
            hueVsSat = colorCurves.hueVsSat;
            master = colorCurves.master;

            huevsHue.Interp(defaultCurve, colorCurveGreen, 0.0f);
        }

        Crescendo_1();
    }

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

    public void Crescendo_1()
    {
        Crescendo_1(null);
    }

    public void Crescendo_1(OSCMessage message)
    {
        TextureCurve colorCurve =  colorCurveGreen;
        AnimationCurve timeCurve = evolutionCrescendoCurve;
        StartCoroutine(InterpolatWithProgressionCurve(huevsHue, defaultCurve, colorCurve, crescendoDuration, timeCurve));
        StartCoroutine(InterpolatWithProgressionCurve(master, defaultMasterCurve, lightMasterCurve, crescendoDuration, timeCurve));
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
}