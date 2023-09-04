using UnityEngine;
using UnityEngine.Rendering;
using extOSC;


public class Background : MonoBehaviour
{
    void Start() {
        generateOSCReceveier();
    }

    private void generateOSCReceveier()
    {
        ShowManager.m_Instance.OSCReceiver.Bind("/BD/start_couplet", StartCouplet);
        ShowManager.m_Instance.OSCReceiver.Bind("/BD/start_refrain", StartRefrain);
    }

    public void StartCouplet(OSCMessage message)
    {
        Debug.Log("StartCouplet");
        StartCouplet();
    }

    public void StartCouplet()
    {
        var renderer = GetComponent<Renderer>();
        var material = renderer.material;
        material.SetFloat("_FuzzyGrain", 80.0F);

        var shader = material.shader;
        material.SetKeyword(
            new LocalKeyword(shader, "_PALETTE_REFRAIN"),
            false
        );
        material.SetKeyword(
            new LocalKeyword(shader, "_PALETTE_COUPLET"),
            true
        );
    }

    public void StartRefrain(OSCMessage message)
    {
        Debug.Log("StartRefrain");
        StartRefrain();
    }

    public void StartRefrain()
    {
        var renderer = GetComponent<Renderer>();
        var material = renderer.material;
        material.SetFloat("_FuzzyGrain", 800.0F);

        var shader = material.shader;
        material.SetKeyword(
            new LocalKeyword(shader, "_PALETTE_REFRAIN"),
            true
        );
        material.SetKeyword(
            new LocalKeyword(shader, "_PALETTE_COUPLET"),
            false
        );
    }


    void Update() {
    }

}