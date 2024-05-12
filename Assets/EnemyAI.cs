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
    [SerializeField] int AircraftSpeed;
    public EnemyRadar radar;
    public float lockOnRange = 50f;
    [SerializeField] Collider2D AircraftCollider;
    private Vector3 lastKnownPlayerPosition;
    private Dictionary<GameObject, int> threatList = new Dictionary<GameObject, int>();
    private const float threatTimeout = 4.0f;
    [SerializeField] private Vector3 lastKnownTargetPosition;
    [SerializeField] string SelectedWeapon;
    [SerializeField] private AiWeaponsManager weaponsManager;
    [SerializeField] bool playerSeesAI;
    public List<GameObject> tempTargets = new List<GameObject>(); // Add this to your class
    public List<GameObject> evadeTargets = new List<GameObject>(); // Add this to your class
    [SerializeField] private float flareIntervalMissile = 1.5f; // Seconds between flares when under missile threat
    [SerializeField] private float chaffIntervalMissile = 1.5f;
    [SerializeField] private float flareIntervalNormal = 3.0f;  // Seconds between flares during regular evasion
    [SerializeField] private float chaffIntervalNormal = 3.0f;
    [SerializeField] private float nextFlareCountermeasureTime = 0f;
    [SerializeField] private float nextChaffCountermeasureTime = 0f;
    [SerializeField] private GameObject flarePrefab;
    [SerializeField] bool missileinbound = false;
    [SerializeField] private GameObject chaffPrefab;
   
    void Start()
    {
        SelectedWeapon = weaponsManager.currentMissileType;
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
                if (obj == null)
                {
                    Debug.LogWarning("Null object detected in radarObjects. Removing it.");
                    radar.RadarObjects.Remove(obj); // Remove the null reference
                    continue;  // Skip to the next object
                }

                if (obj.CompareTag("Player"))
                {
                    target = obj;
                    heading = target.transform;
                    lastKnownTargetPosition = target.transform.position;
                    return;
                }
            }
        }
        else // No targets in radar objects
        {
            target = null;
            weaponsManager.target = null;
        }
        AssessThreats();
        //AssessRWRAndRadarThreat();
    }

    void HandlePursue()
    {
        Debug.Log("AI State: Pursue");

        if (target == null && threatList.Count < 1) // Ensure that the target still exists
        {
            if (lastKnownTargetPosition != Vector3.zero)
            {
                Debug.Log("Lost target but have last known position. Switching to Investigate state.");
                currentState = AIState.Investigate;

            }
            else
            {
                Debug.Log("Lost target and no last known position. Switching to Search state.");
                currentState = AIState.Search;
            }
            return;
        }
        AircraftSpeed = 60;  // Increase speed during pursuit
        aircraftControl.target = heading;
        aircraftControl.ManageThrust(AircraftSpeed);

        // 2. Attempt Lock-on
        if (heading != null)
        {
            float distanceToTarget = Vector2.Distance(transform.position, heading.position);


            if (distanceToTarget <= lockOnRange)
            {
                radar.LockedTarget = heading.gameObject;
                currentState = AIState.Attack;
            }
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

        if (collision.tag != "PlayerMissile" && collision.tag != "Player")
        {

            return;
        }
        else
        {
            Debug.Log("AI RWR Collision detected");
            OnRWRAlert(collision.gameObject);
        }

    }
    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.tag != "PlayerMissile" && collision.tag != "Player")
        {

            return;
        }
        else
        {
            lastKnownTargetPosition = collision.transform.position;
            Debug.Log("AI RWR Collision detected");
            OnRWRAlert(collision.gameObject);
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        
        if (collision.tag != "PlayerMissile" && collision.tag != "Player")
        {

            return;
        }
        else
        {
            Debug.Log("missile radar doesnt see the ai");
          
        }
    }
    public void OnRWRAlert(GameObject detectedBy)
    {
        if (!threatList.ContainsKey(detectedBy))
        {
            Debug.Log("Detected by" + detectedBy.name);
            threatList.Add(detectedBy, (int)Time.time);
        }

        // Update threat assessment
    }

    private void AssessRWRAndRadarThreat()
    {
        if (threatList.Count < 1 && radar.RadarObjects.Count<1)
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
            if (pair.Key.CompareTag("Player"))
            {
                playerSeesAI = true;

                break; // Found a match, no need to continue iteration
            }
            else if (pair.Key.CompareTag("PlayerMissile"))
            {
                missileinbound = true;
                break;
            }
            else if (radar.RadarObjects.Contains(pair.Key))
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
            if (threatList.Count > 0)
            {
                foreach (var pair in threatList)
                {
                    if (Time.time - pair.Value > threatTimeout)
                    {
                        keysToRemove.Add(pair.Key);
                    }
                }
            }


            // Remove outside of iteration loop to avoid modifying while iterating 
            if (keysToRemove.Count > 0)
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
        if (playerSeesAI || missileinbound)
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
    private void AssessThreats()
    {
        // Check for missile threats first
        foreach (var threat in threatList.Keys)
        {
            if (threat != null && threat.CompareTag("PlayerMissile"))
            {
                Debug.Log("Missile threat detected! Switching to Evade state.");
                currentState = AIState.Evade;
                missileinbound = true;
                return; // Exit if missile threat is found
            }
            if (threat != null && threat.CompareTag("Player"))
            {
                Debug.Log("Player threat detected! Switching to Evade state.");
                currentState = AIState.Evade;
                playerSeesAI = true;
                missileinbound = false;
                return; // Exit if missile threat is found
            }
        }

        // No missile threats, check for player threat
        if (target!=null && threatList.ContainsKey(target) )
        {
            playerSeesAI = true;
            Debug.Log("Player threat detected! Switching to Pursue or Attack state.");

            // Determine appropriate action based on distance or other factors
            float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
            if (distanceToTarget <= lockOnRange)
            {
                currentState = AIState.Attack;
            }
            else
            {
                currentState = AIState.Pursue;
            }

            return; // Exit if player threat is found
        }

       return;
    }
    void HandleAttack()
    {
        Debug.Log("AI State: Attack");

        if (target == null) // Safety check if target is lost during attack
        {
            Debug.Log("Target lost during attack. Switching to Search state.");
            weaponsManager.target = null;
            currentState = AIState.Investigate;
            return;
        }
        float distanceToTarget = 0;
        if (target!=null)
        {
             distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
        }


        // Choose missile based on distance
        string selectedMissileType = SelectMissileBasedOnRange(distanceToTarget);
        weaponsManager.SwitchToMissileType(selectedMissileType);

        // Fire missile (replace with your actual firing logic)
        if (weaponsManager)
        {
            weaponsManager.selectedHardpoint.EnableMissile();
            weaponsManager.selectedHardpoint.Fire(weaponsManager);
            Debug.Log("Firing " + selectedMissileType + " at target.");
            currentState = AIState.Evade;
        }
    }

    private string SelectMissileBasedOnRange(float distance)
    {
        // Replace these values with your actual missile types and range thresholds
        if (distance <= 200f)
        {
            return "Fox2";
        }
        else if (distance <= 500f)
        {
            return "Fox1";
        }
        else
        {
            return "Fox3";
        }
    }
    //IEnumerator tempafterAttacK()
    //{
    //    yield return new WaitForSeconds(5);
    //    Debug.Log("Delay after attack complete. Switching to Evade state.");
    //    currentState = AIState.Evade;
    //}
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
            weaponsManager.target = null;
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
       
        //Evade Type
        bool isEvadingMissile = missileinbound;

        if (isEvadingMissile)
        {
            Debug.Log("Evading contact - executing missile evasion maneuvers");
            // ... 
            // Placeholder for missile evasion logic
            EvasiveManeuverAgainstMissile();
            DeployCountermeasures(isEvadingMissile);
        }
        else if (target != null && threatList.ContainsKey(target) ||playerSeesAI==true)
        {
            Debug.Log("Evading player - executing player evasion maneuvers");
            // Evade player (after attack)
            EvasiveManeuverAwayFromPlayer();
            DeployCountermeasures(isEvadingMissile);
        }
        else
        {
            Debug.Log("No immediate threat - falling back to waypoint");

            if (currentWaypoint != null)
            {
                // Move towards the current waypoint
                heading = currentWaypoint.transform;
                aircraftControl.target = heading;

                // Once reached the waypoint, switch to investigate
                if (Vector2.Distance(transform.position, currentWaypoint.transform.position) <= waypointReachedDistance)
                {
                    Debug.Log("Waypoint reached, switching to Investigate");
                    currentState = AIState.Investigate;
                    target = null;
                    heading = null;
                    weaponsManager.target = null;
                }
            }
            else
            {
                // Handle the case where there are no waypoints available
                Debug.LogWarning("No waypoint found - investigate state may not be triggered.");
            }
        }
           
    }
    private void DeployCountermeasures(bool isEvadingMissile)
    {
        if (isEvadingMissile)
        {

            if (Time.time > nextFlareCountermeasureTime)
            {
                Debug.Log("Deploying flares");
                Vector3 offset = Random.insideUnitCircle;
                GameObject flare = Instantiate(flarePrefab, transform.position + offset, transform.rotation);
                nextFlareCountermeasureTime = Time.time + flareIntervalMissile;
                flare.gameObject.tag = gameObject.tag;
            }
            if (Time.time > nextChaffCountermeasureTime)
            {
                Debug.Log("Deploying chaff");
                Vector3 offset = Random.insideUnitCircle;
                GameObject chaff = Instantiate(chaffPrefab, transform.position + offset, transform.rotation);
                nextChaffCountermeasureTime = Time.time + chaffIntervalMissile;
                chaff.gameObject.tag = gameObject.tag;
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
                nextFlareCountermeasureTime = Time.time + flareIntervalNormal;
                flare.gameObject.tag = gameObject.tag;
            }
            if (Time.time > nextChaffCountermeasureTime)
            {
                Debug.Log("Deploying chaff");
                Vector3 offset = Random.insideUnitCircle;
                GameObject chaff = Instantiate(chaffPrefab, transform.position + offset, transform.rotation);
                nextChaffCountermeasureTime = Time.time + chaffIntervalNormal;
                chaff.gameObject.tag = gameObject.tag;
            }
        }
    }
    
    // Evasive Maneuvers
    private void EvasiveManeuverAgainstMissile()
    {
        AircraftSpeed = 80;
        Debug.Log("Evading missile");
        Vector3 newEvadePoint=new Vector3();
        if (GetMissileThreat()!=null)
        {
            Vector3 directionAwayFromMissile = (transform.position - GetMissileThreat().transform.position).normalized;
             newEvadePoint = transform.position + directionAwayFromMissile * 1000f; // Example, adjust as needed
        }
        // Continuously change evade target


        // Destroy any existing "MissileEvadeTarget"
       if(GameObject.FindGameObjectWithTag("MissileEvadeTarget")==null)
       {
            GameObject tempEvadeTarget = new GameObject("MissileEvadeTarget");
            tempEvadeTarget.transform.position = newEvadePoint;
            tempEvadeTarget.tag = "MissileEvadeTarget";
            aircraftControl.target = tempEvadeTarget.transform;
       }
       

        // Check if arrived at evade position OR missile is no longer a threat
        if (Vector2.Distance(transform.position, GameObject.FindGameObjectWithTag("MissileEvadeTarget").transform.position) < 20f)
        {
            Debug.Log("Evade maneuver complete (reached position or missile no longer a threat)");

            // Cleanup and reset
            lastKnownTargetPosition = Vector3.zero;
            currentState = AIState.Investigate; // Go to Investigate since we don't have a target anymore
            heading = null;
            target = null;
            weaponsManager.target = null;
            missileinbound = false;

            Destroy(GameObject.FindGameObjectWithTag("MissileEvadeTarget")); // Destroy the temporary evade target
        }
    }
    private GameObject GetMissileThreat()
    {
        foreach (var pair in threatList)
        {
            if (pair.Key != null && pair.Key.CompareTag("PlayerMissile"))
            {
                return pair.Key;
            }
        }
        return null;
    }


    private void EvasiveManeuverAwayFromPlayer()
    {
        AircraftSpeed = 75;
        if (target == null)
        {
            Debug.LogWarning("Target became null during evasion. Falling back to waypoint.");
            aircraftControl.target = GetFarthestWaypoint()?.transform; // Get waypoint if possible
            return;
        }

        Vector3 directionAwayFromPlayer = (transform.position - target.transform.position).normalized;
        Vector3 farthestEvadePoint = transform.position + directionAwayFromPlayer * 1000;

        // Set up temporary target for movement
        
        if (GameObject.FindGameObjectWithTag("Evade Target")==null)
        {
            GameObject tempEvadeTarget = new GameObject("Evade Target");
            tempEvadeTarget.tag = "Evade Target";
            tempEvadeTarget.transform.position = farthestEvadePoint;
            aircraftControl.target = tempEvadeTarget.transform;
        }
        

        // Logic to destroy tempEvadeTarget (with null checks)
        if (Vector2.Distance(transform.position, GameObject.FindGameObjectWithTag("Evade Target").transform.position) < 20f)
        {
            Debug.Log("Arrived at evade position");
            lastKnownTargetPosition = Vector3.zero; // Reset 
            currentState = AIState.Investigate;
            heading = null; // Reset heading
            target = null; // Reset target
            weaponsManager.target = null;
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
    private void CleanUpOldEvadeTargets()
    {
        for (int i = 0; i < evadeTargets.Count; i++)
        {
            // Destroy if null or if the name doesn't match the expected format
            if (evadeTargets[i] == null || !evadeTargets[i].name.Equals("Temp Target"))
            {
                Destroy(evadeTargets[i]);
                evadeTargets.RemoveAt(i);
                i--; // Decrement index since the list has shortened
            }
        }
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

