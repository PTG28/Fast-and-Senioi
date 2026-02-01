using UnityEngine;

public class WheelVisuals : MonoBehaviour
{
    public Rigidbody rb;

    [Header("Wheel transforms")]
    public Transform frontLeft;
    public Transform frontRight;
    public Transform rearLeft;
    public Transform rearRight;

    [Header("Tuning")]
    public float wheelRadius = 0.35f;   // adjust to match your model
    public float maxSteerAngle = 30f;

    float spinAngle;

    void Awake()
    {
        if (!rb) rb = GetComponentInParent<Rigidbody>();
    }

    void Update()
    {
        if (!rb) return;

        // speed along the car's forward direction
        float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);

        float wheelCircumference = 2f * Mathf.PI * Mathf.Max(0.01f, wheelRadius);
        float spinDegPerSec = (forwardSpeed / wheelCircumference) * 360f;
        spinAngle += spinDegPerSec * Time.deltaTime;

        float steerInput = Input.GetAxis("Horizontal");
        float steerAngle = steerInput * maxSteerAngle;

        ApplyWheel(frontLeft, steerAngle, spinAngle);
        ApplyWheel(frontRight, steerAngle, spinAngle);
        ApplyWheel(rearLeft, 0f, spinAngle);
        ApplyWheel(rearRight, 0f, spinAngle);
    }

    void ApplyWheel(Transform wheel, float steerAngle, float spinAngle)
    {
        if (!wheel) return;

        // Typical setup: steer around Y, spin around X
        Quaternion steer = Quaternion.Euler(0f, steerAngle, 0f);
        Quaternion spin = Quaternion.Euler(spinAngle, 0f, 0f);
        wheel.localRotation = steer * spin;
    }
}
