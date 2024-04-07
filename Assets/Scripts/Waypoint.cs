using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public WaypointManager waypointManager;
    public bool isStart = false;
    public bool isEnd = false;

    private void Start()
    {
        waypointManager = FindFirstObjectByType<WaypointManager>();   
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("collided with Waypoint");
        if (collision.gameObject.CompareTag("Player"))
        {
            // Waypoint Reached!
            waypointManager.WaypointReached(this);
        }
    }
    
    
    
}
