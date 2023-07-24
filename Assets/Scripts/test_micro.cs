using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_micro : MonoBehaviour
{
    [SerializeField] private FFTWindow _fftWindow = FFTWindow.Rectangular;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Mesh _mesh;
    [SerializeField] private Material _mat1;
    [SerializeField] private Material _mat2;
    private int _sampleSize;
    private float[] _samples_1;

    private float[] _samples_2;
    private GameObject[] _go1;
    private GameObject[] _go2;
    // Start is called before the first frame update

    private void Awake()
    {
        _sampleSize = 512;
        _samples_1 = new float[_sampleSize];
        _samples_2 = new float[_sampleSize];
        _go1 = new GameObject[_sampleSize];
        _go2 = new GameObject[_sampleSize];
        _audioSource = GetComponent<AudioSource>();
    }

    private GameObject generate_GO(Material material)
    {
        GameObject go = new GameObject();
        MeshFilter meshFilter = go.AddComponent<MeshFilter>();
        meshFilter.mesh = _mesh;
        MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
        meshRenderer.material = material;
        return go;
    }


    void Start()
    {
        foreach (var device in Microphone.devices)
        {
            Debug.Log("Name: " + device);
        }
        //_audioSource.clip = Microphone.Start("Microphone sur casque (3- Arctis 7 Chat)", true,10,44100);
        _audioSource.clip = Microphone.Start("Line 1/2 (M-Audio Fast Track Pro)", true, 10, 44100);
        _audioSource.loop = true;
        while (!(Microphone.GetPosition(null) > 0)) { }
        _audioSource.Play();
        Debug.Log("Name: " + _audioSource.clip.channels);
        GameObject go = new GameObject("Test");
        for (int i = 0; i < _sampleSize; i++)
        {
            _go1[i] = generate_GO(_mat1);
            _go1[i].transform.SetParent(go.transform);
            _go1[i].transform.localScale = new(1f, 1f, 1f);

            _go2[i] = generate_GO(_mat2);
            _go2[i].transform.SetParent(go.transform);
            _go2[i].transform.localScale = new(1f, 1f, 1f);
        }

    }

    // Update is called once per frame
    void Update()
    {
        _audioSource.GetSpectrumData(_samples_1, 0, _fftWindow);
        _audioSource.GetSpectrumData(_samples_2, 1, _fftWindow);
        for (int i = 0; i < _sampleSize; i++)
        {
            //_go[i].transform.position= new Vector3(i*0.01f,_samples[i]*100,0.0f);
            _go1[i].transform.position = new Vector3(i, Mathf.Log(_samples_1[i] + 0.0001f), 0.0f);

            _go2[i].transform.position = new Vector3(i, 220f - Mathf.Log(_samples_2[i] + 0.0001f), 0.0f);
        }
    }
}
