using UnityEngine;

public class StartForce : MonoBehaviour
{
    public Vector3 startForce;

    void Start()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(startForce, ForceMode.Impulse);
        Physics.gravity = new Vector3(0, -1.0F, 0);
    }
}