using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class ShowManager : MonoBehaviour
{
    [HideInInspector] public static ShowManager m_Instance;
    public enum Location { Interior, Exterior, Both, InBetween, Neither };
    public enum Temperature { Warm, Cool, Both };

    [Header("Script Requirements")]
    [SerializeField] private SkyFogManager m_SkyFogManager;

    [Header("Editables")]
    [SerializeField] private int m_rotationStep = 15;
    [SerializeField] private float m_altitudeLerpDuration = 100f;


    [Serializable]
    public struct TrackList
    {
        public String _SceneName;
        public int _Altitude;
        public Temperature _Temperature;
        public Location _Location;
        [ColorUsage(true, true)] //enables HDR color in the inspector
        public List<Color> _MainColorList;
        [ColorUsage(true, true)]
        public List<Color> _SecondaryColorList;
        [ColorUsage(true, true)]
        public List<Color> _ThirdColorList;
        [ColorUsage(true, true)]
        public List<Color> _FourthColorList;
    }

    [NonReorderable] public TrackList[] m_TrackList;

    private int m_TrackPlaying = -1;
    private Camera m_MainCamera;
    private IEnumerator m_altitudeCoroutine;

    private void Awake()
    {
        if(m_Instance != null && m_Instance != this)
        {
            Destroy(this);
        }
        else
        {
            m_Instance = this;
        }
    }

    void Start()
    {
        m_MainCamera = Camera.main;
        StartNextTrack();
    }

    private void StartNextTrack()
    {
        LoadNextTrack();
        ApplyTrackBasicEffects();
    }

    private void LoadNextTrack()
    {
        if (m_TrackPlaying < m_TrackList.Length -1)
        {
            //go to following track
            SceneManager.LoadScene(m_TrackList[m_TrackPlaying + 1]._SceneName, LoadSceneMode.Additive);
            if(m_TrackPlaying>=0)
            {
                SceneManager.UnloadSceneAsync(m_TrackList[m_TrackPlaying]._SceneName);
            }
            m_TrackPlaying++;

        }
        else
        {
            //reached end of track list, loops back to beginning
            SceneManager.LoadScene(m_TrackList[0]._SceneName, LoadSceneMode.Additive);
            SceneManager.UnloadSceneAsync(m_TrackList[m_TrackList.Length -1]._SceneName);
            m_TrackPlaying = 0;
        }
    }

    private void OnNextTrack(InputValue _Value)
    {
        if (_Value.isPressed)
            StartNextTrack();
    }

    private void ApplyTrackBasicEffects()
    {
        SetAltitude();
        SetSkyColor();
    }

    private void SetAltitude()
    {
        if(m_altitudeCoroutine != null)
        {
            StopCoroutine(m_altitudeCoroutine);
        }

        m_altitudeCoroutine = SetAltitudeCoroutine();
        StartCoroutine(m_altitudeCoroutine);
    }

    private IEnumerator SetAltitudeCoroutine()
    {
        float elapsedTime = 0f;
        while(elapsedTime < m_altitudeLerpDuration)
        {
            var targetVector = new Vector3((float)(m_TrackList[m_TrackPlaying]._Altitude * m_rotationStep), 0f, 0f);
            Quaternion targetRotation = Quaternion.Euler(targetVector);
            Quaternion initialRotation = m_MainCamera.transform.rotation;
            m_MainCamera.transform.rotation =  Quaternion.Lerp(initialRotation, targetRotation, elapsedTime / m_altitudeLerpDuration);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        yield return null;
    }

    private void SetSkyColor()
    {
        Color color = Color.black;
        if(m_TrackList[m_TrackPlaying]._MainColorList != null && m_TrackList[m_TrackPlaying]._MainColorList.Count != 0)
        {
            //sets the middle color of the sky to a darker version of the first main color
            color = m_TrackList[m_TrackPlaying]._MainColorList[0];

            //reduce intensity by 2
            float intensity = -2f;
            for (int i = 0; i < 3; i++)
            {
                color[i] *= (float)Math.Pow(2f,intensity);
            }
        }
        
        m_SkyFogManager.SetSkyColor(SkyFogManager.SkyLevel.Middle, color);
    }

    public string GetTrackName()
    {
        return m_TrackList[m_TrackPlaying]._SceneName;
    }

}