using System.Collections;
using UnityEngine;
using extOSC;
public class AubeManager : TrackTailorMadeManager
{
    [SerializeField] private Transform lightTransform;
    [SerializeField] private Transform endTransform;

    protected override void Start()
    {
        base.Start();
        // ShowManager.m_Instance.TransitionAube.AddListener(OnTransitionAube);
        base.ApplyDefaultEffects();
        generateOSCReceveier();
    }

    private void generateOSCReceveier()
    {
        ShowManager.m_Instance.OSCReceiver.Bind("/osc_test", OSCTest);
        ShowManager.m_Instance.OSCReceiver.Bind("/percu_start", DebutPercussion);
        ShowManager.m_Instance.OSCReceiver.Bind("/fin_chant", AubeSeLeve);
        ShowManager.m_Instance.OSCReceiver.Bind("/debut_deuxieme_harmo", DeuxiemeVagueHarmonique);
        ShowManager.m_Instance.OSCReceiver.Bind("/End", OnEnd);
    }

    public void OSCTest()
    {
        OSCTest(null);
    }

    public void OSCTest(OSCMessage message)
    {
        ShowManager.m_Instance.ShowOSCTestUI();
    }

    public void DeuxiemeVagueHarmonique()
    {
        DeuxiemeVagueHarmonique(null);
    }
    public void DeuxiemeVagueHarmonique(OSCMessage message)
    {
        m_VFX.SendEvent("FirstLine");
        StartCoroutine(Utils.Utils.InterpolatVfxFloatVisibility(false, "Delay", 3f, m_VFX, 50f, 0.05f));
    }
    public void DebutPercussion()
    {
        DebutPercussion(null);
    }
    public void DebutPercussion(OSCMessage message)
    {
        m_VFX.SendEvent("SecondLine");
        StartCoroutine(Utils.Utils.InterpolatVfxFloatVisibility(false, "Delay_Side", 3f, m_VFX, 50f, 0.05f));
    }

    public void AubeSeLeve()
    {
        AubeSeLeve(null);
    }
    public void AubeSeLeve(OSCMessage message)
    {
        StartCoroutine(LaunchDawn(lightTransform, endTransform, 45f));
    }

    public void OnEnd()
    {
        OnEnd(null);
    }

    public void OnEnd(OSCMessage message)
    {
        StopCoroutine(LaunchDawn(lightTransform, endTransform, 45f));

        endTransform.Rotate(-90f, 0f, 0f);
        StartCoroutine(LaunchDawn(lightTransform, endTransform, 0.5f));
        Transition();
    }

    public IEnumerator LaunchDawn(Transform begin, Transform target, float duration)
    {
        Debug.Log("Test");
        float elapsedTime = 0f;
        Quaternion start = begin.rotation;
        Quaternion end = target.rotation;
        while (elapsedTime < duration)
        {
            float time = elapsedTime / duration;
            elapsedTime += Time.deltaTime;
            begin.rotation = Quaternion.Lerp(start, end, time);
            yield return null;
        }
        yield return null;
    }
}

