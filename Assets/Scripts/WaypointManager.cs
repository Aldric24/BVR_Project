using UnityEngine;
using System.Collections.Generic;

public class WaypointManager : MonoBehaviour
{
    public List<Waypoint> waypoints = new List<Waypoint>();
    private int currentWaypointIndex = 0;

    // ... other variables if needed

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

        // 2. Advance to the next waypoint 
        currentWaypointIndex++;

        // Handle potential wrap-around if we reach the end of the list
        currentWaypointIndex %= waypoints.Count;

        // 3. Do something to indicate the new active waypoint 
        // ... (Logic for updating line renderer, UI, etc.)
    }
}