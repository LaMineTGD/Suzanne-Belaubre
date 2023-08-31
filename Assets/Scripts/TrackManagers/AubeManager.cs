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
        Debug.Log("DeuxiemeVagueHarmonique");
    }
    public void DebutPercussion()
    {
        DebutPercussion(null);
    }
    public void DebutPercussion(OSCMessage message)
    {
        m_VFX.SendEvent("SecondLine");
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
        StartCoroutine(LaunchDawn(lightTransform, endTransform, 30f));
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


    public IEnumerator LaunchDawn(Transform begin, Transform end, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float time = elapsedTime / duration;
            elapsedTime += Time.deltaTime;
            begin.rotation = Quaternion.Lerp(begin.rotation, end.rotation, time);
            yield return null;
        }
        yield return null;
    }
}

