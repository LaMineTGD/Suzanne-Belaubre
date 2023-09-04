using extOSC;

public class LesAlarmesManager : ITrackManager
{
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
    }

    private void generateOSCReceveier()
    {
        ShowManager.m_Instance.OSCReceiver.Bind("/Transition", OnTransition);
        ShowManager.m_Instance.OSCReceiver.Bind("/End", OnEnd);
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
}
