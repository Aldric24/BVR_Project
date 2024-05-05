using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform target;
    public float followSpeed = 5.0f;

    private Vector3 offset;

    void Start()
    {
        // Calculate initial offset based on starting distance
        offset = transform.position - target.position;
    }

    void Update()
    {
        if (target != null)
        {
            // Calculate the desired position (target's XY, object's Z)
            Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

            // Option 1: Smooth movement using Vector3.Lerp
          

            // Option 2: Snappier Movement
            // transform.position = desiredPosition + offset; 

            // Maintain Z rotation
            transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z);
        }
    }
}
