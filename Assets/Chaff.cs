using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chaff : MonoBehaviour
{
    public float lifetime = 5f;
    public float movementSpeed = 2f;
    public float decayRate = 0.5f; // Intensity drop per second
    private float startTime;
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {

        // Simple downward movement
        transform.Translate(Vector3.down * movementSpeed * Time.deltaTime);

        // Destroy after lifetime
        if (Time.time > startTime + lifetime)
        {
            Destroy(gameObject);
        }
    }
}
