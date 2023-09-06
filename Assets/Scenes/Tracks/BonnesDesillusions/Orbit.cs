using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC;

public class Orbit : MonoBehaviour
{
    [SerializeField] GameObject center;
    // Start is called before the first frame update
    float outroProgress = -1;

    void Start()
    {
        generateOSCReceveier();        
    }

    private void generateOSCReceveier()
    {
        ShowManager.m_Instance.OSCReceiver.Bind("/BD/outro", StartOutro);
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

    // Update is called once per frame
    void Update()
    {
        this.transform.RotateAround(
            this.transform.position, Vector3.up, 50f * Time.deltaTime
        );

        // var center = GameObject.Find("Center Sphere");
        this.transform.RotateAround(
            center.transform.position, Vector3.forward, 20f * Time.deltaTime
        );

        var step = Mathf.Sin((float) Time.time*0.5f)*0.0015f;
        transform.position = Vector3.MoveTowards(
            transform.position, center.transform.position, step
        );

        if (outroProgress < 1 && outroProgress >= 0){
            outroProgress += Time.deltaTime * 0.2F;

            var outroStep = -0.1F * Time.deltaTime / 0.03F;
            transform.position = Vector3.MoveTowards(
                transform.position, center.transform.position, outroStep
            );

            // Also do the outro for the center.
            center.transform.localScale *= 0.97F;
        }
    }
}
