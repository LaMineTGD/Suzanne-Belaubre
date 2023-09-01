using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCenter : MonoBehaviour
{
    void Start()
    {   
    }

    public float degreesPerSecond = 10;
    private void Update()
    {
        transform.Rotate(new Vector3(0, 0, -degreesPerSecond) * Time.deltaTime);
        float scale = Mathf.Sin((float) Time.time*2f)*0.0005f;
        transform.localScale += new Vector3(scale, scale, scale);
    }
}
