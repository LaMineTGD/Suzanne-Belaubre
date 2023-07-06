using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class ShowManager : MonoBehaviour
{
    [HideInInspector] public static ShowManager m_Instance;
    public enum Location { Interior, Exterior, Both, InBetween, Neither };
    public enum Temperature { Warm, Cool, Both };

    [Header("References")]
    [SerializeField] private SkyFogManager m_SkyFogManager;
    [SerializeField] private LineVFXManager m_LineVFXManager;
    [SerializeField] private PostProcessVolumeManager m_postProcessVolumeManager;

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
        //m_MainCamera = Camera.main;
        StartNextTrack();
    }

    private void StartNextTrack()
    {
        LoadNextTrack();
        //ApplyTrackBasicEffects();
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

    public string GetTrackName()
    {
        return m_TrackList[m_TrackPlaying]._SceneName;
    }

    public ShowManager.TrackList GetCurrentTrack()
    {
        return m_TrackList[m_TrackPlaying];
    }

    public SkyFogManager GetSkyFogManager()
    {
        return m_SkyFogManager;
    }

    public LineVFXManager GetLineVFXManager()
    {
        return m_LineVFXManager;
    }

    public PostProcessVolumeManager GetPostProcessVolumeManager()
    {
        return m_postProcessVolumeManager;
    }

}