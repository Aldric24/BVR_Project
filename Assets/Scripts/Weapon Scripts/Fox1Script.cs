using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fox1Script : Weapon
{
    // Missile Properties
    [SerializeField] private float thrustForce;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float thrustDuration;
    [SerializeField] private bool isBoosting;
    private float boostTimer = 0;
    private Rigidbody2D rb;
    [SerializeField] private ParticleSystem missileParticleEffect;

    // Target Related
    [SerializeField] private Transform target; // Set externally or retrieved from the player
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
    bool lockestablished = false;
    void Start()
    {
        if (gameObject.transform.parent.tag == "Player")
        {
            gameObject.tag = "PlayerMissile";
        }
        weaponName = "Fox1 - AIM 7"; // Update with your missile name
        boostTimer = 0;
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {

        if (rb.simulated != false)
        {
            velocity = ((int)rb.velocity.magnitude);
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
            // Calculate heading velocity (assuming heading has a Rigidbody2D component)
            Vector3 targetVelocity = target.GetComponent<Rigidbody2D>().velocity;

            // Use first-order intercept to predict heading position
            float timeToIntercept = Vector3.Distance(transform.position, target.position) / maxSpeed;
            Vector3 predictedTargetPosition = target.position + (targetVelocity * timeToIntercept);

            // Update heading direction based on predicted position
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
        //  Destroy(gameObject);
        //}
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
    bool CheckValidCollision(Collider2D collider)
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
    IEnumerator checkTargetinginfo(WeaponsManager info)
    {
        while (lockestablished==true)
        {
            if (info.target == null)
            {
                

                target = null;
                lockestablished = false;
            }
            else if (info.target != null)
            {
                
                target = info.target.transform;
                lastKnownTargetPosition = target.position;
                lastKnownTargetVelocity = target.GetComponent<Rigidbody2D>().velocity;
            }
            else // info.heading is null but we have past data
            {
                // Estimate new heading position based on previous velocity
               
                float timeSinceLastLock = Time.time - timeOfLastLock;
                Vector3 estimatedPosition = lastKnownTargetPosition + (lastKnownTargetVelocity * timeSinceLastLock);
                targetDirection = (estimatedPosition - transform.position).normalized;
                
            }

            yield return null;
        }





    }
    internal void fire(WeaponsManager parent)
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        if (parent != null)
        {
            lockestablished = true;
            StartCoroutine(checkTargetinginfo(parent));
        }
        transform.parent = null;
        

    }
}
