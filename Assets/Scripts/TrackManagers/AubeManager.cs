using UnityEngine;

public class AubeManager : ITrackManager
{
    protected override void Start()
    {
        base.Start();
        base.ApplyDefaultEffects();
    }

    public void SayHi()
    {
        Debug.Log("Hi!");
    }
}

