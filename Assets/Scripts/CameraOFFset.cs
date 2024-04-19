using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraOFFset : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = Vector3.zero;
    public float moveDuration = 1.5f;

    private bool foundPlayer = false;
    private Camera cameraComponent;
    private bool isFollowing = false;

    void Start()
    {
        cameraComponent = GetComponent<Camera>();
    }

    void Update()
    {
        if (!foundPlayer)
        {
            FindPlayer();
        }
        else if (isFollowing)
        { // Only follow if not already following
            FollowPlayer();
        }
    }

    void FindPlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");       ;
        if (player != null)
        {
            GameObject aicraft=player.GetComponentInChildren<NewControl>().gameObject;
            target = aicraft.transform;
            foundPlayer = true;
            StartMove();
            
        }
    }

    void StartMove()
    {
        Vector3 desiredPosition = target.position + offset;
        LeanTween.move(gameObject, desiredPosition, moveDuration)
                 .setEaseInOutSine()
                 .setOnComplete(() => isFollowing = true); // Set flag on completion
    }

    void FollowPlayer()
    {
        Vector3 desiredPosition = target.position + offset;
        transform.position = desiredPosition; // Continuously update position
    }
}