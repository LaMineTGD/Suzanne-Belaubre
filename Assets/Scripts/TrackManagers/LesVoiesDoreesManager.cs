using System.Collections;
using extOSC;
using UnityEngine;

public class LesVoiesDoreesManager : TrackTailorMadeManager
{
    [SerializeField] private Material _backgroundMaterial;

    protected override void Start()
    {
        base.Start();
        base.ApplyDefaultEffects();
         //Change the sky color
        var currentTrackData = ShowManager.m_Instance.GetCurrentTrack();
        SetSkyColor(currentTrackData._MainColorList[0], currentTrackData._MainColorList[1], currentTrackData._MainColorList[2]);
        generateOSCReceveier();
        _backgroundMaterial.SetFloat("_ColorMultiplier", 1f);
    }

    private void generateOSCReceveier()
    {
        ShowManager.m_Instance.OSCReceiver.Bind("/FadeOut", ToBlack);
        // ShowManager.m_Instance.OSCReceiver.Bind("/End", OnEnd);
    }

    public void ToBlack()
    {
        ToBlack(null);
    }

    public void ToBlack(OSCMessage message)
    {
        StartCoroutine(ToBlackCoroutine());
    }

    public void OnEnd()
    {
        OnEnd(null);
    }

    public void OnEnd(OSCMessage message)
    {
        Transition();
    }

    private IEnumerator ToBlackCoroutine()
    {
        float elapsedTime = 0f;
        float duration = 3f;

        while(elapsedTime < duration)
        {
            _backgroundMaterial.SetFloat("_ColorMultiplier", Mathf.Lerp(1f, 0f, elapsedTime / duration));
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        yield return null;
        OnEnd();
    }
}
