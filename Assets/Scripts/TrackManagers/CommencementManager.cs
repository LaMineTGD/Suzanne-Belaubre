using UnityEngine;
using UnityEngine.InputSystem;
using extOSC;

public class CommencementManager : ITrackManager
{
    public GameObject m_VFX;
    public float m_Speed;
    Vector3 m_BaseVFXPosition;

    protected override void Start()
    {
        base.Start();
        base.ApplyDefaultEffects();
        //Change the sky color
        var currentTrackData = ShowManager.m_Instance.GetCurrentTrack();
        SetSkyColor(currentTrackData._MainColorList[0], currentTrackData._MainColorList[1], currentTrackData._MainColorList[2]);
        m_BaseVFXPosition = m_VFX.transform.position;

        generateOSCReceveier();
    }

    private void generateOSCReceveier()
    {
        ShowManager.m_Instance.OSCReceiver.Bind("/Transition", OnTransition);
    }

    public void OnTransition()
    {
        OnTransition(null);
    }

    public void OnTransition(OSCMessage message)
    {
        Transition();
    }

    void OnTremble(InputValue _Value)
    {
        Debug.Log(m_BaseVFXPosition.z);
        Vector3 _TempPos = new Vector3(m_BaseVFXPosition.x + _Value.Get<Vector2>().x * m_Speed, m_BaseVFXPosition.y, m_BaseVFXPosition.z + _Value.Get<Vector2>().y * m_Speed);
        m_VFX.transform.position = _TempPos;
    }
}