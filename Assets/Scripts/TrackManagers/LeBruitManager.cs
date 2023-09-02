public class LeBruitManager : ITrackManager
{
    protected override void Start()
    {
        base.Start();
        base.ApplyDefaultEffects();

        //Change the sky color
        var currentTrackData = ShowManager.m_Instance.GetCurrentTrack();
        SetSkyColor(currentTrackData._MainColorList[0],
         currentTrackData._MainColorList[1],
         currentTrackData._MainColorList[2]);
    }
}