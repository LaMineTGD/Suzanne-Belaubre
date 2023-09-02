public class LesVoiesDoreesManager : ITrackManager
{
    protected override void Start()
    {
        base.Start();
        base.ApplyDefaultEffects();
         //Change the sky color
        var currentTrackData = ShowManager.m_Instance.GetCurrentTrack();
        SetSkyColor(currentTrackData._MainColorList[0], currentTrackData._MainColorList[1], currentTrackData._MainColorList[2]);
        BoostTones();
    }

    public void OnTransition()
    {
    }

    public void OnEnd()
    {
        Transition();
    }
}
