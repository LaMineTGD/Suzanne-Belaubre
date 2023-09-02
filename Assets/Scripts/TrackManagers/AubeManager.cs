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
        base.ApplyDefaultEffects();
        generateOSCReceveier();
    }

    private void generateOSCReceveier()
    {
        ShowManager.m_Instance.OSCReceiver.Bind("/percu_start", DebutPercussion);
        ShowManager.m_Instance.OSCReceiver.Bind("/Chant_start", ChantContreSens);
        ShowManager.m_Instance.OSCReceiver.Bind("/fin_chant", AubeSeLeve);
        ShowManager.m_Instance.OSCReceiver.Bind("/crescendo_f", DebutTransition);
        ShowManager.m_Instance.OSCReceiver.Bind("/debut_deuxieme_harmo", DeuxiemeVagueHarmonique);
    }

    public void DeuxiemeVagueHarmonique()
    {
        DeuxiemeVagueHarmonique(null);
    }
    public void DeuxiemeVagueHarmonique(OSCMessage message)
    {
        m_VFX.SendEvent("FirstLine");
        StartCoroutine(Utils.Utils.InterpolatVfxFloatVisibility(false, "Delay", 3f, m_VFX, 50f, 0.05f));
        Debug.Log("DeuxiemeVagueHarmonique");
    }
    public void DebutPercussion()
    {
        DebutPercussion(null);
    }
    public void DebutPercussion(OSCMessage message)
    {
        m_VFX.SendEvent("SecondLine");
        StartCoroutine(Utils.Utils.InterpolatVfxFloatVisibility(false, "Delay_Side", 3f, m_VFX, 50f, 0.05f));
        Debug.Log("DebutPercussion");
    }
    public void ChantContreSens()
    {
        ChantContreSens(null);
    }
    public void ChantContreSens(OSCMessage message)
    {
        Debug.Log("ChantContreSens");
    }
    public void AubeSeLeve()
    {
        AubeSeLeve(null);
    }
    public void AubeSeLeve(OSCMessage message)
    {
        StartCoroutine(LaunchDawn(lightTransform, endTransform, 45f));
        Debug.Log("AubeSeLeve");
    }
    public void DebutTransition()
    {
        DebutTransition(null);
    }
    public void DebutTransition(OSCMessage message)
    {
        Debug.Log("DebutTransition");
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

