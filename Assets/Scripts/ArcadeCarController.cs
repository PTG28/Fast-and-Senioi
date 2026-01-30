using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ArcadeCarController : MonoBehaviour
{
    public float acceleration = 35f;
    public float turnStrength = 120f;
    public float maxSpeed = 18f;
    public float dragOnGround = 1.5f;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void FixedUpdate()
    {
        float v = Input.GetAxis("Vertical");   // W/S or Up/Down
        float h = Input.GetAxis("Horizontal"); // A/D or Left/Right

        // Forward force
        rb.AddForce(transform.forward * (v * acceleration), ForceMode.Acceleration);

        // Clamp flat speed
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatVel.magnitude > maxSpeed)
        {
            Vector3 limited = flatVel.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(limited.x, rb.linearVelocity.y, limited.z);
        }

        // Turn more when moving
        float speedFactor = Mathf.Clamp01(flatVel.magnitude / maxSpeed);
        float turn = h * turnStrength * speedFactor * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turn, 0f));

        // Simple “ground feel”
        rb.linearDamping = dragOnGround;
    }
}
