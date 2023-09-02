using UnityEngine;

public class BonnesDesillusionsManager : ITrackManager
{
    [SerializeField] private Transform _background;
    protected override void Start()
    {
        base.Start();
        base.ApplyDefaultEffects();
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
