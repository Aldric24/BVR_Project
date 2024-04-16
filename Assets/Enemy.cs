using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float patrolRadius = 10f;

    private Vector2 currentWaypoint;

    void Start()
    {
        SetNewPatrolDestination();
    }

    void Update()
    {
        // Move toward the current waypoint
        transform.position = Vector2.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);

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
