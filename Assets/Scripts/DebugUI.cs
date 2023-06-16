using System.Net;
using System.Net.Sockets;
using UnityEngine;
using TMPro;

public class DebugUI : MonoBehaviour
{
    public TMP_Text m_FPSText, m_TrackName, m_IPText;

    const float fpsMeasurePeriod = 0.5f;
    private int m_FpsAccumulator = 0;
    private float m_FpsNextPeriod = 0;
    private int m_CurrentFps;

    void Start()
    {
        m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
    }

    void Update()
    {
        m_FPSText.text = GetFramerate();
        m_IPText.text = GetLocalIPv4();
    }

    public string GetFramerate()
    {
        m_FpsAccumulator++;
        if (Time.realtimeSinceStartup > m_FpsNextPeriod)
        {
            m_CurrentFps = (int)(m_FpsAccumulator / fpsMeasurePeriod);
            m_FpsAccumulator = 0;
            m_FpsNextPeriod += fpsMeasurePeriod;
        }

        return m_CurrentFps.ToString() + " FPS";
    }

    public string GetLocalIPv4()
    {
        IPHostEntry host;
        string localIP = "0.0.0.0";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }
        return localIP;
    }
}