using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public WaypointManager waypointManager;
    public bool isStart = false;
    public bool isEnd = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Waypoint Reached!
            waypointManager.WaypointReached(this);
        }
    }
}
