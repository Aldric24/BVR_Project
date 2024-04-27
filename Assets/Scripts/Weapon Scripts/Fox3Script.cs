using System;
using System.Collections;
using UnityEngine;

public class Fox3Script : Weapon
{

    // Missile Properties
    [SerializeField] private float thrustForce;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float thrustDuration;
    [SerializeField]private bool isBoosting;
    private float boostTimer = 0;
    private Rigidbody2D rb;
    [SerializeField] private ParticleSystem missileParticleEffect;
    // Target Related
    [SerializeField]private Transform target; // Set externally or retrieved from the player
    private Vector3 targetDirection;
    [SerializeField] private float raycastDistance = 2f; // Adjust as needed
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private float minVelocityThreshold = 1f;
    [SerializeField] private float baseDecelerationRate = 0.5f;
    [SerializeField] private float turningDecelerationMultiplier = 2.0f; // Increase deceleration during turns
                                                                         // Minimum velocity before destroying
    [SerializeField] private float decelerationRate = 0.5f; // Rate of velocity decrease
    SweepRotation S;
    [SerializeField] private float acceleration = 10f; // Acceleration rate
    Vector3 lastposition;
    [SerializeField] private int velocity;
    [SerializeField] private GameObject acquiredtarget;
    WeaponsManager targetinginfo;
    private Vector3 lastKnownTargetPosition;
    private Vector3 lastKnownTargetVelocity;
    private float timeOfLastLock;
    [SerializeField] float radarAngle = 30f;  // Half the angle of the radar cone
    [SerializeField] float radarRange = 20f;
    
    [SerializeField]Collider2D radar;
    void Start()
    {
        weaponName = "Amraam Aim120 -D";
        boostTimer = 0;
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        

        if (rb.simulated!=false)
        {
            velocity=((int)rb.velocity.magnitude);
            guidance();
            UpdateParticleEffect();

            if (isBoosting)
            {
                BoostPhase();
            }
            else
            {
                InertialPhase();
            }
            CheckRaycastCollision();

        }
        
    }
    private void guidance()
    {
        if (target != null)
        {
            // Calculate target velocity (assuming target has a Rigidbody2D component)
            Vector3 targetVelocity = target.GetComponent<Rigidbody2D>().velocity;

            // Use first-order intercept to predict target position
            float timeToIntercept = Vector3.Distance(transform.position, target.position) / maxSpeed;
            Vector3 predictedTargetPosition = target.position + (targetVelocity * timeToIntercept);

            // Update target direction based on predicted position
            targetDirection = (predictedTargetPosition - transform.position).normalized;
        }
        else
        {
            targetDirection = transform.up;
        }
    }
    void BoostPhase()
    {
        AlignWithVelocity();

        // Accelerate towards maxSpeed
        rb.velocity = Vector2.MoveTowards(rb.velocity, transform.up * maxSpeed, acceleration * Time.fixedDeltaTime);

        boostTimer += Time.fixedDeltaTime;
        if (boostTimer >= thrustDuration)
        {
            isBoosting = false;
            boostTimer = 0; // Reset the timer
        }
        if (!missileParticleEffect.isPlaying)
        {
            missileParticleEffect.Play();
        }
    }

    void InertialPhase()
    {
        AlignWithVelocity();
        // Determine Angular Velocity (how fast the missile is turning)
        float angularVelocityMagnitude = Mathf.Abs(rb.angularVelocity);

        // Calculate Dynamic Deceleration Rate
        float decelerationRate = baseDecelerationRate;
        if (angularVelocityMagnitude > 10f) // Adjust this threshold as needed
        {
            decelerationRate *= turningDecelerationMultiplier;
        }
        if (missileParticleEffect.isPlaying)
        {
            missileParticleEffect.Stop();
        }
        // Adjust velocity based on dynamic deceleration
        rb.velocity = transform.up * (rb.velocity.magnitude - decelerationRate * Time.fixedDeltaTime);

        // Self-destruct Logic
        //if (rb.velocity.magnitude < minVelocityThreshold)
        //{
        //    Destroy(gameObject);
        //}
    }
    void OnTriggerEnter2D(Collider2D radar)
    {
        if (radar.gameObject.CompareTag("Adversary"))
        {
            acquiredtarget = radar.gameObject;
            target = radar.transform;
        }
        

    }
    void OnTriggerStay2D(Collider2D radar)
    {
        if (radar.gameObject.CompareTag("Adversary"))
        {
            acquiredtarget = radar.gameObject;
            target = radar.transform;
        }
        
        
    }
    void OnTriggerExit2D(Collider2D radar)
    {

        if (radar.gameObject.CompareTag("Adversary"))
        {
            target = null;
        }

    }
    bool RadarSweep()
    {
        radar.enabled = true;
        
        //get collision info from the collider
       
        //int numRays = 5; // Number of rays within the cone
        //float angleIncrement = radarAngle / numRays;

        //for (float angle = -radarAngle; angle <= radarAngle; angle += angleIncrement)
        //{
        //    Vector3 rayDir = Quaternion.Euler(0, 0, angle) * transform.up;
        //    RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDir, radarRange);

        //    // Customize what qualifies as a "detected" target
        //    if (CheckValidCollision(hit.collider) && hit.collider.CompareTag("Adversary"))
        //    {
        //        Debug.Log("missile radar sees something");
        //        target = hit.transform; // Assign the detected object as the target
        //        return true;
        //    }
        //}
        return false; // No target detected
    }
    IEnumerator checkTargetinginfo(WeaponsManager info)
    {
        while (true)
        {
            if (info.target == null)
            {
                target = null;
                float timeSinceLastLock = Time.time - timeOfLastLock;
                Vector3 estimatedPosition = lastKnownTargetPosition + (lastKnownTargetVelocity * timeSinceLastLock);
                targetDirection = (estimatedPosition - transform.position).normalized;
                RadarSweep();
            }
            else if (info.target != null)
            {
                target = info.target.transform;
                lastKnownTargetPosition = target.position;
                lastKnownTargetVelocity = target.GetComponent<Rigidbody2D>().velocity;
            }
            else // info.target is null but we have past data
            {
                // Estimate new target position based on previous velocity
                float timeSinceLastLock = Time.time - timeOfLastLock;
                Vector3 estimatedPosition = lastKnownTargetPosition + (lastKnownTargetVelocity * timeSinceLastLock);
                targetDirection = (estimatedPosition - transform.position).normalized;
                RadarSweep();
            }

            yield return null;
        }
        
        

       
        
    }
    bool CheckValidCollision(Collider2D collider )
    {
        if (collider == null) return false;

        if (collider.gameObject.CompareTag("Player")) return false; // Ignore player collisions

        return collisionMask.value == (collisionMask.value | (1 << collider.gameObject.layer));
    }
    bool CheckRaycastCollision()
    {
        // Continuous Raycast Detection (adjust numRays and offset as needed)
        int numRays = 3;
        float rayOffset = 0.5f;
        for (int i = 0; i < numRays; i++)
        {
            Vector3 rayDir = transform.up + (transform.right * (rayOffset * (i - 1)));
            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDir, maxSpeed * Time.fixedDeltaTime);
            if (CheckValidCollision(hit.collider))
            {
                Debug.Log("Hit something with Raycast!");
                OnMissileHit(hit.collider.gameObject);
                return true; // Collision detected!
            }
        }

        // SphereCast Overlap Detection (adjust radius as needed)
        float overlapRadius = 0.75f;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, overlapRadius, collisionMask);
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != gameObject && CheckValidCollision(collider)) // Avoid self-collision
            {
                Debug.Log("Hit something with Overlap!");
                OnMissileHit(collider.gameObject);
                return true; //  Collision detected! 
            }
        }

        return false; // No collision detected
    }
    void OnMissileHit(GameObject objectHit)
    {
      Destroy(objectHit);
      FindAnyObjectByType<HUD_Text>().SplashText(objectHit.name);
    }
    void AlignWithVelocity()
    {

        if (rb.velocity != Vector2.zero) // Check if we have any velocity
        {
            float rotateAmount = Vector3.Cross(targetDirection, transform.up).z;
            rb.angularVelocity = -rotateAmount * 200;

        }
    }
   
    void UpdateParticleEffect()
    {
        if (rb.velocity != Vector2.zero) // Check if we have any velocity
        {
            if (!missileParticleEffect.isPlaying)
            {
                missileParticleEffect.Play();
            }
        }
        else
        {
            if (missileParticleEffect.isPlaying)
            {
                missileParticleEffect.Stop();
            }
        }
    }

    internal void fire(WeaponsManager parent)
    {
        if(parent!=null)
        {
            StartCoroutine(checkTargetinginfo(parent));
        }
        transform.parent = null;
        gameObject.GetComponent<Rigidbody2D>().simulated=true;
       
    }
}
