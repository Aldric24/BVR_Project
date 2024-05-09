using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;



public class EnemyAI : MonoBehaviour
{
    public enum AIState { Search, Pursue, Attack, Evade, Investigate }
    public AIState currentState = AIState.Search;
    public Transform heading;
    public GameObject target;
    public PatrolWaypoint currentWaypoint;
    public float waypointReachedDistance = 5f;
    public AiControl aircraftControl;
    [SerializeField]int AircraftSpeed;
    public EnemyRadar radar;
    public float lockOnRange = 50f;
    [SerializeField] Collider2D AircraftCollider;
    private Vector3 lastKnownPlayerPosition;
    private Dictionary<GameObject, ThreatAssessment> threatList = new Dictionary<GameObject, ThreatAssessment>();
    private const float threatTimeout = 4.0f;
    [SerializeField]private Vector3 lastKnownTargetPosition;
    [SerializeField] string SelectedWeapon;
    [SerializeField]private AiWeaponsManager weaponsManager;
    private class ThreatAssessment
    {
        public float lastDetectionTime;
        public float threatLevel; // You'll determine how to calculate this
    }
    void Start()
    {
        SelectedWeapon= weaponsManager.currentMissileType;
        aircraftControl = GetComponent<AiControl>();
        SelectNewPatrolWaypoint();
        StartCoroutine(RemoveStaleThreats());
    }

    void Update()
    {

        
        switch (currentState)
        {
            case AIState.Search:
                HandleSearch();
                
                break;
            case AIState.Pursue:
                HandlePursue();
                break;
            case AIState.Attack:
                Debug.Log("Attacking");
                HandleAttack();
                break;
            case AIState.Evade:
                HandleEvade();
                break;
            case AIState.Investigate:
                HandleInvestigate();
                break;
        }
        ScanForPlayerTarget();
    }
    
    void HandleSearch()
    {
        AircraftSpeed = 40;
        aircraftControl.targetSpeed = AircraftSpeed;
        aircraftControl.target = currentWaypoint.transform; 
        if (heading != null)
        {
            Debug.Log("Heading is not null");
            currentState = AIState.Pursue;
            return;
        }
        aircraftControl.ManageThrust(AircraftSpeed);
       
        if (Vector2.Distance(transform.position, currentWaypoint.transform.position) < waypointReachedDistance)
        {
            SelectNewPatrolWaypoint();
        }
    }
    //void HandleRotation()
    //{
    //    if (heading == null)
    //    {
    //        return; // Do nothing if there's no heading
    //    }

    //    float rotationAngle = CalculateAngleTowards(heading.position);
    //    //aircraftControl.HandleRotation(rotationAngle);
    //    Debug.Log("HandleRotation called with angle: " + rotationAngle);

    //    // Add gizmos here for visualization
    //    Debug.DrawRay(transform.position, transform.forward * 10, Color.red, 0.5f);
    //    Vector3 targetDirection = Quaternion.Euler(0, 0, rotationAngle) * Vector3.forward;
    //    Debug.DrawRay(transform.position, targetDirection * 5, Color.green, 0.5f);
    //}

    void ScanForPlayerTarget()
    {
        // 1. Check for Player in RadarObjects
        if (radar.RadarObjects.Count > 0)
        {
            //Debug.Log("radar objects "+ radar.RadarObjects.Count);
            foreach (GameObject obj in radar.RadarObjects)
            {
                if (obj.CompareTag("Player"))
                {
                    target = obj;
                    heading = target.transform;
                    lastKnownTargetPosition = target.transform.position; // Update last position
                    return;
                }
            }
        }
        else // No targets in radar objects
        {
            target = null;
        }
            //    if (lastKnownTargetPosition != Vector3.zero)
            //    {
            //        currentState = AIState.Investigate;
            //    }
            //    else
            //    {
            //        currentState = AIState.Search;
            //    }
            //}
            AssessRWRAndRadarThreat();
    }

    void HandlePursue()
    {
       
        if (target == null) // Ensure that the target still exists
        {
            if (lastKnownTargetPosition != Vector3.zero)
            {
                currentState = AIState.Investigate;
            }
            else
            {
                currentState = AIState.Search;
            }
            return;
        }
        AircraftSpeed = 60;  // Increase speed during pursuit
        aircraftControl.target = heading;
        aircraftControl.ManageThrust(AircraftSpeed);

        // 2. Attempt Lock-on
        float distanceToTarget = Vector2.Distance(transform.position, heading.position);
        
 
        if (distanceToTarget <= lockOnRange)
        {
            radar.LockedTarget = heading.gameObject;
            currentState = AIState.Attack;
        }
    }
    private void OnDrawGizmos()
    {
        if (target != null) // Only draw if a target exists
        {
            Gizmos.color = Color.yellow; // Adjust color as desired
            Gizmos.DrawWireSphere(transform.position, lockOnRange);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
       OnRWRAlert(collision.gameObject);
    }
    public void OnRWRAlert(GameObject detectedBy)
    {
        if (!threatList.ContainsKey(detectedBy))
        {
            threatList.Add(detectedBy, new ThreatAssessment());
        }

        // Update threat assessment
        threatList[detectedBy].lastDetectionTime = Time.time;
        UpdateThreatLevel(detectedBy);
    }
    private void UpdateThreatLevel(GameObject threatObject)
    {
        ThreatAssessment threatData = threatList[threatObject];

        // Logic to calculate threatLevel:
        if (threatObject.CompareTag("PlayerMissile"))
        {
            threatData.threatLevel = 1.0f; // Highest priority
        }
        else if (threatObject.CompareTag("Player"))
        {
            threatData.threatLevel = 0.7f;
        }
        else
        {
            threatData.threatLevel = 0.3f; // Lower priority
        }

        // You can add more factors here (distance, AI state, etc.)
    }
    private void AssessRWRAndRadarThreat()
    {
        bool playerSeesAI = false;

        // Check if player is both in radar list and detected by RWR
        foreach (var pair in threatList)
        {
            if (pair.Key.CompareTag("Player") && radar.RadarObjects.Contains(pair.Key))
            {
                playerSeesAI = true;
                break; // Found a match, no need to continue iteration
            }
        }

        ReactToThreatScenario(playerSeesAI);
    }
    private IEnumerator RemoveStaleThreats()
    {
        while (true)
        {
            List<GameObject> keysToRemove = new List<GameObject>();
            if(threatList.Count > 0)
            {
                foreach (var pair in threatList)
                {
                    if (Time.time - pair.Value.lastDetectionTime > threatTimeout)
                    {
                        keysToRemove.Add(pair.Key);
                    }
                }
            }
            

            // Remove outside of iteration loop to avoid modifying while iterating 
            if(keysToRemove.Count > 0)
            {
                foreach (GameObject key in keysToRemove)
                {
                    threatList.Remove(key);
                }
            }
            

            yield return new WaitForSeconds(1.0f); // Check for stale threats every second
        }
    }
    private void ReactToThreatScenario(bool playerSeesAI)
    {
        if (playerSeesAI)
        {
            // Mutual Detection: React defensively and prepare to counterattack
            currentState = AIState.Evade;
            // ... Add immediate counterattack logic ...
        }
        else if (radar.RadarObjects.Count > 0)
        {
            // Player in radar but not RWR: Upper hand situation
            currentState = AIState.Pursue;
        }
        // ... handle other scenarios as needed...
    }
    private void AssessThreats() // Call this periodically, maybe inside Update()
    {
        GameObject highestThreat = null;
        float highestThreatLevel = 0.0f;

        foreach (var pair in threatList)
        {
            if (pair.Value.threatLevel > highestThreatLevel)
            {
                highestThreat = pair.Key;
                highestThreatLevel = pair.Value.threatLevel;
            }
        }

        // React to the highest threat (if any)
        if (highestThreat != null)
        {
           // RespondToThreat(highestThreat);
        }
    }
    void HandleAttack()
    {
        Debug.Log("Attacking target");
        if (target == null) // Ensure that the target still exists
        {
            currentState = AIState.Search;
            heading = null; // Reset heading
            return;
        }
        StartCoroutine(tempafterAttacK());

        //// Fine-tune Attack Positioning (optional)
        //// ... Add logic to maneuver for optimal attack position 

        //// Check if able to fire
        //if (CanFire())
        //{
        //    FireMissile();
        //    currentState = AIState.Evade;
        //}
    }
    IEnumerator tempafterAttacK()
    {
        yield return new WaitForSeconds(5);
        currentState = AIState.Evade;
    }
    void HandleInvestigate()
    {
        AircraftSpeed = 50;

        // Check if we have a temporary target from previous investigation
        if (aircraftControl.target != null && aircraftControl.target.name == "Temp Target")
        {
            // Existing target was a temporary one, so keep using it
            aircraftControl.ManageThrust(AircraftSpeed);
        }
        else
        {
            // Create a temporary GameObject
            GameObject tempTargetObject = new GameObject("Temp Target");
            tempTargetObject.transform.position = lastKnownTargetPosition;
            heading = tempTargetObject.transform;
            aircraftControl.target = heading;
            aircraftControl.ManageThrust(AircraftSpeed);
        }

        // Check if arrived at the investigation point
        if (Vector2.Distance(transform.position, lastKnownTargetPosition) < 20f) // Adjust threshold 
        {
            Debug.Log("Arrived at last known target position");
            lastKnownTargetPosition = Vector3.zero; // Reset 
            currentState = AIState.Search;
            heading = null; // Reset heading
            target = null; // Reset target

            // Destroy the temporary target object
            if (aircraftControl.target != null && aircraftControl.target.gameObject != null)
                Destroy(aircraftControl.target.gameObject);
        }
        if (radar.RadarObjects.Contains(target))
        {

            // Player detected again! Switch to pursue, discard temp target
            currentState = AIState.Pursue;
            heading = target.transform;

            // Destroy temporary target object (if it exists)
            if (aircraftControl.target != null && aircraftControl.target.gameObject != null)
                Destroy(aircraftControl.target.gameObject);
        }
    }

    void HandleEvade()
    {
        // Evade Type
        bool isEvadingMissile = threatList.Any(pair => pair.Key.CompareTag("PlayerMissile"));

        if (isEvadingMissile)
        {
            // Placeholder for missile evasion logic
            EvasiveManeuverAgainstMissile();
        }
        else
        {
            // Evade player (after attack)
            EvasiveManeuverAwayFromPlayer();
        }
    }

    // Evasive Maneuvers
    private void EvasiveManeuverAgainstMissile()
    {
        // TODO: Implement missile evasion logic (e.g., notching, flares, etc.)
    }

    private void EvasiveManeuverAwayFromPlayer()
    {
        if (target == null) // Ensure we have a target
        {
            currentState = AIState.Search;
            return;
        }

        Vector3 directionAwayFromPlayer = (transform.position - target.transform.position).normalized;
        Vector3 farthestEvadePoint = transform.position + directionAwayFromPlayer * 100f; // Example, adjust distance

        // Set up temporary target for movement
        GameObject tempEvadeTarget = new GameObject("Evade Target");
        tempEvadeTarget.transform.position = farthestEvadePoint;
        aircraftControl.target = tempEvadeTarget.transform;

        // Add logic to destroy the tempEvadeTarget when appropriate 

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
    //void OnDrawGizmos()
    //{
    //    // Show the aircraft's actual forward direction
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawRay(transform.position, transform.forward * 2f);

    //    // The green line remains helpful as well!
    //}
}

