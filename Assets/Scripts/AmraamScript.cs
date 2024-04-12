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
    SweepRotation S = new SweepRotation();
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
        //AlignWithVelocity();
    }
    private void guidance()
    {
        if(target != null)
        {
            targetDirection = (target.position - transform.position).normalized;
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
    void CheckRaycastCollision()
    {
        Vector3 currentPosition = transform.position;
        Vector3 error = currentPosition - lastposition;
        Ray ray = new Ray(lastposition, error.normalized);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, error.magnitude, collisionMask.value))
        {
            Debug.Log("HIT " + hit.collider.gameObject);
            

            if (hit.collider.gameObject.tag == "Player")
            {
                rb.position = hit.point;
                Debug.Log("HIT " + hit.collider.gameObject);
            }
        }

        lastposition = currentPosition;
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
    void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Adversary"))
        {
            Debug.Log("Enemy hit by missile! onject hit " + collision.gameObject.name);
            Destroy(collision.transform.parent.gameObject);

        }
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player hit by missile!");
            //Destroy(collision.transform.parent.gameObject);
        }
        else
        {
            Debug.Log("Object hit by missile!");
        }
        // Put a particle effect he
        // {re

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
