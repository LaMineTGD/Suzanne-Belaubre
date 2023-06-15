using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class HorizonManager : MonoBehaviour
{
    public float m_RotationSpeed, m_FOVSpeed, m_GradientSpeed;
    public Volume m_SkyAndFogVolume, m_PostProcessVolume;
    float m_RotationValue, m_FOVValue, m_GradientValue;
    GradientSky m_Sky;
    Fog m_Fog;
    Camera m_Camera;

    void Start()
    {
        m_Camera = Camera.main;
        m_SkyAndFogVolume.profile.TryGet(out m_Sky);
        m_SkyAndFogVolume.profile.TryGet(out m_Fog);
    }

    void Update()
    {
        m_Camera.transform.Rotate(new Vector3(m_RotationValue * m_RotationSpeed, 0f, 0f));
        m_Camera.fieldOfView += m_FOVValue * m_FOVSpeed;
        m_Sky.gradientDiffusion.value = Mathf.Clamp(m_Sky.gradientDiffusion.value + m_GradientValue * m_GradientSpeed, 0f, 100f);
    }

    void OnBascule(InputValue _Value)
    {
        m_RotationValue = _Value.Get<float>();
    }

    void OnFOV(InputValue _Value)
    {
        m_FOVValue = _Value.Get<Vector2>().x;
        m_GradientValue = _Value.Get<Vector2>().y;
    }

    void OnFogToggle(InputValue _Value)
    {
        if(_Value.isPressed)
            m_Fog.active = !m_Fog.active;
    }
}