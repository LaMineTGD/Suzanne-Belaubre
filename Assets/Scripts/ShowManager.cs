using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using extOSC;
using UnityEngine.Events;

public class ShowManager : MonoBehaviour
{
    [HideInInspector] public static ShowManager m_Instance;
    [HideInInspector] public UnityEvent TransitionAube;
    public enum Location { Interior, Exterior, Both, InBetween, Neither };
    public enum Temperature { Warm, Cool, Both };
    public enum TrackType { Default, TailorMade };
    [SerializeField] public OSCReceiver OSCReceiver;
    [Header("References")]
    [SerializeField] private SkyFogManager m_SkyFogManager;
    [SerializeField] private LineVFXManager m_LineVFXManager;
    [SerializeField] private PostProcessVolumeManager m_PostProcessVolumeManager;
    [SerializeField] private ITrackManager m_ITrackManager;

    [Serializable]
    public struct TrackList
    {
        public String _SceneName;
        public int _Altitude;
        public TrackType _Type;
        public float _Start_Transition_duration;
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

    private TrackTailorMadeManager currentTailorManager;
    private int m_TrackPlaying = -1;
    private bool firstTrack;
    private bool isTransitionning = false;
    private Coroutine currentTransition;
    private Volume _previousTailorSkyVolume;
    private Volume _previousTailorPostProcessVolume;

    private void Awake()
    {
        if (m_Instance != null && m_Instance != this)
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
        firstTrack = true;
        currentTailorManager = null;
        StartNextTrack();

        OSCReceiver.Connect();
        Debug.Log("OSCR :" + OSCReceiver.IsStarted);
    }


    private void StartNextTrack()
    {
        LoadNextTrack();
    }

    private IEnumerator DeleteSceneWithDelay(float duration, string sceneName)
    {
        float elapsedTime = 0f;
        isTransitionning = true;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SceneManager.UnloadSceneAsync(sceneName);
        isTransitionning = false;
        yield return null;
    }

    public void LoadNextTrack()
    {
        int lastTrack = (m_TrackList.Length + m_TrackPlaying - 1) % m_TrackList.Length;
        int currentTrack = (m_TrackList.Length + m_TrackPlaying) % m_TrackList.Length;
        int nextTrack = (currentTrack + 1) % m_TrackList.Length;

        // SceneManager.LoadScene(m_TrackList[nextTrack]._SceneName, LoadSceneMode.Additive);
        // float duration = m_TrackList[nextTrack]._Start_Transition_duration;
        float duration = 0f;

        if (firstTrack)
        {
            duration = 0.0f;
        }
        if (isTransitionning)
        {
            StopCoroutine(currentTransition);
            SceneManager.UnloadSceneAsync(m_TrackList[lastTrack]._SceneName);
            isTransitionning = false;
        }
        if (!firstTrack)
        {
            currentTransition = StartCoroutine(DeleteSceneWithDelay(duration, m_TrackList[currentTrack]._SceneName));
        }

        if (m_TrackList[nextTrack]._Type == TrackType.TailorMade && (m_TrackList[currentTrack]._Type == TrackType.Default || firstTrack))
            SetDefaultVisible(false, duration);
        else if (m_TrackList[currentTrack]._Type == TrackType.TailorMade)
        {
            currentTailorManager.SetTransitionToVisibleOff(duration);

            if (m_TrackList[nextTrack]._Type == TrackType.Default)
            {
                _previousTailorSkyVolume = currentTailorManager.GetTailorMadeSkyVolume();
                _previousTailorPostProcessVolume = currentTailorManager.GetTailorMadePostProcessVolume();
                // SetDefaultVisible(true, duration);
                SetDefaultVisible(true, 0f);
            }
        }
        SceneManager.LoadScene(m_TrackList[nextTrack]._SceneName, LoadSceneMode.Additive);

        firstTrack = false;
        m_TrackPlaying = nextTrack;
    }


    private void SetDefaultVisible(bool isVisible, float duration)
    {
        m_SkyFogManager.SetVisible(isVisible, duration);
        m_LineVFXManager.SetVisible(isVisible, duration);
        m_PostProcessVolumeManager.SetVisible(isVisible, duration);
    }

    private void OnNextTrack(InputValue _Value)
    {
        if (_Value.isPressed)
        {
            //StartNextTrack();
            if(GetCurrentTrack()._SceneName == "Aube")
            {
                TransitionAube.Invoke();
            }
        }
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
        return m_PostProcessVolumeManager;
    }

    public void SetCurrentTailorTrack(TrackTailorMadeManager manager)
    {
        currentTailorManager = manager;
    }

    public bool IsPreviousTrackTailorMade()
    {
        if (m_TrackPlaying == -1 || m_TrackPlaying == 0)
        {
            return false;
        }
        else
        {
            return m_TrackList[m_TrackPlaying - 1]._Type == TrackType.TailorMade;
        }
    }

    public Volume[] GetPreviousTailorVolumes()
    {
        Volume[] result = new Volume[2] { _previousTailorSkyVolume, _previousTailorPostProcessVolume };
        return result;
    }

}