using UnityEngine;

public class StartForce : MonoBehaviour
{
    public Vector3 startForce;

    void Start() {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(startForce, ForceMode.Impulse);
        Physics.gravity = new Vector3(0, -1.0F, 0);
    }

    void Update() {
        KeepMovingInVerticalPlane();
    }

    void KeepMovingInVerticalPlane () {
        // There was an unexpected behavior that makes some balls boucning
        // outside of the vertical plane. We don't know why, so we implement
        // this hack, that always keeps the velocity in the vertical plane.
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (rigidbody.velocity.z != 0) {
            Vector3 newVelocity = rigidbody.velocity;
            newVelocity.z = 0;
            rigidbody.velocity = newVelocity;
        }
    }
}