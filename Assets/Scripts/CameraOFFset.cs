using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraOFFset : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = Vector3.zero; // Offset from the target position

    void LateUpdate()
    {
        if (target != null)
        {
            // Calculate the desired position with offset
            Vector3 desiredPosition = target.position + offset;

            // Set the position of the camera directly
            transform.position = desiredPosition;
        }
    }
}