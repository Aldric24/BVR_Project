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
        lastPosition = rigidbody.position;
        // This stays the same, but will store Vector2 now
    }
    //    rigidbody.AddRelativeForce(new Vector3(0, 0, speed), ForceMode.VelocityChange);
    //    rigidbody.AddForce(owner.GetComponent<Rigidbody2D>().velocity, ForceMode.VelocityChange);
    //    lastPosition = rigidbody.position;
    //}

    void FixedUpdate() {
        if (Time.time > startTime + lifetime) {
            Destroy(gameObject);
            return;
        }

        //var diff = rigidbody.position - lastPosition;
        //lastPosition = rigidbody.position;

        //float distanceTravelled = diff.magnitude;
        //int numberOfRaycasts = 4; // Adjust as needed for precision
        //float raycastInterval = distanceTravelled / numberOfRaycasts;

        //for (int i = 0; i < numberOfRaycasts; i++)
        //{
        //    Vector3 rayStart = lastPosition + diff.normalized * (i * raycastInterval);
        //    Ray ray = new Ray(rayStart, diff.normalized);
            
        //    RaycastHit hit;
        //    if (Physics.Raycast(ray, out hit, raycastInterval, collisionMask.value))
        //    {
        //        Plane other = hit.collider.GetComponent<Plane>();

        //        Debug.Log("Hit " + hit.collider.gameObject.name);

        //        Destroy(gameObject);
        //    }
        //}
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit " + other.gameObject.name);
        if (other.gameObject.CompareTag("Adversary")) // Or your preferred check 
        {
           
            Debug.Log("Hit " + other.gameObject.name);
            Destroy(gameObject);
        }
    }
}
