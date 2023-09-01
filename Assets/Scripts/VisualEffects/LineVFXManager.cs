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
    private const float DEFAULT_RADIUS_VALUE = .15f;
    private const float DEFAULT_PARTICLE_SPEED = 0.52f;
    private const float DEFAULT_VALUE1_X = -0.39f;
    private const float DEFAULT_VALUE1_Y = 5.03f;
    private const float DEFAULT_VALUE2_X = -2.33f;
    private const float DEFAULT_VALUE2_Y = 1.66f;
    private const float CIRCLE_X = 1.46f;
    private const float CIRCLE_Y = 1000.5f;

    private float m_ColorLerpDuration = 5f;
    protected float base_rate_value = DEFAULT_RATE_VALUE;
    private Vector2 base_value1= new (DEFAULT_VALUE1_X, DEFAULT_VALUE1_Y);
    private Vector2 base_value2= new (DEFAULT_VALUE2_X, DEFAULT_VALUE2_Y);
    private Vector2 base_circle = new(CIRCLE_X, CIRCLE_Y);
    private VisualEffect m_LineVFX;
    private IEnumerator m_ColorCoroutine;

    private void Awake()
    {
        m_LineVFX = GetComponent<VisualEffect>();
    }

    private void Start()
    {
        InitValues();
    }

    private void InitValues()
    {
        SetRate(DEFAULT_RATE_VALUE);
        SetLineRadius(DEFAULT_RADIUS_VALUE);
        SetParticleSpeed(DEFAULT_PARTICLE_SPEED);
        SetLineAspectValue1(base_value1);
        SetLineAspectValue2(base_value2);
        SetLineAspectCircle(base_circle);
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

    #region Setters

    public void SetLineVFXPosition(Vector3 targetPosition)
    {
        m_LineVFX.transform.position = targetPosition;
    }

    public void SetColorOverLifetime(Gradient gradient)
    {
        m_LineVFX.SetGradient(COLOR_OVER_LIFETIME_PROPERTY, gradient);
    }

    public void SetRate(float rate)
    {
        m_LineVFX.SetFloat(rate_name, rate);
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

    public void SetLineAspectValue2(Vector2 value2)
    {
        m_LineVFX.SetVector2(value2_name, value2);

    }

    public void SetLineAspectCircle(Vector2 circle)
    {
        m_LineVFX.SetVector2(circle_name, circle);
    }
    #endregion

    #region Getters
    public float GetDefaultRate()
    {
        return DEFAULT_RATE_VALUE;
    }

    public float GetRate()
    {
        return m_LineVFX.GetFloat(rate_name);
    }

    public float GetDefaultParticleSpeed()
    {
        return DEFAULT_PARTICLE_SPEED;
    }

    public float GetParticleSpeed()
    {
        return m_LineVFX.GetFloat(particle_speed_name);
    }

    public float GetRadius()
    {
        return m_LineVFX.GetFloat(line_radius_name);
    }

    public float GetDefaultRadius()
    {
        return DEFAULT_RADIUS_VALUE;
    }

    public Vector2 GetLineAspectValue1()
    {
        return m_LineVFX.GetVector2(value1_name);
    }

    public Vector2 GetLineAspectDefaultValue1()
    {
        return base_value1;
    }

    public Vector2 GetLineAspectValue2()
    {
        return m_LineVFX.GetVector2(value2_name);
    }

    public Vector2 GetLineAspectDefaultValue2()
    {
        return base_value2;
    }

    public Vector2 GetLineAspectDefaultCircle()
    {
        return base_circle;
    }

    public Vector2 GetLineAspectCircle()
    {
        return m_LineVFX.GetVector2(circle_name);
    }

    #endregion
}
