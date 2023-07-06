using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class LineVFXManager : MonoBehaviour
{
    private const string COLOR_OVER_LIFETIME_PROPERTY = "Color_Over_LifeTime";

    [SerializeField] private float m_ColorLerpDuration = 5f;

    private VisualEffect m_LineVFX;
    private IEnumerator m_ColorCoroutine;

    private void Awake()
    {
        m_LineVFX = GetComponent<VisualEffect>();
    }

    public void SetColorOverLifetime(Color color)
    {
        if(m_ColorCoroutine != null)
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

        while(elapsedTime < m_ColorLerpDuration)
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


}
