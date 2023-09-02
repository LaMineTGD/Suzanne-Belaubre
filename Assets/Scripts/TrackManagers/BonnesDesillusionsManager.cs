using UnityEngine;

public class BonnesDesillusionsManager : ITrackManager
{
    [SerializeField] private Transform _background;
    protected override void Start()
    {
        base.Start();
        base.ApplyDefaultEffects();

        //Change the sky color
        var currentTrackData = ShowManager.m_Instance.GetCurrentTrack();
        SetSkyColor(currentTrackData._MainColorList[0], currentTrackData._MainColorList[1], currentTrackData._MainColorList[2]);
    }

    public void RemoveBackground()
    {
        _background.Rotate(90f, 0f, 0f);
    }

    public void OnEnd()
    {
        Transition();
    }
}
