using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class LineVFXManager : MonoBehaviour
{
    private const string COLOR_OVER_LIFETIME_PROPERTY = "Color_Over_LifeTime";
    private const string rate_name = "Rate";
    private const string line_radius_name = "LineRadius";
    private const string lifetime_name = "LifeTime";
    private const string value1_name = "Value_1";
    private const string value2_name = "Value_2";
    private const string circle_name = "Circle";
    private const string particle_speed_name = "Particule_Speed";

    private const float DEFAULT_RATE_VALUE = 67534;
    private const float DEFAULT_RADIUS_VALUE = .1f;
    private const float DEFAULT_VALUE1_X = 2f;
    private const float DEFAULT_VALUE1_Y = 30f;

    [SerializeField] private float m_ColorLerpDuration = 5f;
    [SerializeField] private float m_PulseLerpDuration = 0.1f;
    [SerializeField] protected float base_rate_value = DEFAULT_RATE_VALUE;
    [SerializeField] private float m_EffilageLerpDuration = 1000f;
    [SerializeField] private Vector2 base_value1= new (DEFAULT_VALUE1_X, DEFAULT_VALUE1_Y);


    private VisualEffect m_LineVFX;
    private IEnumerator m_ColorCoroutine;
    private IEnumerator m_PulseEffectCoroutine;
    private IEnumerator m_EffilageEffectCoroutine;
    private Vector2 movingValue1;


    private void Awake()
    {
        m_LineVFX = GetComponent<VisualEffect>();
    }

    private void Start()
    {
        InitValues();
        movingValue1 = base_value1;
    }

    private void InitValues()
    {
        SetRate(DEFAULT_RATE_VALUE);
        SetLineRadius(DEFAULT_RADIUS_VALUE);
        SetLineAspectValue1(base_value1);
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

            SetColorOverLifetime(targetGradient);

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

    public void EndEffilageEffect()
    {
        if (m_EffilageEffectCoroutine != null)
        {
            StopCoroutine(m_EffilageEffectCoroutine);
        }
    }

    private IEnumerator EffilageEffectCoroutine(float speed)
    {
        float elapsedTime = 0f;
        Vector2 targetValue1 = new (30f, 2f);

        Vector2 currentValue1 = m_LineVFX.GetVector2(value1_name);
        if((currentValue1 - targetValue1).x < Mathf.Epsilon && (currentValue1 - targetValue1).y < Mathf.Epsilon)
        {
            //if traget value was already reached, rotate back to the other side
            targetValue1 = new(2f, 30f);
        }

        while((elapsedTime * speed) < m_EffilageLerpDuration)
        {
            movingValue1 = Vector2.Lerp(base_value1, targetValue1, (elapsedTime * speed) / m_EffilageLerpDuration);
            SetLineAspectValue1(movingValue1);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        yield return null;
    }

    public void PulseEffect(float targetRadius = .15f)
    {
        if (m_PulseEffectCoroutine != null)
        {
            StopCoroutine(m_PulseEffectCoroutine);
            SetLineRadius(DEFAULT_RADIUS_VALUE);
        }

        m_PulseEffectCoroutine = PulseEffectCoroutine(targetRadius);
        StartCoroutine(m_PulseEffectCoroutine);
    }

    private IEnumerator PulseEffectCoroutine(float targetLineRadius)
    {
        float elapsedTime = 0f;
        float movingLineRadius;

        while(elapsedTime < m_PulseLerpDuration)
        {
            movingLineRadius = Mathf.SmoothStep(DEFAULT_RADIUS_VALUE, targetLineRadius, elapsedTime / m_PulseLerpDuration);
            SetLineRadius(movingLineRadius);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        while(elapsedTime > 0)
        {
            //Lerp going backwards
            movingLineRadius = Mathf.SmoothStep(DEFAULT_RADIUS_VALUE, targetLineRadius, elapsedTime / m_PulseLerpDuration);
            SetLineRadius(movingLineRadius);

            elapsedTime -= Time.deltaTime;

            yield return null;
        }
        yield return null;
    }

    public void SetColorOverLifetime(Gradient gradient)
    {
        m_LineVFX.SetGradient(COLOR_OVER_LIFETIME_PROPERTY, gradient);
    }

    public void SetRate(float rate)
    {
        m_LineVFX.SetFloat(rate_name, rate);
    }

    public float GetDefaultRate()
    {
        return DEFAULT_RATE_VALUE;
    }

    public float GetRate()
    {
        return m_LineVFX.GetFloat(rate_name);
    }

    public void SetLineRadius(float lineRadius)
    {
        if(lineRadius > 1 || lineRadius < 0)
        {
            Debug.LogError("LineRadius must be a float in range 0 to 1.0");
        }
        else
        {
            m_LineVFX.SetFloat(line_radius_name, lineRadius);
        }
    }

    public float GetRadius()
    {
        return m_LineVFX.GetFloat(line_radius_name);
    }

    public float GetDefaultRadius()
    {
        return DEFAULT_RADIUS_VALUE;
    }

    public void SetParticleSpeed(float particleSpeed)
    {
        m_LineVFX.SetFloat(particle_speed_name, particleSpeed);
    }

    public void SetLifeTime(Vector2 lifetime)
    {
        m_LineVFX.SetVector2(lifetime_name, lifetime);
    }

    public void SetLineAspectValue1(Vector2 value1)
    {
        m_LineVFX.SetVector2(value1_name, value1);
    }

    public Vector2 GetLineAspectValue1()
    {
        return m_LineVFX.GetVector2(value1_name);
    }

    public Vector2 GetLineAspectDefaultValue1()
    {
        return base_value1;
    }

    public void SetLineAspectValue2(Vector2 value2)
    {
        m_LineVFX.SetVector2(value2_name, value2);

    }

    public void SetLineAspectCircle(Vector2 circle)
    {
        m_LineVFX.SetVector2(circle_name, circle);
    }
}
