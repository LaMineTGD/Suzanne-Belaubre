using UnityEngine;

public class Tracks : MonoBehaviour
{
    public static Tracks Instance { get; private set; }

    public enum TracksEnum
    {
        AliceAlice,
        AmiImaginaire,
        Aube,
        BonnesDesillusions,
        CestRien,
        Commencement,
        LeBruit,
        LesAlarmes,
        LesVoiesDorees,
        Seum,
        Siffle
    }

    [SerializeField]
    private  AudioClip _trackAliceAlice;
    [SerializeField]
    private  AudioClip _trackAmiImaginaire;
    [SerializeField]
    private  AudioClip _trackAube;
    [SerializeField]
    private  AudioClip _trackBonnesDesillusions;
    [SerializeField]
    private  AudioClip _trackCestRien;
    [SerializeField]
    private  AudioClip _trackCommencement;
    [SerializeField]
    private  AudioClip _trackLeBruit;
    [SerializeField]
    private  AudioClip _trackLesAlarmes;
    [SerializeField]
    private  AudioClip _trackLesVoiesDorees;
    [SerializeField]
    private  AudioClip _trackSeum;
    [SerializeField]
    private  AudioClip _trackSiffle;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public AudioClip GetTrack(TracksEnum track)
    {
        switch(track)
        {
            case(TracksEnum.AliceAlice):
                return _trackAliceAlice;
            case(TracksEnum.AmiImaginaire):
                return _trackAmiImaginaire;
            case(TracksEnum.Aube):
                return _trackAube;
            case(TracksEnum.BonnesDesillusions):
                return _trackBonnesDesillusions;
            case(TracksEnum.CestRien):
                return _trackCestRien;
            case(TracksEnum.Commencement):
                return _trackCommencement;
            case(TracksEnum.LeBruit):
                return _trackLeBruit;
            case(TracksEnum.LesAlarmes):
                return _trackLesAlarmes;
            case(TracksEnum.LesVoiesDorees):
                return _trackLesVoiesDorees;
            case(TracksEnum.Seum):
                return _trackSeum;
            case(TracksEnum.Siffle):
                return _trackSiffle;
        }

        Debug.LogWarning("Issue trying to access AudioClip");
        return null;
    }
}
