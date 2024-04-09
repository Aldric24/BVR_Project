using UnityEngine;
using System.Collections.Generic;

public class WaypointManager : MonoBehaviour
{
    public GameObject waypointPrefab;
    public int numberOfWaypoints = 5;
    public float waypointAreaRadius = 20f; // Area for random placement
    public LineRenderer lineRenderer;
    public GameObject player;
    private List<Waypoint> waypoints = new List<Waypoint>();
    private int currentWaypointIndex = 0;

    void Start()
    {

        GenerateWaypoints();
        SetupLineRenderer();
    }

    void GenerateWaypoints()
    {
        for (int i = 0; i < numberOfWaypoints; i++)
        {
            Vector2 randomPosition = Random.insideUnitSphere * waypointAreaRadius;
            randomPosition += new Vector2(transform.position.x, transform.position.y); // Offset based on WaypointManager's position

            GameObject newWaypointObj = Instantiate(waypointPrefab, randomPosition, Quaternion.identity);
            if(i==0)
            {
                newWaypointObj.GetComponent<BoxCollider2D>().enabled = true;   
            }
            Waypoint waypoint = newWaypointObj.GetComponent<Waypoint>();

            // Setup Waypoint Properties
            waypoint.waypointManager = this;
            waypoint.name = "Waypoint " + (i + 1); // Assign a sequence number

            waypoints.Add(waypoint);
        }

    }

    void SetupLineRenderer()
    {
        lineRenderer.positionCount = waypoints.Count;
    }

    void Update()
    {
        UpdateLineRendererPositions();
    }

    void UpdateLineRendererPositions()
    {
        // Set the first position to the player's position
        

        if (currentWaypointIndex < waypoints.Count) // Check if there are waypoints left
        {
            int numPositions = waypoints.Count - currentWaypointIndex + 1; // Calculate positions needed (including player)
            lineRenderer.positionCount = numPositions;
            if (player != null)
            {
                lineRenderer.SetPosition(0, new Vector3(player.transform.position.x, player.transform.position.y, 0));
            }
            else
                return;
            


            // Set remaining positions based on waypoints from current index onwards
            for (int i = 1; i < numPositions; i++)
            {
                lineRenderer.SetPosition(i, waypoints[currentWaypointIndex + i - 1].transform.position);
            }
        }
        else // No waypoints left, hide the line renderer
        {
            lineRenderer.positionCount = 0;
        }
    }


    public void AddWaypoint(Waypoint newWaypoint)
    {
        waypoints.Add(newWaypoint);
    }

    public void WaypointReached(Waypoint reachedWaypoint)
    {
        // 1. Check if the reached waypoint is the final one
        if (reachedWaypoint.isEnd)
        {
            Debug.Log("All waypoints completed!");
            // ... your logic when all waypoints are reached
            return;  // End the waypoint task
        }

        // 2. Destroy Old Waypoint, Enable Next
        Destroy(reachedWaypoint.gameObject);  // Destroy the old waypoint
        currentWaypointIndex++;
        currentWaypointIndex %= waypoints.Count;

        if (currentWaypointIndex < waypoints.Count)
        {
            waypoints[currentWaypointIndex].gameObject.GetComponent<Collider2D>().enabled = true; // Assuming 2D collider
        }
    }
}   