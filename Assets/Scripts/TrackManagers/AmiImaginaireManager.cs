using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class AmiImaginaireManager : TrackTailorMadeManager
{
    private const string DROP_SIGNAL = "DropSignalTime"; // Nom de la propriété exposée pour le rayon de la sphère de conformité.

    private const string SDF_RATIO = "SDF_ratio"; // Nom de la propriété exposée pour le rayon de la sphère de conformité.
    private const string Drop = "drop_"; // Nom de la propriété exposée pour le rayon de la sphère de conformité.
    private float start_time = 0;
    private bool isFinal = false;

    public void Awake()
    {

        Debug.Log("Awake:" + Time.time);
    }

    protected override void Start()
    {
        Debug.Log("Start:" + Time.time);
        start_time = Time.time;
        base.Start();
        base.ApplyDefaultEffects();
    }

    private void ChangeDropLocation(Vector2 location, int drop)
    {
        string name = Drop + drop.ToString();
        m_VFX.SetVector2(name, location);
    }

    public void WaterDropSignal()
    {
        float signalTime = Time.time - start_time - 1.0f;
        Debug.Log("WaterDropSignal :" + signalTime);
        m_VFX.SetFloat(DROP_SIGNAL, signalTime);
        for (int i = 1; i <= 3; i++)
        {
            float random_x = Random.Range(-2.2f, 2.2f);
            float random_y = Random.Range(-1.5f, 1.5f);
            ChangeDropLocation(new Vector2(random_x, random_y), i);
        }
    }

    public void Crescendo_1()
    {
        Debug.Log("Crescendo_1");
        m_VFX.SetFloat(SDF_RATIO, 0.55f);
    }

    public void Crescendo_2()
    {
        Debug.Log("Crescendo_2");
        m_VFX.SetFloat(SDF_RATIO, 0.35f);
    }

    public void Crescendo_f()
    {
        Debug.Log("Crescendo_f");
        isFinal = true;
        m_VFX.SetFloat(SDF_RATIO, 0f);
    }

    public void Diminuendo()
    {
        Debug.Log("Diminuendo");
        if (isFinal)
            StartCoroutine(Utils.Utils.InterpolatVfxFloatVisibility(true, SDF_RATIO, 0.9f, m_VFX, 10f));
        else
            m_VFX.SetFloat(SDF_RATIO, 0.9f);
    }

    public void End()
    {
        m_VFX.SetFloat(rate_name, 0f);
        Debug.Log("End");
    }

}
