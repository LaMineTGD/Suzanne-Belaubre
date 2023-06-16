using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class ShowManager : MonoBehaviour
{
    public enum Location { Interior, Exterior, Both, InBetween, Neither };

    [Serializable]
    public struct TrackList
    {
        public String _SceneName;
        public int _Altitude;
        public int _Temperature;
        public Location _Location;
        public List<Color> _MainColorList;
        public List<Color> _SecondaryColorList;
    }

    [NonReorderable] public TrackList[] m_TrackList;
    private int m_TrackPlaying = -1;

    void Start()
    {
        StartNextTrack();
    }

    void Update()
    {
        
    }

    void StartNextTrack()
    {
        if (m_TrackPlaying < m_TrackList.Length)
            SceneManager.LoadScene(m_TrackList[m_TrackPlaying + 1]._SceneName, LoadSceneMode.Additive);
        if(m_TrackPlaying>=0)
            SceneManager.UnloadSceneAsync(m_TrackList[m_TrackPlaying]._SceneName);
        m_TrackPlaying++;
    }

    void OnNextTrack(InputValue _Value)
    {
        Debug.Log("meuh");
        if (_Value.isPressed)
            StartNextTrack();
    }
}