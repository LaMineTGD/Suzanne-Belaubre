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

    [SerializeField] private int m_rotationStep = 15;
    [SerializeField] private float m_altitudeLerpDuration = 100f;


    [Serializable]
    public struct TrackList
    {
        public String _SceneName;
        public int _Altitude;
        public Temperature _Temperature;
        public Location _Location;
        public List<Color> _MainColorList;
        public List<Color> _SecondaryColorList;
        public List<Color> _ThirdColorList;
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

    void LoadNextTrack()
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

    void OnNextTrack(InputValue _Value)
    {
        if (_Value.isPressed)
            StartNextTrack();
    }

    private void ApplyTrackBasicEffects()
    {
        if(m_altitudeCoroutine != null)
        {
            StopCoroutine(m_altitudeCoroutine);
        }

        m_altitudeCoroutine = SetAltitude();
        StartCoroutine(m_altitudeCoroutine);
    }

    private IEnumerator SetAltitude()
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

    private void SetLocation()
    {

    }

    public string GetTrackName()
    {
        return m_TrackList[m_TrackPlaying]._SceneName;
    }

}