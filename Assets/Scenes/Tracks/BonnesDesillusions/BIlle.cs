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
        Rotate();
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

    void Rotate () {
        const float speed = 0.01F;
        var perlinX = Time.time * speed;
        var perlinY = 0.5F * (this.transform.position.x + this.transform.position.y) * speed;
        var angle = 50.0F * (Mathf.PerlinNoise(perlinX, perlinY) - 0.5F);
        transform.Rotate(new Vector3(0, 0, angle));
    }
}