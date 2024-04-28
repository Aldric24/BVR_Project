using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    [SerializeField]
    float damage;
    [SerializeField]
    float lifetime;
    [SerializeField]
    float speed;
    [SerializeField]
    LayerMask collisionMask;
    [SerializeField]
    float width;
    [SerializeField] float detectionRadius;
    public GameObject owner;
    new Rigidbody2D rigidbody;
    Vector3 lastPosition;
    float startTime;

    //public void Fire(GameObject owner) {
    //    this.owner = owner;
    //    rigidbody = GetComponent<Rigidbody>();
    //    startTime = Time.time;
    private void Start()
    {
        Debug.DrawLine(transform.position, transform.position + new Vector3(0, speed), Color.red, 0.5f);
        startTime = Time.time;
        rigidbody = GetComponent<Rigidbody2D>(); // Change to Rigidbody2D
        rigidbody.AddRelativeForce(new Vector2(1, -speed), (ForceMode2D)ForceMode.VelocityChange); // Change to Vector2
        rigidbody.AddForce(owner.GetComponent<Rigidbody2D>().velocity, (ForceMode2D)ForceMode.VelocityChange);
        //float distanceToTravel = speed * Time.deltaTime;
        //RaycastHit2D hit = Physics2D.Raycast(lastPosition, transform.up, distanceToTravel, collisionMask);

        //if (hit.collider != null)
        //{
        //    Debug.Log("Hit: " + hit.collider.name);
        //    // Handle collision here
        //}

        //lastPosition = transform.position;
        // This stays the same, but will store Vector2 now
    }
    

    void FixedUpdate() {
        if (Time.time > startTime + lifetime) {
            Destroy(gameObject);
            return;
        }
        FindHeatTarget();
        float distanceToTravel = speed * Time.deltaTime;
        RaycastHit2D hit = Physics2D.Raycast(lastPosition, transform.up, distanceToTravel, collisionMask);

        if (hit.collider != null)
        {
            Debug.Log("Hit: " + hit.collider.name);
            // Handle collision here
        }

        lastPosition = transform.position;
        //This stays the same, but will store Vector2 now

    }

    private void FindHeatTarget()
    {
        Collider2D detectedColliders = Physics2D.OverlapCircle(transform.position, detectionRadius, collisionMask);
        if(detectedColliders != null)
        {
            Debug.Log("Targets hit: " + detectedColliders.name);
            if(detectedColliders.tag=="Adversary")
            {
                Destroy(detectedColliders.gameObject);
            }
        }
        
       
    }
    void OnDrawGizmos()
    {
        if (enabled)  // Only draw gizmo when the script is active
        {
            Gizmos.color = Color.yellow; // Adjust color as you like
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}
