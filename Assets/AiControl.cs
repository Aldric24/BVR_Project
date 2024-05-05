using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiControl : MonoBehaviour
{
    // Movement Parameters
    [SerializeField] private float thrustForce;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float maxBoostDuration;
    [SerializeField] private float turningDecelerationMultiplier;
    [SerializeField] private float basedecelerationrate;
    RWR rwr;
    public int targetSpeed;
    [SerializeField] int Currentspeed;
    // Target Tracking
    public Transform target;
    public Vector3 targetDirection;

    // State Management
    public  bool isBoosting = false;
    private float boostTimer = 0f;

    // Internal References
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        
        Currentspeed= (int)rb.velocity.magnitude;
        UpdateTargetDirection();
        
        ManageThrust(targetSpeed);
    }

    // Update heading direction (from Fox3Script)
    void UpdateTargetDirection()
    {
        if (target != null)
        {
            targetDirection = (target.position - transform.position).normalized;
            AlignWithVelocity();
        }
        else
        {
            targetDirection = transform.up; // Default to forward direction
        }
    }

    // Rotation Logic (similar to Fox3Script's AlignWithVelocity)
    void AlignWithVelocity()
    {
        if (rb.velocity != Vector2.zero)
        {
            
            
            float rotateAmount = Vector3.Cross(targetDirection, transform.up).z;
            rb.angularVelocity = -rotateAmount * 200;

            
        }
    }
    void InertialPhase()
    {

        AlignWithVelocity();
        // Determine Angular Velocity (how fast the missile is turning)
        float angularVelocityMagnitude = Mathf.Abs(rb.angularVelocity);

        // Calculate Dynamic Deceleration Rate
        float decelerationRate = basedecelerationrate;
        if (angularVelocityMagnitude > 10f) // Adjust this threshold as needed
        {
            decelerationRate *= turningDecelerationMultiplier;
        }
        
        // Adjust velocity based on dynamic deceleration
        rb.velocity = transform.up * (rb.velocity.magnitude - decelerationRate * Time.fixedDeltaTime);

        //Self - destruct Logic
        
    }

    // Thrust Management (inspired by Fox3Script phases)
    public void ManageThrust(int velocity)
    {
        // Boost Logic
        rb.velocity = Vector2.MoveTowards(rb.velocity, transform.up * velocity, thrustForce * Time.fixedDeltaTime);
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Adversary Radar"))
        {
            rwr.Popup(collision.gameObject.transform.parent.gameObject);
        }


    }
}
