using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    [SerializeField] GameObject center;
    // Start is called before the first frame update
    void Start()
    {
        
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
    }
}
