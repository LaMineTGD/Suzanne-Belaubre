using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using System.Collections;
using extOSC;

public class CommencementManager : ITrackManager
{
    public GameObject m_VFXObject;
    public float m_Speed;
    public OSCReceiver m_OSCReceiver;
    VisualEffect m_VFX;
    Vector3 m_BaseVFXPosition;
    Color m_ParticleColor;
    Color m_BaseParticleColor;
    float m_Intensity = 1.0f;
    float m_Size;

    protected override void Start()
    {
        m_BaseVFXPosition = m_VFXObject.transform.position;
        m_VFX = m_VFXObject.GetComponent<VisualEffect>();
        m_ParticleColor = m_VFX.GetVector4("ParticleColor");
        m_BaseParticleColor = m_VFX.GetVector4("ParticleColor");
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
        if (Mathf.Abs(_Value.Get<Vector2>().x) < 0.1f && Mathf.Abs(_Value.Get<Vector2>().y) < 0.1f)
            m_ParticleColor = m_BaseParticleColor;
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
        StartCoroutine(FadeIn());
    }

    void OSCFadeOut(OSCMessage message)
    {
        StartCoroutine(FadeOut());
    }

    void OSCPrecipitation(OSCMessage message)
    {
        StartCoroutine(Precipitation());
    }

    void OSCSize(OSCMessage message)
    {
        m_VFX.SetFloat("Size", message.Values[0].FloatValue / 25.0f);
    }

    IEnumerator FadeOut()
    {
        for (int Rate = 2000; Rate >= 0; Rate -= 100)
        {
            //m_ParticleColor.a = alpha;
            m_VFX.SetInt("Rate", Rate);
            yield return new WaitForSeconds(.1f);
        }
    }

    IEnumerator FadeIn()
    {
        for (int Rate = 0; Rate <= 2000; Rate += 100)
        {
            //m_ParticleColor.a = alpha;
            m_VFX.SetInt("Rate", Rate);
            yield return new WaitForSeconds(.1f);
        }
    }

    IEnumerator Precipitation()
    {
        Debug.Log("hey");
        for (float attraction = 0f; attraction <= 10; attraction += 0.5f)
        {
            m_VFX.SetFloat("Attraction", attraction);
            yield return new WaitForSeconds(.1f);
        }
    }
}