using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float patrolRadius = 10f;
    [SerializeField] private float minSpeed = 1f;
    [SerializeField] private float maxSpeed = 3f;
    [SerializeField] private float speedChangeDistance = 5f; // Distance for speed adjustments
    [SerializeField] private float rotationSpeed = 5f;

    private Vector2 currentWaypoint;
    private float currentSpeed;
    [SerializeField] private float rotationSmoothness = 5f;

    void Start()
    {
        SetNewPatrolDestination();
        currentSpeed = maxSpeed; // Start by moving fast
    }

    void Update()
    {
        // Determine heading rotation
        Vector2 direction = currentWaypoint - (Vector2)transform.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Smoothly rotate towards the heading
        float smoothedAngle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, rotationSmoothness * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, targetAngle);

        // Adjust speed based on distance 
        float distanceToWaypoint = Vector2.Distance(transform.position, currentWaypoint);
        if (distanceToWaypoint > speedChangeDistance)
        {
            currentSpeed = maxSpeed;
        }
        else
        {
            currentSpeed = Mathf.Lerp(minSpeed, maxSpeed, distanceToWaypoint / speedChangeDistance);
        }

        // Move towards the current waypoint
        transform.position = Vector2.MoveTowards(transform.position, currentWaypoint, currentSpeed * Time.deltaTime);

        // Check if we reached the waypoint
        if (Vector2.Distance(transform.position, currentWaypoint) < 0.5f)
        {
            SetNewPatrolDestination();
        }
    }

    void SetNewPatrolDestination()
    {
        Vector3 randomOffset = Random.insideUnitSphere * patrolRadius;
        randomOffset.y = 0; // Assuming you want patrolling on a plane
        currentWaypoint = transform.position + randomOffset;
       
    }
}
