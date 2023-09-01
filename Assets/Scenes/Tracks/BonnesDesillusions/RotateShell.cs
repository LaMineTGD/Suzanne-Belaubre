using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateShell : MonoBehaviour
{
    void Start()
    {   
    }

    public float degreesPerSecond = 20;
    private void Update()
    {
        transform.Rotate(new Vector3(0, 0, degreesPerSecond) * Time.deltaTime);
    }
}
