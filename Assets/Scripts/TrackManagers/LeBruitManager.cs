public class LeBruitManager : TrackTailorMadeManager
{
    int m_TrackNumber;

    protected override void Start()
    {
        base.Start();
        base.ApplyDefaultEffects();

        for(int i=0;i< ShowManager.m_Instance.m_TrackList.Length;i++)
        {
            if(ShowManager.m_Instance.m_TrackList[i]._SceneName == "LeBruit")
            {
                m_TrackNumber = i;
            }
        }

        //ShowManager.m_Instance.GetSkyFogManager().SetSkyColor(SkyFogManager.SkyLevel.Top, ShowManager.m_Instance.m_TrackList[m_TrackNumber]._MainColorList[1]);
        //ShowManager.m_Instance.GetSkyFogManager().SetSkyColor(SkyFogManager.SkyLevel.Middle, ShowManager.m_Instance.m_TrackList[m_TrackNumber]._MainColorList[0]);
        //ShowManager.m_Instance.GetSkyFogManager().SetSkyColor(SkyFogManager.SkyLevel.Bottom, ShowManager.m_Instance.m_TrackList[m_TrackNumber]._MainColorList[1]);
    }
}