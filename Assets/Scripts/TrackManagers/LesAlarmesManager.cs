using extOSC;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class LesAlarmesManager : ITrackManager
{
    public OSCReceiver m_OSCReceiver;
    public VisualEffect m_VFX;
    public KeyboardManager m_KeyboardManager;

    protected override void Start()
    {
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
}
