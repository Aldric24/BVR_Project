using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class AmraamScript : MonoBehaviour
{
    [SerializeField] private float thrustForce;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float thrustDuration;
    [SerializeField]private bool isBoosting = true;
    private float boostTimer = 0;
    private Rigidbody2D rb;
    [SerializeField] private ParticleSystem missileParticleEffect;
    // Target Related
    private Transform target; // Set externally or retrieved from the player
    private Vector3 targetDirection;
    [SerializeField] private float raycastDistance = 2f; // Adjust as needed
    [SerializeField] private LayerMask collisionMask;
    SweepRotation S;
    Vector3 lastposition;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
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
        // Thrust-based movement (similar to ApplySteeringForce but using targetDirection)
        //Vector2 force = targetDirection * thrustForce;
        //rb.AddForce(force);
        rb.velocity = transform.up * maxSpeed;
        boostTimer += Time.fixedDeltaTime;
        if (boostTimer >= thrustDuration)
        {
            isBoosting = false;
            boostTimer = 0; // Reset the timer
        }
    }

    void InertialPhase()
    {
        // Movement based on current velocity 
        // ... (Potentially add some simple guidance updates here, e.g., slight course corrections)
        AlignWithVelocity();
        // Thrust-based movement (similar to ApplySteeringForce but using targetDirection)
        //Vector2 force = targetDirection * thrustForce;
        //rb.AddForce(force);
        rb.velocity = transform.up * 1;

        //boostTimer += Time.fixedDeltaTime;
        //if (boostTimer >= thrustDuration)
        //{
        //    isBoosting = false;
        //    boostTimer = 0; // Reset the timer
        //}
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
    }
    void AlignWithVelocity()
    {

        if (rb.velocity != Vector2.zero) // Check if we have any velocity
        {
            float rotateAmount = Vector3.Cross(targetDirection, transform.up).z;
            rb.angularVelocity = -rotateAmount * 200;

        }
    }
    //void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Adversary"))
    //    {
    //        Debug.Log("Enemy hit by missile! onject hit " + collision.gameObject.name);
    //        Destroy(collision.transform.parent.gameObject);

    //    }
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        Debug.Log("Player hit by missile!");
    //        //Destroy(collision.transform.parent.gameObject);
    //    }
    //    else
    //    {
    //        Debug.Log("Object hit by missile!");
    //    }
    //    // Put a particle effect he
    //    // {re

    //}
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
            
            
            if(parent.target!=null)
            {
                target = parent.target.transform;
            }
            else
            {
                target = null;
            }
        }
        
        gameObject.GetComponent<Rigidbody2D>().simulated=true;
       
    }
}
