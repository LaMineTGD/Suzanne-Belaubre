using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using extOSC;

public class CommencementManager : ITrackManager
{
    public GameObject m_VFXObject;
    public float m_Speed;
    public OSCReceiver m_OSCReceiver;
    VisualEffect m_VFX;
    Vector3 m_BaseVFXPosition;
    Color m_ParticleColor;
    float m_Intensity = 1.0f;
    float m_Size;

    protected override void Start()
    {
        m_BaseVFXPosition = m_VFXObject.transform.position;
        m_VFX = m_VFXObject.GetComponent<VisualEffect>();
        m_ParticleColor = m_VFX.GetVector4("ParticleColor");
        m_Size = m_VFX.GetFloat("Size");
        m_OSCReceiver.Bind("/Note1", OSCNote);
        m_OSCReceiver.Bind("/FadeIn", OSCFadeIn);
        m_OSCReceiver.Bind("/Precipitation", OSCPrecipitation);
        m_OSCReceiver.Bind("/Size", OSCSize);
        m_OSCReceiver.Bind("/FadeOut", OSCFadeOut);
        base.Start();
        base.ApplyDefaultEffects();
        //Change the sky color
        var currentTrackData = ShowManager.m_Instance.GetCurrentTrack();
        SetSkyColor(currentTrackData._MainColorList[0], currentTrackData._MainColorList[1], currentTrackData._MainColorList[2]);

        generateOSCReceveier();
    }

    private void Update()
    {
        Debug.Log(m_Intensity);
        m_VFX.SetVector4("ParticleColor", m_ParticleColor * m_Intensity);
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
        Vector3 _TempPos = new Vector3(m_BaseVFXPosition.x + _Value.Get<Vector2>().x * m_Speed, m_BaseVFXPosition.y, m_BaseVFXPosition.z + _Value.Get<Vector2>().y * m_Speed);
        m_VFXObject.transform.position = _TempPos;
    }

    void OnFOV(InputValue _Value)
    {
        m_ParticleColor = new Vector4(_Value.Get<Vector2>().x + 1, m_ParticleColor.g, _Value.Get<Vector2>().y + 1, 255);
    }

    void OnIntensity(InputValue _Value)
    {
        m_Intensity += _Value.Get<float>();
        m_Intensity = Mathf.Clamp(m_Intensity, 0.0f, 10.0f);
    }

    void OSCNote(OSCMessage message)
    {
        //m_VFX.SetFloat("Size", message.Values[0].IntValue / 20.0f);
    }

    void OSCFadeIn(OSCMessage message)
    {
        
    }

    void OSCFadeOut(OSCMessage message)
    {

    }

    void OSCPrecipitation(OSCMessage message)
    {

    }
    void OSCSize(OSCMessage message)
    {
        m_VFX.SetFloat("Size", message.Values[0].FloatValue / 25.0f);
    }
}