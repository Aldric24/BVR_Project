using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidewinderScript : Weapon
{
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
    [SerializeField] float radarAngle = 30f;  // Half the angle of the radar cone
    [SerializeField] float radarRange = 20f;

    [SerializeField] private PolygonCollider2D radarCollider; // Cone-shaped collider

    private List<GameObject> potentialTargets = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        weaponName = "Sidewinder AIM-9M";
        rb = GetComponent<Rigidbody2D>();
        // ... [Your existing Start() code] ...
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (rb.simulated != false)
        {
            guidance();
            //UpdateParticleEffect();

            if (isBoosting)
            {
                //BoostPhase();
            }
            else
            {
                //InertialPhase();
            }
            // CheckRaycastCollision(); // Might not need raycast with heat-seeking
        }
    }

    private void guidance()
    {
        if (target != null)
        {
            // ... [Your existing target tracking logic] ...
        }
        else
        {
            targetDirection = FindHeatSource();
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        //if (//other.gameObject.GetComponent<HeatSource>() != null)
        //{
        //    potentialTargets.Add(other.gameObject);
        //}
    }

    Vector3 FindHeatSource()
    {
        GameObject hottestTarget = null;
        float maxTemperature = 0;

        foreach (GameObject target in potentialTargets)
        {
            ////HeatSource heatSource = target.GetComponent<HeatSource>();
            //if (heatSource != null && heatSource.temperature > maxTemperature)
            //{
            //    hottestTarget = target;
            //    maxTemperature = heatSource.temperature;
            //}
        }

        potentialTargets.Clear();

        if (hottestTarget != null)
        {
            return (hottestTarget.transform.position - transform.position).normalized;
        }
        else
        {
            return transform.up;
        }
    }

    // ... [Rest of your functions from the AmraamScript] ...
}