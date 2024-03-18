using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.UI;

public class NewControl: MonoBehaviour
{
    [SerializeField] private float thrustForce;
    [SerializeField] private float maxSpeedKnots;
    [SerializeField] private float dragCoefficient;
    [SerializeField] private Slider throttleSlider;
    [SerializeField] public Text speedText;
    [SerializeField] private float thrustTransitionTime = 0.5f; // Time in seconds
    private float currentThrust; //
    private Rigidbody2D rb;
    public float speedKnots;
    private const float KNOTS_TO_MS_CONVERSION = 5.4444f;
    [SerializeField] private float rotationSensitivity;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Input.gyro.enabled = true;
    }

    void FixedUpdate()
    {
        HandleRotation();
        ApplySteeringForce(); // Replace ApplyThrust
        LimitMaxSpeed();
        HandleDrag();
        UpdateSpeedDisplay();
    }

    void ApplyThrust()
    {
        float targetThrust = throttleSlider.value * thrustForce;

        // Smoothly transition thrust
        currentThrust = Mathf.MoveTowards(currentThrust, targetThrust, Time.fixedDeltaTime / thrustTransitionTime);

        Vector2 direction = -GetDirectionFromRotation();
        rb.AddForce(direction * currentThrust);
    }

    void HandleDrag()
    {
        if (throttleSlider.value == 0 && speedKnots > 0)
        {
            speedKnots -= dragCoefficient * Time.fixedDeltaTime;
            speedKnots = Mathf.Max(0, speedKnots);
        }

        speedKnots = Mathf.Min(speedKnots, maxSpeedKnots);
    }
    Vector2 GetDirectionFromRotation()
    {
        float baseRotationRadians = transform.eulerAngles.z * Mathf.Deg2Rad;
        float offsetRotationRadians = baseRotationRadians + Mathf.PI; // 180 degrees in radians

        return new Vector2(Mathf.Sin(offsetRotationRadians), Mathf.Cos(offsetRotationRadians));
    }
    void ApplySteeringForce()
    {
        
        float targetThrust = throttleSlider.value * thrustForce;
        currentThrust = Mathf.MoveTowards(currentThrust, targetThrust, Time.fixedDeltaTime / thrustTransitionTime);

        Vector2 desiredDirection = GetDirectionFromRotation(); // Get the direction the aircraft is pointed
        Vector2 steeringForce = -desiredDirection * currentThrust;

        // Optionally limit the steering force magnitude here if needed

        rb.AddForce(steeringForce);
        
    }
    void UpdateSpeedDisplay()
    {
        float speedMS = rb.velocity.magnitude;

        // Convert meters per second to knots
        speedKnots = speedMS * KNOTS_TO_MS_CONVERSION;

    }
    void HandleRotation()
    {
        float tiltAroundX = Input.acceleration.x * rotationSensitivity;

        transform.Rotate(0, 0, tiltAroundX);
        // Y-axis Rotation (New Logic)
        //float tiltAroundY = Input.acceleration.y * rotationSensitivity;
        //float targetRotationY = Mathf.Lerp(120.0f, 240.0f, (tiltAroundY + 1.0f) / 2.0f);

        //transform.localEulerAngles = new Vector3(0, targetRotationY, transform.localEulerAngles.z);
    }
    void LimitMaxSpeed()
    {
        // Calculate current speed in meters per second
        float speedMS = rb.velocity.magnitude;
        speedText.text = "Speed: " + speedKnots.ToString("F1") + " Knots";
        // If exceeding max speed, limit velocity
        if (speedMS > maxSpeedKnots * KNOTS_TO_MS_CONVERSION)
        {
            Vector2 normalizedVelocity = rb.velocity.normalized;
            rb.velocity = normalizedVelocity * (maxSpeedKnots * KNOTS_TO_MS_CONVERSION);
        }
    }
}