using UnityEngine;

// Le script AudioSpectrum analyse le spectre audio d'un composant AudioSource attaché.

[RequireComponent(typeof(AudioSource))]
public class AudioSpectrum : MonoBehaviour
{
    [SerializeField] private FFTWindow _fftWindow = FFTWindow.Hamming; // Le type de fenêtre FFT utilisé pour l'analyse du spectre. Hanning ou Hamming semble suffisant pour traiter des signaux propres

    private float[] _samples; // Tableau pour stocker les données du spectre audio.
    private AudioSource _audioSource; 
    private int _sampleSize; // La taille du spectre audio. Puissance de 2.

    private void Awake()
    {
        _sampleSize = 512; 
        _samples = new float[_sampleSize];

        _audioSource = GetComponent<AudioSource>(); 
    }

    private void Update()
    {
        _audioSource.GetSpectrumData(_samples, 0, _fftWindow); // Récupérer les données du spectre audio dans le tableau _samples.
    }

    // Renvoie les données du spectre audio.
    public float[] GetSpectrum()
    {
        return _samples;
    }

    // Renvoie la taille du spectre audio.
    public int GetSamplesSize()
    {
        return _sampleSize;
    }
}

