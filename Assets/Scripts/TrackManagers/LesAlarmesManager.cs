using extOSC;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class LesAlarmesManager : ITrackManager
{
    public OSCReceiver m_OSCReceiver;
    public VisualEffect m_VFX;
    public KeyboardManager m_KeyboardManager;
    public GameObject m_SphereContainer;

    protected override void Start()
    {
        StartCoroutine(SphereIn());
        base.Start();
        base.ApplyDefaultEffects();
        generateOSCReceveier();

        EnableGrainPP();

        //Change the sky color
        var currentTrackData = ShowManager.m_Instance.GetCurrentTrack();
        SetSkyColor(currentTrackData._MainColorList[0],currentTrackData._MainColorList[1],currentTrackData._MainColorList[2]);
        //BoostTones();
        m_KeyboardManager.Init();
    }

    private void generateOSCReceveier()
    {
        //ShowManager.m_Instance.OSCReceiver.Bind("/Transition", OnTransition);
        //ShowManager.m_Instance.OSCReceiver.Bind("/End", OnEnd);
    }

    public void OnTransition()
    {
        OnTransition(null);
    }

    public void OnTransition(OSCMessage message)
    {
        StartCoroutine(DisableGrainPPCoroutine());
    }

    public void OnEnd()
    {
        OnEnd(null);
    }


    public void OnEnd(OSCMessage message)
    {
        Transition();
    }

    public void FadeOut()
    {
        m_VFX.SetBool("Emit", false);
        StartCoroutine(SphereOut());
        StartCoroutine(FadeOutCorou());
    }

    private IEnumerator FadeOutCorou()
    {
        float elapsedTime = 0f;
        float duration = 15f;

        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Transition();
        yield return null;
    }

    IEnumerator SphereIn()
    {
        for (float _Scale = 0f; _Scale <= 1f; _Scale += 0.01f)
        {
            m_SphereContainer.transform.localScale = new Vector3(_Scale, _Scale, _Scale);
            yield return new WaitForSeconds(.1f);
        }
    }

    IEnumerator SphereOut()
    {
        for (float _Scale = 1f; _Scale >= 0f; _Scale -= 0.01f)
        {
            m_SphereContainer.transform.localScale = new Vector3(_Scale, _Scale, _Scale);
            yield return new WaitForSeconds(.1f);
        }
        m_SphereContainer.SetActive(false);
    }
}
