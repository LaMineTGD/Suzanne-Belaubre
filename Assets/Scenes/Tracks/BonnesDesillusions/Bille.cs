using UnityEngine;
using extOSC;


public class Bille : MonoBehaviour
{
    public Vector3 startForce;
    public float rotationSpeed = 0.01F;
    public float orbitSpeed = 15F;
    public float orbitAltitude = 5F;
    [SerializeField] GameObject center;


    Vector3 positionBeforeMovingToOrbit;
    Vector3 orbitStartPosition;
    float timeToReachOrbit;
    float movingToOrbitProgress;
    bool movingToOrbit = false;

    float outroProgress = -1;

    void Start() {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(startForce, ForceMode.Impulse);
        Physics.gravity = new Vector3(0, -1.0F, 0);
        generateOSCReceveier();
    }

    private void generateOSCReceveier()
    {
        ShowManager.m_Instance.OSCReceiver.Bind("/start_orbit", StartOrbit);
        ShowManager.m_Instance.OSCReceiver.Bind("/start_bouncing", StartBouncing);
        ShowManager.m_Instance.OSCReceiver.Bind("/BD/outro", StartOutro);
    }

    public void StartOrbit(OSCMessage message)
    {
        StartOrbit();
    }

    public void StartBouncing(OSCMessage message)
    {
        StartBouncing();
    }

    public void StartBouncing()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        rigidbody.AddForce(startForce, ForceMode.Impulse);
    }

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }

    public void StartOrbit()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;

        positionBeforeMovingToOrbit = transform.position;
        orbitStartPosition = center.transform.position;
        orbitStartPosition.y += orbitAltitude;
        var angle = Random.Range(0F, 360F);
        orbitStartPosition = RotatePointAroundPivot(
            orbitStartPosition,
            center.transform.position,
            new Vector3(0, 0, angle)
        );
        timeToReachOrbit = 1.5F;
        movingToOrbitProgress = 0F;
        movingToOrbit = true;
    }

    void MaybeMoveToOrbit() {
        if (movingToOrbit) {
            movingToOrbitProgress += Time.deltaTime/timeToReachOrbit;
            transform.position = Vector3.Lerp(
                positionBeforeMovingToOrbit,
                orbitStartPosition,
                movingToOrbitProgress
            );
            if (movingToOrbitProgress > 1) {
                movingToOrbit = false;
            }
        }
    }

    void Update() {
        KeepMovingInVerticalPlane();
        Rotate();
        // We choose to keep orbit movements even for the bouncing phase. This
        // will add some non linear moving to the bouncing movements.
        Orbit();
        MaybeMoveToOrbit();
        if (outroProgress < 1 && outroProgress >= 0){
            outroProgress += Time.deltaTime * 0.2F;

            var step = -0.1F;
            transform.position = Vector3.MoveTowards(
                transform.position, center.transform.position, step
            );
        }
    }

    public void StartOutro(OSCMessage message)
    {
        Debug.Log("StartOutro");
        StartOutro();
    }

    public void StartOutro()
    {
        outroProgress = 0;
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
    }

    void KeepMovingInVerticalPlane () {
        // There was an unexpected behavior that makes some balls bouncing
        // outside of the vertical plane. We don't know why. So we implement
        // this hack, that always keeps the velocity in the vertical plane (i.e.
        // forcing velocity.z to 0).
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (rigidbody.velocity.z != 0) {
            Vector3 newVelocity = rigidbody.velocity;
            newVelocity.z = 0;
            rigidbody.velocity = newVelocity;
        }
    }

    void Rotate () {
        var perlinX = Time.time * rotationSpeed;
        var perlinY = 0.5F * (this.transform.position.x + this.transform.position.y) * rotationSpeed;
        var angle = 50.0F * (Mathf.PerlinNoise(perlinX, perlinY) - 0.5F);
        transform.Rotate(new Vector3(0, 0, angle));
    }

    void Orbit () {
        this.transform.RotateAround(
            center.transform.position,
            Vector3.forward,
            orbitSpeed * Time.deltaTime
        );
    }
}