using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
    [SerializeField] bool playerSeesAI;
    public List<GameObject> tempTargets = new List<GameObject>(); // Add this to your class
    [SerializeField] private float flareIntervalMissile = 1.5f; // Seconds between flares when under missile threat
    [SerializeField] private float chaffIntervalMissile = 1.5f;
    [SerializeField] private float flareIntervalNormal = 3.0f;  // Seconds between flares during regular evasion
    [SerializeField] private float chaffIntervalNormal = 3.0f;
    [SerializeField] private float nextFlareCountermeasureTime = 0f;
    [SerializeField] private float nextChaffCountermeasureTime = 0f;
    [SerializeField] private GameObject flarePrefab;
    [SerializeField] bool missileinbound = false;
    [SerializeField] private GameObject chaffPrefab;
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
        Debug.Log("AI State: Search");
        AircraftSpeed = 40;
        aircraftControl.targetSpeed = AircraftSpeed;
        aircraftControl.target = currentWaypoint.transform; 
        if (heading != null)
        {
            Debug.Log("Heading acquired during search, switching to Pursue state");
            currentState = AIState.Pursue;
            return;
        }
        aircraftControl.ManageThrust(AircraftSpeed);
       
        if (Vector2.Distance(transform.position, currentWaypoint.transform.position) < waypointReachedDistance)
        {
            SelectNewPatrolWaypoint();
        }
    }
   
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
          
        AssessRWRAndRadarThreat();
    }

    void HandlePursue()
    {
        Debug.Log("AI State: Pursue");

        if (target == null && threatList.Count<1 ) // Ensure that the target still exists
        {
            if (lastKnownTargetPosition != Vector3.zero)
            {
                Debug.Log("Lost target but have last known position. Switching to Investigate state.");
                currentState = AIState.Investigate;
                
            }
            else
                Debug.Log("Lost target and no last known position. Switching to Search state.");
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
       
        if(collision.tag!="PlayerMissile" && collision.tag!="Player")
        {
           
            return;
        }
        else
        {
            Debug.Log("AI RWR Collision detected");
            OnRWRAlert(collision.gameObject);
        }
       
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
            target = threatObject;
        }
        else
        {
            threatData.threatLevel = 0.3f; // Lower priority
        }

        // You can add more factors here (distance, AI state, etc.)
    }
    private void AssessRWRAndRadarThreat()
    {
        if(threatList.Count<1 )
        {
            if (lastKnownTargetPosition != Vector3.zero)
            {
                currentState = AIState.Investigate;
            }
            
            return;
        }
       
        // Check if player is both in radar list and detected by RWR
        foreach (var pair in threatList)
        {
            if (pair.Key.CompareTag("Player") )
            {
                playerSeesAI = true;
                
                break; // Found a match, no need to continue iteration
            }
            else if(pair.Key.CompareTag("PlayerMissile"))
            {
                missileinbound = true;
                break;
            }
            else if(radar.RadarObjects.Contains(pair.Key))
            {
                playerSeesAI = false;
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
        else 
        {
            // Player in radar but not RWR: Upper hand situation
            currentState= AIState.Pursue;
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
        Debug.Log("AI State: Attack");

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
        Debug.Log("Delay after attack complete. Switching to Evade state.");
        currentState = AIState.Evade;
    }
    void HandleInvestigate()
    {
        Debug.Log("AI State: Investigate");
        AircraftSpeed = 50;

        // Manage temp target list
        CleanUpOldTempTargets();

        // Create a new temporary GameObject
       

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

            // Add to the list
            tempTargets.Add(tempTargetObject);
        }

        // Check if arrived at the investigation point
        if (Vector2.Distance(transform.position, lastKnownTargetPosition) < 20f) // Adjust threshold 
        {
            Debug.Log("Arrived at last known target position - Investigation complete.");
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
            Debug.Log("Player detected during investigation! Switching to Pursue state");
       
            // Player detected again! Switch to pursu   e, discard temp target
            currentState = AIState.Pursue;
            heading = target.transform;

            // Destroy temporary target object (if it exists)
            if (aircraftControl.target != null && aircraftControl.target.gameObject != null)
                Destroy(aircraftControl.target.gameObject);
        }
    }
    private void CleanUpOldTempTargets()
    {
        for (int i = 0; i < tempTargets.Count; i++)
        {
            // Destroy if null or if the name doesn't match the expected format
            if (tempTargets[i] == null || !tempTargets[i].name.Equals("Temp Target"))
            {
                Destroy(tempTargets[i]);
                tempTargets.RemoveAt(i);
                i--; // Decrement index since the list has shortened
            }
        }
    }
    void HandleEvade()
    {
        Debug.Log("AI State: Evade");
        if (!playerSeesAI && !radar.RadarObjects.Contains(target))
        {
            Debug.Log("No longer under threat, switching to Search state.");
            currentState = AIState.Search;
            target = null;
            heading = null;
        }
        // Evade Type
        bool isEvadingMissile = missileinbound;

        if (isEvadingMissile)
        {
            Debug.Log("Evading missile - executing missile evasion maneuvers");
            // ... 
            // Placeholder for missile evasion logic
            EvasiveManeuverAgainstMissile();
            DeployCountermeasures(isEvadingMissile);
        }
        else
        {
            Debug.Log("Evading player - executing player evasion maneuvers");
            // Evade player (after attack)
            EvasiveManeuverAwayFromPlayer();
            DeployCountermeasures(isEvadingMissile);
        }
    }
    private void DeployCountermeasures(bool isEvadingMissile)
    {
        if (isEvadingMissile)
        {
            // More frequent countermeasures
            if (Time.time > nextFlareCountermeasureTime)
            {
                weaponsManager.DeployFlares();
                nextFlareCountermeasureTime = Time.time + flareIntervalMissile;
            }
            if (Time.time > nextChaffCountermeasureTime)
            {
                weaponsManager.DeployChaff();
                nextChaffCountermeasureTime = Time.time + chaffIntervalMissile;
            }
        }
        else
        {
            // More frequent countermeasures
            if (Time.time > nextFlareCountermeasureTime)
            {
                Debug.Log("Deploying flares");
                Vector3 offset = Random.insideUnitCircle ;
                GameObject flare = Instantiate(flarePrefab, transform.position + offset, transform.rotation);
                nextFlareCountermeasureTime = Time.time + flareIntervalMissile;
            }
            if (Time.time > nextChaffCountermeasureTime)
            {
                Debug.Log("Deploying chaff");
                Vector3 offset = Random.insideUnitCircle;
                GameObject flare = Instantiate(chaffPrefab, transform.position + offset, transform.rotation);
                nextChaffCountermeasureTime = Time.time + chaffIntervalMissile;
            }
        }
    }
    // Evasive Maneuvers
    private void EvasiveManeuverAgainstMissile()
    {
        // TODO: Implement missile evasion logic (e.g., notching, flares, etc.)
        Debug.Log("evading missile");
        missileinbound = false;
    }

    private void EvasiveManeuverAwayFromPlayer()
    {
        if (target == null)
        {
            Debug.LogWarning("Target became null during evasion. Falling back to waypoint.");
            aircraftControl.target = GetFarthestWaypoint()?.transform; // Get waypoint if possible
            return;
        }

        Vector3 directionAwayFromPlayer = (transform.position - target.transform.position).normalized;
        Vector3 farthestEvadePoint = transform.position + directionAwayFromPlayer * 1000;

        // Set up temporary target for movement
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Evade Target");
        if (gameObjects.Count() > 2)
        {
            Destroy(GameObject.Find("Evade Target"));
        }
        GameObject tempEvadeTarget = new GameObject("Evade Target");
        tempEvadeTarget.tag = "Evade Target";
        tempEvadeTarget.transform.position = farthestEvadePoint;
        aircraftControl.target = tempEvadeTarget.transform;

        // Logic to destroy tempEvadeTarget (with null checks)
        if (Vector2.Distance(transform.position, tempEvadeTarget.transform.position) < 20f)
        {
            Debug.Log("Arrived at evade position");
            lastKnownTargetPosition = Vector3.zero; // Reset 
            currentState = AIState.Search;
            heading = null; // Reset heading
            target = null; // Reset target

            // Destroy the temporary target object (with null checks)
            if (aircraftControl.target != null && aircraftControl.target.gameObject != null)
            {
                Destroy(aircraftControl.target.gameObject);
            }
        }
    }

    // Helper function to get the farthest waypoint
    private PatrolWaypoint GetFarthestWaypoint()
    {
        PatrolWaypoint[] waypoints = FindObjectsOfType<PatrolWaypoint>();
        if (waypoints.Length == 0)
        {
            Debug.LogError("No waypoints found!");
            return null;
        }

        PatrolWaypoint farthestWaypoint = waypoints[0];
        float farthestDistance = 0f;

        foreach (PatrolWaypoint waypoint in waypoints)
        {
            float distance = Vector2.Distance(transform.position, waypoint.transform.position);
            if (distance > farthestDistance)
            {
                farthestDistance = distance;
                farthestWaypoint = waypoint;
            }
        }

        return farthestWaypoint;
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

