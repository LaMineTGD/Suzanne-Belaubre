using extOSC;
using Utils;

public class LesVoiesDoreesManager : TrackTailorMadeManager
{
    protected override void Start()
    {
        base.Start();
        base.ApplyDefaultEffects();
         //Change the sky color
        var currentTrackData = ShowManager.m_Instance.GetCurrentTrack();
        SetSkyColor(currentTrackData._MainColorList[0], currentTrackData._MainColorList[1], currentTrackData._MainColorList[2]);
        //BoostTones();
        generateOSCReceveier();

    }

    private void generateOSCReceveier()
    {
        ShowManager.m_Instance.OSCReceiver.Bind("/End", OnEnd);
    }

    public void OnEnd()
    {
        OnEnd(null);
    }

    public void OnEnd(OSCMessage message)
    {
        Transition();
    }
}
