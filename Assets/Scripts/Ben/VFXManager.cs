using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

// Le script VFXManager est responsable de la gestion des  paramètres VFX.

public class VFXManager : MonoBehaviour
{
    private const string CONFORM_SPHERE_RADIUS = "ConformSphereRadius"; // Nom de la propriété exposée pour le rayon de la sphère de conformité.
    private const string CONFORM_FIELD_FORCE = "ConformFieldForce"; // Nom de la propriété exposée pour la force du champ de conformité.

    [SerializeField] private AudioSpectrum _audioSpectrum; 

    [Header("Exposed Properties Settings")]
    [SerializeField] private int _radiusAmplitude = 75; // Amplitude du rayon de la sphère de conformité.
    [SerializeField] private int _fieldAmplitude = 100000; // Amplitude de la force du champ de conformité.
    [SerializeField] private int _spectrumNumber = 300; // Indice du spectre audio utilisé pour la force du champ de conformité.

    private VisualEffect _visualEffect; 
    private float[] _spectrum; 

    private void Awake()
    {
        _visualEffect = GetComponent<VisualEffect>(); 
    }

    private void Start()
    {
        _spectrum = _audioSpectrum.GetSpectrum(); // Récupérer les données du spectre audio à partir du script AudioSpectrum.
    }

    private void Update()
    {
        // Mettre à jour les propriétés exposées de l'effet visuel (VFX) en fonction des données du spectre audio.
        _visualEffect.SetFloat(CONFORM_SPHERE_RADIUS, _spectrum[0] * _radiusAmplitude);
        _visualEffect.SetFloat(CONFORM_FIELD_FORCE, _spectrum[_spectrumNumber] * _fieldAmplitude);
    }
}
