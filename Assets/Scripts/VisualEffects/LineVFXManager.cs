using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class LineVFXManager : MonoBehaviour
{
    private const string COLOR_OVER_LIFETIME_PROPERTY = "Color_Over_LifeTime";
    private const string rate_name = "Rate";
    private const string line_radius_name = "LineRadius";
    private const string value1_name = "Value_1";

    [SerializeField] private float m_ColorLerpDuration = 5f;
    [SerializeField] private float m_PulseLerpDuration = 0.1f;
    [SerializeField] protected float base_rate_value = 67534;
    [SerializeField] private float m_EffilageLerpDuration = 10000f;
    [SerializeField] private Vector2 base_value1= new (2f, 30f);


    private VisualEffect m_LineVFX;
    private IEnumerator m_ColorCoroutine;
    private IEnumerator m_PulseEffectCoroutine;
    private IEnumerator m_EffilageEffectCoroutine;
    private float baseLineRadius;
    private Vector2 movingValue1;


    private void Awake()
    {
        m_LineVFX = GetComponent<VisualEffect>();
    }

    private void Start()
    {
        baseLineRadius = m_LineVFX.GetFloat(line_radius_name);
        m_LineVFX.SetVector2(value1_name, base_value1);
        m_LineVFX.SetFloat(rate_name, base_rate_value);
        movingValue1 = base_value1;
    }

    public void SetColorOverLifetime(Color color)
    {
        if (m_ColorCoroutine != null)
        {
            StopCoroutine(m_ColorCoroutine);
        }

        m_ColorCoroutine = ChangeColorCoroutine(color);
        StartCoroutine(m_ColorCoroutine);
    }

    private IEnumerator ChangeColorCoroutine(Color targetColor)
    {
        float elapsedTime = 0f;
        Gradient targetGradient = m_LineVFX.GetGradient(COLOR_OVER_LIFETIME_PROPERTY);
        Color startColor = targetGradient.colorKeys[1].color;

        while (elapsedTime < m_ColorLerpDuration)
        {
            GradientColorKey[] keys = targetGradient.colorKeys;
            keys[keys.Length - 1].color = Color.Lerp(startColor, targetColor, elapsedTime / m_ColorLerpDuration);
            targetGradient.SetKeys(keys, targetGradient.alphaKeys);

            m_LineVFX.SetGradient(COLOR_OVER_LIFETIME_PROPERTY, targetGradient);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        yield return null;
    }

    public void SetVisible(bool isVisible, float duration)
    {
        StartCoroutine(Utils.Utils.InterpolatVfxFloatVisibility(isVisible, rate_name, base_rate_value, m_LineVFX, duration));
    }

    public void EffilageEffect(float speed)
    {
        if (m_EffilageEffectCoroutine != null)
        {
            StopCoroutine(m_EffilageEffectCoroutine);
        }

        m_EffilageEffectCoroutine = EffilageEffectCoroutine(speed);
        StartCoroutine(m_EffilageEffectCoroutine);
    }

    private IEnumerator EffilageEffectCoroutine(float speed)
    {
        float elapsedTime = 0f;
        Vector2 targetValue1 = new (30f, 2f);

        while(elapsedTime < m_EffilageLerpDuration)
        {
            movingValue1 = Vector2.Lerp(base_value1, targetValue1, elapsedTime / (m_EffilageLerpDuration * speed));
            m_LineVFX.SetVector2(value1_name, movingValue1);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        yield return null;
    }

    public void PulseEffect()
    {
        if (m_PulseEffectCoroutine != null)
        {
            StopCoroutine(m_PulseEffectCoroutine);
            m_LineVFX.SetFloat(line_radius_name, baseLineRadius);
        }

        m_PulseEffectCoroutine = PulseEffectCoroutine();
        StartCoroutine(m_PulseEffectCoroutine);
    }

    private IEnumerator PulseEffectCoroutine()
    {
        float elapsedTime = 0f;
        float targetLineRadius = 0.15f;
        float movingLineRadius;

        while(elapsedTime < m_PulseLerpDuration)
        {
            movingLineRadius = Mathf.SmoothStep(baseLineRadius, targetLineRadius, elapsedTime / m_PulseLerpDuration);
            m_LineVFX.SetFloat(line_radius_name, movingLineRadius);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        while(elapsedTime > 0)
        {
            movingLineRadius = Mathf.SmoothStep(baseLineRadius, targetLineRadius, elapsedTime / m_PulseLerpDuration);
            m_LineVFX.SetFloat(line_radius_name, movingLineRadius);

            elapsedTime -= Time.deltaTime;

            yield return null;
        }

        yield return null;
    }
}
