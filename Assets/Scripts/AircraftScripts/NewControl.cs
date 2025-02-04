using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NewControl: MonoBehaviour
{
    [SerializeField] private float thrustForce;
    [SerializeField] private float maxSpeedKnots;
    [SerializeField] private float dragCoefficient;
    [SerializeField] public Slider throttleSlider;
    [SerializeField] public Text speedText;
    [SerializeField] private float thrustTransitionTime = 0.5f; // Time in seconds
    [SerializeField] private float optimalTurningSpeed; // Speed in knots
    [SerializeField] private float maxTurnSensitivityLoss; // Percentage (0 to 1)
    [SerializeField] private float  someFactor = 0.05f;
    [SerializeField] private RWR rwr;
    private float currentThrust; //
    [SerializeField] private Rigidbody2D rb;
    public float speedKnots;
    private const float KNOTS_TO_MS_CONVERSION = 5.4444f;
    [SerializeField] private float rotationSensitivity;
    public HeatSource HeatSource;
    
    [SerializeField] private float heatBuildUpRate = 0.1f; // Per second
    [SerializeField] private float heatCoolDownRate = 0.05f; // Per second
    [SerializeField] private float minHeatIntensity = 0.2f;
    [SerializeField] private float maxHeatIntensity = 0.8f;
    public int velocity;
    [SerializeField] GameObject Ab1;
    [SerializeField] GameObject Ab2;
    void Start()
    {
        throttleSlider = FindAnyObjectByType<Slider>();
        throttleSlider.value = 0.5f;
        //rb = GetComponent<Rigidbody2D>();
        Input.gyro.enabled = true;
        
    }

    void FixedUpdate()
    {
        Ab();
        RegulateHeatIntensity();
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
        //Debug.Log("Steering Force Magnitude: " + steeringForce.magnitude);  
        // Optionally limit the steering force magnitude here if needed

        rb.AddForce(steeringForce);
        //rb.velocity = transform.up * thrustForce;

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

        // Calculate sensitivity based on speed
        float normalizedSpeed = Mathf.Clamp01(speedKnots / maxSpeedKnots);
        float turnSensitivityFactor = normalizedSpeed * 0.7f;

        float adjustedRotationSensitivity = rotationSensitivity * turnSensitivityFactor;
        transform.Rotate(0, 0, tiltAroundX * adjustedRotationSensitivity);
        //float tiltAroundX = Input.acceleration.x * rotationSensitivity;

        //float speedDifference = Mathf.Abs(speedKnots - optimalTurningSpeed);
        //float turnLossFactor = Mathf.Clamp01(speedDifference * someFactor); // You'll need to tune 'someFactor'
        //float turnSensitivityFactor = 1.0f - (maxTurnSensitivityLoss * 0.01f * turnLossFactor);

        //float adjustedRotationSensitivity = rotationSensitivity * turnSensitivityFactor;
        //Debug.Log("Rotation Sensitivity: " + rotationSensitivity);
        //Debug.Log("Turn Sensitivity Factor: " + turnSensitivityFactor);
        //transform.Rotate(0, 0, -tiltAroundX * adjustedRotationSensitivity);

    }
    void LimitMaxSpeed()
    {
        velocity = (int)rb.velocity.magnitude;
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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.CompareTag("Adversary Radar"))
        {
            rwr.Popup(collision.gameObject.transform.parent.gameObject);
        }
        else if(collision.gameObject.CompareTag("AdversaryMissile"))
        {
            rwr.Popup(collision.gameObject.transform.parent.gameObject);
        }
        else if (collision.gameObject.CompareTag("Missile"))
        {
            rwr.Popup(collision.gameObject.transform.parent.gameObject);
        }




    }
    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Adversary Radar"))
        {
            rwr.Popup(collision.gameObject.transform.parent.gameObject);
        }
        else if (collision.gameObject.CompareTag("AdversaryMissile"))
        {
            rwr.Popup(collision.gameObject.transform.parent.gameObject);
        }
        else if (collision.gameObject.CompareTag("Missile"))
        {
            rwr.Popup(collision.gameObject.transform.parent.gameObject);
        }

    }
   void Ab()
   {

        
        if (throttleSlider.value == throttleSlider.maxValue)
        {
            Ab1.SetActive(true);
            Ab2.SetActive(true);
                
        }
        else
        {
            Ab1.SetActive(false);
            Ab2.SetActive(false);
                
        }
           
        
   }

    void RegulateHeatIntensity()
    {
        // Increase heat based on thrust
        if (throttleSlider.value > 0)
        {
            HeatSource.heatIntensity += heatBuildUpRate * Time.fixedDeltaTime;
        }
        else
        {
            HeatSource.heatIntensity -= heatCoolDownRate * Time.fixedDeltaTime;
        }

        // Clamp heat intensity within the desired range
        HeatSource.heatIntensity = Mathf.Clamp(HeatSource.heatIntensity, minHeatIntensity, maxHeatIntensity);

      
    }

}
