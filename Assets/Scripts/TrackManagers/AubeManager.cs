using UnityEngine;

public class AubeManager : TrackTailorMadeManager
{
    protected override void Start()
    {
        base.Start();
        base.ApplyDefaultEffects();
    }

    public void DeuxiemeVagueHarmonique()
    {
        Debug.Log("DeuxiemeVagueHarmonique");
    }

    public void DebutPercussion()
    {
        Debug.Log("DebutPercussion");
    }

    public void ChantContreSens()
    {
        Debug.Log("ChantContreSens");
    }

    public void AubeSeLeve()
    {
        Debug.Log("AubeSeLeve");
    }

    public void DebutTransition()
    {
        Debug.Log("DebutTransition");
    }
}

