using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrottleScript : MonoBehaviour
{
    public Rigidbody planeRigidbody;
    public Slider throttleSlider;
    public float minThrottle = 0f;
    public float maxThrottle = 100f;
    public float throttleSensitivity = 10f;
    public float maxSpeed = 20f;

    void Start()
    {
        // Set initial throttle value
        throttleSlider.value = maxThrottle;
    }

    void Update()
    {
        // Adjust throttle based on slider value
        float throttle = Mathf.Lerp(minThrottle, maxThrottle, throttleSlider.value / throttleSlider.maxValue);
        ApplyThrottle(throttle);
    }

    void ApplyThrottle(float throttle)
    {
        // Calculate speed based on throttle value
        float targetSpeed = Mathf.Lerp(0f, maxSpeed, throttle / maxThrottle);

        // Apply constant forward thrust
        planeRigidbody.AddRelativeForce(Vector3.back * targetSpeed * throttleSensitivity * Time.deltaTime);
    }
}
