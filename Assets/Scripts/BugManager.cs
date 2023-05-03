using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class BugManager : MonoBehaviour
{
    float m_YReference, m_BugIntensity;
    VisualEffect m_Effect;
    void Start()
    {
        m_YReference = transform.position.y;
        m_Effect = GetComponent<VisualEffect>();
    }

    void Update()
    {
        m_Effect.SetFloat("BugIntensity", m_BugIntensity);
        transform.position = new Vector3(transform.position.x, m_YReference, transform.position.z);
        Vector3 _yNoise = new Vector3(transform.position.x, transform.position.y + Random.Range(-m_BugIntensity, m_BugIntensity), transform.position.z);
        transform.position = _yNoise;
    }
    void OnTremble(InputValue _Value)
    {
        m_BugIntensity = _Value.Get<Vector2>().x;
    }

    void OnExplosion(InputValue _Value)
    {
        m_Effect.SetFloat("Explosion", _Value.Get<float>());
    }

    void OnAmplitude(InputValue _Value)
    {
        m_Effect.SetFloat("Amplitude", m_Effect.GetFloat("Amplitude") + _Value.Get<float>());
    }

    void OnFrequency(InputValue _Value)
    {
        m_Effect.SetFloat("Frequency", m_Effect.GetFloat("Frequency") + _Value.Get<float>());
    }
}