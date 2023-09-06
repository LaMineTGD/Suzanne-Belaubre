using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using extOSC;


public class Background : MonoBehaviour
{
    float outroProgress = -1;

    void Start() {
        generateOSCReceveier();
    }

    private void generateOSCReceveier()
    {
        ShowManager.m_Instance.OSCReceiver.Bind("/BD/start_couplet", StartCouplet);
        ShowManager.m_Instance.OSCReceiver.Bind("/BD/start_refrain", StartRefrain);
        ShowManager.m_Instance.OSCReceiver.Bind("/BD/start_fire", StartFire);
        ShowManager.m_Instance.OSCReceiver.Bind("/BD/start_small_fire", StartSmallFire);
        ShowManager.m_Instance.OSCReceiver.Bind("/BD/start_small_fire2", StartSmallFire2);
        ShowManager.m_Instance.OSCReceiver.Bind("/BD/outro", StartOutro);
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
        material.SetFloat("_Speed", 0.1F);
        material.SetFloat("_FuzzyGrain", 80.0F);
        material.SetFloat("_PixelizeFlag", 0);

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
        material.SetFloat("_Speed", 0.1F);
        material.SetFloat("_FuzzyGrain", 800.0F);
        material.SetFloat("_PixelizeFlag", 0);

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

    public void StartFire(OSCMessage message)
    {
        Debug.Log("StartFire");
        StartFire();
    }

    public void StartFire()
    {
        var renderer = GetComponent<Renderer>();
        var material = renderer.material;
        material.SetFloat("_Speed", 0.9F);
        material.SetFloat("_FuzzyGrain", 800.0F);
        material.SetFloat("_PixelizeFlag", 1);

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

    public void StartSmallFire(OSCMessage message)
    {
        Debug.Log("StartSmallFire");
        StartSmallFire();
    }

    public void StartSmallFire()
    {
        StartCoroutine(AnimateNoiseScale(2.8f, 1f));
    }

    public void StartSmallFire2(OSCMessage message)
    {
        Debug.Log("StartSmallFire2");
        StartSmallFire2();
    }

    public void StartSmallFire2()
    {
        StartCoroutine(AnimateNoiseScale(1.3f, 0.2f));
    }

    private IEnumerator AnimateNoiseScale(float duration, float amplitude)
    {
        float elapsedTime = 0f;
        float baseNoiseScale = 1.5f;

        var renderer = GetComponent<Renderer>();
        var material = renderer.material;

        while(elapsedTime < duration)
        {
            material.SetFloat(
                "_Noise_Scale",
                baseNoiseScale + amplitude * Mathf.Sin(
                    Mathf.Lerp(0.0f, Mathf.PI, elapsedTime / duration)
                )
            );
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        // Be sure to leave with the base value
        material.SetFloat("_Noise_Scale", baseNoiseScale);
        yield return null;
    }

    public void StartOutro(OSCMessage message)
    {
        Debug.Log("StartOutro");
        StartOutro();
    }

    public void StartOutro()
    {
        outroProgress = 0;
    }

    void Update() {
        if (outroProgress < 1F && outroProgress >= 0F){
            outroProgress += Time.deltaTime * 0.1F;

            var angle = 0.6F * Time.deltaTime / 0.03F;
            if (transform.rotation.x < 0.1) {
                transform.Rotate(new Vector3(angle, 0, 0));
            }
        }
    }

}