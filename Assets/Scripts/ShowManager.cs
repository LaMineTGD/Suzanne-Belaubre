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

    [SerializeField] private int m_rotationStep = 10;

    private int m_TrackPlaying = -1;
    private Camera m_mainCamera;
    private bool m_isAltitudeDirty = true;

    void Start()
    {
        m_Instance = this;
        m_mainCamera = Camera.main;
        StartNextTrack();
    }

    void Update()
    {
        if(m_isAltitudeDirty)
        {
            SetAltitude();
        }
    }

    void StartNextTrack()
    {
        if (m_TrackPlaying < m_TrackList.Length -1)
        {
            SceneManager.LoadScene(m_TrackList[m_TrackPlaying + 1]._SceneName, LoadSceneMode.Additive);
            if(m_TrackPlaying>=0)
            {
                SceneManager.UnloadSceneAsync(m_TrackList[m_TrackPlaying]._SceneName);
            }
            m_TrackPlaying++;

        }
        else
        {
            SceneManager.LoadScene(m_TrackList[0]._SceneName, LoadSceneMode.Additive);
            SceneManager.UnloadSceneAsync(m_TrackList[m_TrackList.Length -1]._SceneName);
            m_TrackPlaying = 0;
        }

        ApplyEffects();
    }

    void OnNextTrack(InputValue _Value)
    {
        if (_Value.isPressed)
            StartNextTrack();
    }

    private void ApplyEffects()
    {
        m_isAltitudeDirty = true;
    }

    private void SetAltitude()
    {
        float lerpSpeed = 5f;
        var targetVector = new Vector3((float)(m_TrackList[m_TrackPlaying]._Altitude * m_rotationStep), 0f, 0f);
        Quaternion targetRotation = Quaternion.Euler(targetVector);
        var rotation =  Quaternion.Lerp(m_mainCamera.transform.rotation, targetRotation, lerpSpeed * Time.deltaTime);
        m_mainCamera.transform.rotation = rotation;

        if(rotation == targetRotation)
        {
            m_isAltitudeDirty = false;
        }
    }

    private void SetLocation()
    {

    }

    public string GetTrackName()
    {
        return m_TrackList[m_TrackPlaying]._SceneName;
    }

}