using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Fox2Script : Weapon
{
    [SerializeField] private float thrustForce;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float thrustDuration;
    [SerializeField] private bool isBoosting;
    private float boostTimer = 0;
    public Rigidbody2D rb;
    [SerializeField] private ParticleSystem missileParticleEffect;
    // Target Related// Set externally or retrieved from the player
    private Vector3 targetDirection;
    [SerializeField] private float raycastDistance = 2f; // Adjust as needed
    [SerializeField] private float minVelocityThreshold = 1f;
    [SerializeField] private float baseDecelerationRate = 0.5f;
    [SerializeField] private float turningDecelerationMultiplier = 2.0f; // Increase deceleration during t                                             // Minimum velocity before destroying
    [SerializeField] private float decelerationRate = 0.5f; // Rate of velocity decrease
    [SerializeField] private float acceleration = 10f; // Acceleration rate
    Vector3 lastposition;
    [SerializeField] private int velocity;
    [SerializeField] float radarAngle = 30f;  // Half the angle of the radar cone
    [SerializeField] float radarRange = 20f;

    [SerializeField] private PolygonCollider2D radarCollider; // Cone-shaped collider

    private List<GameObject> potentialTargets = new List<GameObject>();
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] public Transform heatTarget;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip searchSound;
    [SerializeField] private AudioClip lockSound;
    [SerializeField] private bool isSearching = false;
    [SerializeField] private bool hasTarget = false;
    [SerializeField]bool launched= false;
    public bool is_equipped=false;
    public bool mute=true;
    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.transform.parent!= null && gameObject.transform.parent.tag == "Player")
        {
            gameObject.tag = "PlayerMissile";
        }
        StartCoroutine(AudioCallOut());
        weaponName = "Sidewinder AIM-9M";

        isSearching = true;
       
        Debug.Log("Target LAyer " + targetLayerMask);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        FindHeatTarget();
        if (rb.simulated != false)
        {
            
            if (launched)
            {
                guidance();
                velocity = ((int)rb.velocity.magnitude);
                //UpdateParticleEffect();

                if (isBoosting)
                {
                    BoostPhase();
                }
                else
                {
                    InertialPhase();
                }
                CheckRaycastCollision(); // Might not need raycast with heat-seeking
            }
            
        }
    }
    IEnumerator AudioCallOut()
    {
        while (mute)
        {
            if (isSearching)
            {
                if (audioSource.clip != searchSound)
                {
                    audioSource.clip = searchSound;
                    audioSource.loop = true;
                    audioSource.Play();
                }
            }
            else
            {
                if (audioSource.clip != lockSound)
                {
                    audioSource.clip = lockSound;
                    audioSource.loop = true;
                    audioSource.Play();
                }
            }

            yield return new WaitForSeconds(0.2f); // Slight delay between checks
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

        //Self - destruct Logic
        if (rb.velocity.magnitude < minVelocityThreshold)
        {
            Destroy(gameObject);
        }
    }
    void AlignWithVelocity()
    {

        if (rb.velocity != Vector2.zero) // Check if we have any velocity
        {
            float rotateAmount = Vector3.Cross(targetDirection, transform.up).z;
            rb.angularVelocity = -rotateAmount * 200;

        }

    }
    bool CheckValidCollision(Collider2D collider)
    {
        if (collider == null) return false;

        if (gameObject.tag == "PlayerMissile" && collider.gameObject.tag == "Player")
        {
            return false;
        }
        if (gameObject.CompareTag("AdversaryMissile") && collider.gameObject.CompareTag("Adversary")) return false;// Ignore player collisions
        else {

            return collisionMask.value == (collisionMask.value | (1 << collider.gameObject.layer));
        }

        
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
        //Destroy(objectHit);
        objectHit.SetActive(false);
        FindAnyObjectByType<HUD_Text>().SplashText(objectHit.name);
    }
    private void guidance()
    {
        FindHeatTarget(); // W
        if (heatTarget  != null)
        {
            
            // Calculate heading velocity (assuming heading has a Rigidbody2D component)
            Vector3 targetVelocity = heatTarget.GetComponent<Rigidbody2D>().velocity;

            // Use first-order intercept to predict heading position
            float timeToIntercept = Vector3.Distance(transform.position, heatTarget.position) / maxSpeed;
            Vector3 predictedTargetPosition = heatTarget.position + (targetVelocity * timeToIntercept);

            // Update heading direction based on predicted position
            targetDirection = (predictedTargetPosition - transform.position).normalized;
        }
        else
        {
            audioSource.Stop(); // Stop playing the lock sound
            targetDirection = transform.up;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        //if (//other.gameObject.GetComponent<HeatSource>() != null)
        //{
        //    potentialTargets.Add(other.gameObject);
        //}
    }

    private void FindHeatTarget()
    {
        Collider2D[] detectedColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, targetLayerMask);
        //Debug.Log("Detected colliders: " + detectedColliders.Length);
        if (detectedColliders.Length > 0)
        {
            //Debug.Log("Targets detected: " + detectedColliders[0].name);

            HeatSource potentialTarget = null;
            float largestHeatIntensity = 0f;

            foreach (Collider2D collider in detectedColliders)
            {
                HeatSource heatSource = collider.gameObject.GetComponent<HeatSource>();
                if(gameObject.tag == "PlayerMissile" && collider.gameObject.tag=="Player")
                {
                    return;
                } 
                if(gameObject.tag == "AdversaryMissile" && collider.gameObject.tag=="Adversary")
                {
                    return;
                }
                else if(heatSource != null && heatSource.heatIntensity > largestHeatIntensity)
                {
                    potentialTarget = heatSource;
                    largestHeatIntensity = heatSource.heatIntensity;
                }
            }

            if (potentialTarget != null )
            {
                heatTarget = potentialTarget.transform;
                isSearching = false;
                hasTarget = true;
                //Debug.Log("Found target: " + heatTarget.gameObject.name + ", intensity: " + largestHeatIntensity);

            }
            else
            {
                isSearching = true;
                hasTarget = false;
                heatTarget = null;
                //Debug.Log("No targets detected within radius");
            }
            // Clear after processing
        }
        else
        {
            isSearching = true;
            hasTarget = false;
            heatTarget = null;
            //Debug.Log("No targets detected within radius");
        }
           
    }
    void OnDrawGizmos()
    {
        if (enabled)  // Only draw gizmo when the script is active
        {
            Gizmos.color = Color.yellow; // Adjust color as you like
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
    internal void fire(WeaponsManager parent)
    {
        rb.bodyType= RigidbodyType2D.Dynamic;
        launched = true;
        if (parent != null)
        {
            FindHeatTarget();
        }
        transform.parent = null;
        gameObject.GetComponent<Rigidbody2D>().simulated = true;

    }

    
}