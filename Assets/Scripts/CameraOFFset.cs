using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraOFFset : MonoBehaviour
{
    public Transform target; // The object to follow

    public float distance = 5f; // Adjust this to control the distance of the camera from the target object

    void LateUpdate()
    {
        // Check if the target is assigned
        if (target != null)
        {
            // Calculate the desired position of the camera
            Vector3 desiredPosition = target.position;
            desiredPosition.z = target.position.z - target.position.z * distance;

            // Set the position of the camera
            transform.position = desiredPosition;

            // Optionally, you can reset the camera's rotation to a fixed value
            // transform.rotation = Quaternion.identity; // This keeps the rotation unchanged

            // Alternatively, you can set the camera's rotation to match the initial rotation
            // transform.rotation = Quaternion.Euler(Vector3.zero); // This sets the rotation to zero degrees
        }
    }
}
