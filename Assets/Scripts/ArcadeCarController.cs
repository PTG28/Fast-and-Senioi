using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ArcadeCarController : MonoBehaviour
{
    [Header("Base Driving")]
    public float acceleration = 35f;
    public float turnStrength = 120f;
    public float maxSpeed = 18f;
    public float dragOnGround = 1.5f;

    [Header("Grip / Drift")]
    public float grip = 12f;
    public float driftGrip = 3.5f;
    public float driftTurnMultiplier = 1.35f;
    public float driftAccelMultiplier = 1.1f;
    public float driftDrag = 0.6f;
    public float minSpeedToDrift = 4f;
    public float slipToShowTrails = 1.2f;

    [Header("Trails")]
    public TrailRenderer rearLeftTrail;
    public TrailRenderer rearRightTrail;

    [Header("Ground Check (for trails/sound)")]
    public Transform rearLeftPoint;
    public Transform rearRightPoint;
    public float groundCheckDistance = 0.35f;
    public LayerMask groundMask = ~0;

    [Header("Audio")]
    public AudioSource engineSource;
    public AudioClip engineLoop;
    public float engineBaseVolume = 0.25f;
    public float engineMaxVolume = 0.7f;
    public float engineMinPitch = 0.85f;
    public float engineMaxPitch = 1.6f;

    public AudioSource driftSource;
    public AudioClip driftLoop;
    public float driftMaxVolume = 0.75f;
    public float driftMinPitch = 0.9f;
    public float driftMaxPitch = 1.2f;
    public float slipForMaxDriftSound = 6f; // sideways speed that maps to max drift volume

    private Rigidbody rb;

    // values computed in FixedUpdate, applied smoothly in Update
    private float speed01;
    private float throttle01;
    private float slip01;
    private bool drifting;
    private bool groundedRear;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        SetTrails(false);

        // Setup audio
        if (engineSource && engineLoop)
        {
            engineSource.clip = engineLoop;
            engineSource.loop = true;
            if (!engineSource.isPlaying) engineSource.Play();
        }

        if (driftSource && driftLoop)
        {
            driftSource.clip = driftLoop;
            driftSource.loop = true;
            driftSource.playOnAwake = false;
            driftSource.volume = 0f;
        }
    }

    void FixedUpdate()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");
        bool driftHeld = Input.GetKey(KeyCode.Space);

        Vector3 vel = rb.linearVelocity; // if your project uses rb.linearVelocity, swap this
        Vector3 flatVel = new Vector3(vel.x, 0f, vel.z);
        float flatSpeed = flatVel.magnitude;

        // drift state
        drifting = driftHeld && flatSpeed > minSpeedToDrift;

        // forward force
        float accelMult = drifting ? driftAccelMultiplier : 1f;
        rb.AddForce(transform.forward * (v * acceleration * accelMult), ForceMode.Acceleration);

        // clamp speed
        if (flatSpeed > maxSpeed)
        {
            Vector3 limited = flatVel.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(limited.x, vel.y, limited.z);
            flatVel = limited;
            flatSpeed = maxSpeed;
        }

        // steering
        float speedFactor = Mathf.Clamp01(flatSpeed / maxSpeed);
        float turnMult = drifting ? driftTurnMultiplier : 1f;

        //float turn = h * turnStrength * turnMult * speedFactor * Time.fixedDeltaTime;
        //rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turn, 0f));
       
        // in reverse => turn to the other side
        float direction = Mathf.Sign(Vector3.Dot(flatVel, transform.forward));
        if (direction == 0) direction = 1f;

        float turn = h * turnStrength * turnMult * speedFactor * direction * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turn, 0f));

        // lateral grip
        float currentGrip = drifting ? driftGrip : grip;
        Vector3 lateralVel = transform.right * Vector3.Dot(flatVel, transform.right);
        rb.AddForce(-lateralVel * currentGrip, ForceMode.Acceleration);

        // drag feel
        rb.linearDamping = drifting ? driftDrag : dragOnGround;

        // grounded + trails
        groundedRear = RearGrounded();
        float sidewaysSpeed = Mathf.Abs(Vector3.Dot(flatVel, transform.right));
        bool showTrails = drifting && groundedRear && sidewaysSpeed > slipToShowTrails;
        SetTrails(showTrails);

        // values for audio (0..1)
        speed01 = Mathf.Clamp01(flatSpeed / maxSpeed);
        throttle01 = Mathf.Clamp01(Mathf.Abs(v));
        slip01 = Mathf.Clamp01(sidewaysSpeed / slipForMaxDriftSound);
    }

    void Update()
    {
        UpdateAudio();
    }

    void UpdateAudio()
    {
        // ENGINE (always on)
        if (engineSource && engineSource.clip)
        {
            if (!engineSource.isPlaying) engineSource.Play();

            // pitch goes with speed, volume with speed + throttle
            float targetPitch = Mathf.Lerp(engineMinPitch, engineMaxPitch, speed01);
            float targetVol = Mathf.Lerp(engineBaseVolume, engineMaxVolume, Mathf.Max(speed01, throttle01));

            engineSource.pitch = Mathf.Lerp(engineSource.pitch, targetPitch, Time.deltaTime * 8f);
            engineSource.volume = Mathf.Lerp(engineSource.volume, targetVol, Time.deltaTime * 8f);
        }

        // DRIFT (only while drifting + sliding + grounded)
        if (driftSource && driftSource.clip)
        {
            bool shouldPlay = drifting && groundedRear && slip01 > 0.05f;

            float targetVol = shouldPlay ? (slip01 * driftMaxVolume) : 0f;
            float targetPitch = Mathf.Lerp(driftMinPitch, driftMaxPitch, slip01);

            if (shouldPlay && !driftSource.isPlaying) driftSource.Play();

            driftSource.volume = Mathf.Lerp(driftSource.volume, targetVol, Time.deltaTime * 12f);
            driftSource.pitch = Mathf.Lerp(driftSource.pitch, targetPitch, Time.deltaTime * 12f);

            // stop when faded out (saves CPU and avoids faint hiss)
            if (!shouldPlay && driftSource.isPlaying && driftSource.volume < 0.01f)
                driftSource.Stop();
        }
    }

    bool RearGrounded()
    {
        if (!rearLeftPoint || !rearRightPoint) return true;

        bool left = Physics.Raycast(rearLeftPoint.position, Vector3.down, groundCheckDistance, groundMask);
        bool right = Physics.Raycast(rearRightPoint.position, Vector3.down, groundCheckDistance, groundMask);
        return left || right;
    }

    void SetTrails(bool on)
    {
        if (rearLeftTrail) rearLeftTrail.emitting = on;
        if (rearRightTrail) rearRightTrail.emitting = on;
    }

    public void ApplySpecs(float accel, float turn, float speed)
    {
        acceleration = accel;
        turnStrength = turn;
        maxSpeed = speed;
    }
}
