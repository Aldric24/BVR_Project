using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EnemyAI : MonoBehaviour
{
    public enum AIState { Search, Pursue, Attack, Evade }
    public AIState currentState = AIState.Search;
    public Transform target;
    public PatrolWaypoint currentWaypoint;
    public float waypointReachedDistance = 5f;
    public AiControl aircraftControl;
    [SerializeField]int AircraftSpeed;
    public EnemyRadar radar;
    public float lockOnRange = 50f;
    [SerializeField] Collider2D AircraftCollider;
    void Start()
    {
        aircraftControl = GetComponent<AiControl>();
        SelectNewPatrolWaypoint();
    }

    void Update()
    {
        ScanForPlayerTarget();
        switch (currentState)
        {
            case AIState.Search:
                HandleSearch();
                
                break;
            case AIState.Pursue:
                HandlePursue();
                break;
            case AIState.Attack:
                HandleAttack();
                break;
            case AIState.Evade:
                HandleEvade();
                break;
        }
       
    }
    
    void HandleSearch()
    {
        AircraftSpeed = 40;
        aircraftControl.target = currentWaypoint.transform; 
        if (target != null)
        {
            currentState = AIState.Pursue;
            return;
        }
        aircraftControl.ManageThrust(AircraftSpeed);
       
        if (Vector2.Distance(transform.position, currentWaypoint.transform.position) < waypointReachedDistance)
        {
            SelectNewPatrolWaypoint();
        }
    }
    void HandleRotation()
    {
        if (target == null)
        {
            return; // Do nothing if there's no target
        }

        float rotationAngle = CalculateAngleTowards(target.position);
        //aircraftControl.HandleRotation(rotationAngle);
        Debug.Log("HandleRotation called with angle: " + rotationAngle);

        // Add gizmos here for visualization
        Debug.DrawRay(transform.position, transform.forward * 10, Color.red, 0.5f);
        Vector3 targetDirection = Quaternion.Euler(0, 0, rotationAngle) * Vector3.forward;
        Debug.DrawRay(transform.position, targetDirection * 5, Color.green, 0.5f);
    }

    void ScanForPlayerTarget()
    {
        // 1. Check for Player in RadarObjects
        if(radar.RadarObjects.Count > 0)
        {
            foreach (GameObject obj in radar.RadarObjects)
            {
                if (obj.CompareTag("Player"))
                {
                    target = obj.transform;
                    currentState = AIState.Pursue;
                    return;  // Found the player, stop searching
                }
            }
        }
        else
            target = null;
            currentState = AIState.Search;
        
    }

    void HandlePursue()
    {
        
        if (target == null) // Ensure that the target still exists
        {
            currentState = AIState.Search;
            return;
        }

        AircraftSpeed = 60;  // Increase speed during pursuit
        aircraftControl.target = target;
        aircraftControl.ManageThrust(AircraftSpeed);

        // 2. Attempt Lock-on
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget <= lockOnRange)
        {
            radar.LockedTarget = target.gameObject;
            currentState = AIState.Attack;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 3. Check for player collision
        if (collision.gameObject.CompareTag("Player"))
        {
            // Handle collision with player
            currentState = AIState.Evade;
        }
    }
    void HandleAttack()
    {
        if (target == null)
        {
            currentState = AIState.Search;
            return;
        }

        float rotationAngle = CalculateAngleTowards(target.position);
     

       
    }

    void HandleEvade()
    {
        // TODO: Implement evasive maneuvers 
    }

    void SelectNewPatrolWaypoint()
    {
        PatrolWaypoint[] waypoints = FindObjectsOfType<PatrolWaypoint>();

        if (waypoints.Length > 0)
        {
            currentWaypoint = waypoints[Random.Range(0, waypoints.Length)];
        }
    }

    float CalculateAngleTowards(Vector3 destination)
    {
        Vector3 direction = (destination - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return angle;
    }
    void OnDrawGizmos()
    {
        // Show the aircraft's actual forward direction
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 2f);

        // The green line remains helpful as well!
    }
}

