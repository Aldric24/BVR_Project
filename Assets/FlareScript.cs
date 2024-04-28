using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlareScript : HeatSource
{
    public float lifetime = 5f;
    public float maxHeatIntensity;  // Set higher than regular targets
    public float movementSpeed = 2f;
    public float decayRate = 0.5f; // Intensity drop per second

    private HeatSource heatSource;
    private float startTime;

    void Start()
    {
        heatSource = GetComponent<HeatSource>();
        heatSource.heatIntensity = maxHeatIntensity;
        startTime = Time.time;
    }

    void Update()
    {
        // Decay heat intensity over time
        heatSource.heatIntensity = Mathf.Lerp(maxHeatIntensity, 0f, (Time.time - startTime) / lifetime);

        // Simple downward movement
        transform.Translate(Vector3.down * movementSpeed * Time.deltaTime);

        // Destroy after lifetime
        if (Time.time > startTime + lifetime)
        {
            Destroy(gameObject);
        }
    }
}
