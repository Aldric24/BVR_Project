using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolWaypoint : MonoBehaviour
{
    public float radius = 10f;
    public Vector3 point;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
