using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //public float maxThrustForce = 10f; // Maximum upward force based on slider
    //public Slider throttleSlider; // Reference to the UI slider for throttle control
    //public float turnSpeed = 100f; // Rotation speed in degrees per second
    //public int maxSpeed = 10;
    //private Rigidbody rb;

    //void Start()
    //{
    //    rb = GetComponent<Rigidbody>();
    //    // Ensure neutral starting rotation

    //}

    //void Update()
    //{
    //    // Apply upward force based on slider value



    //    // Rotate on Z-axis based on input (optional)
    //    //get accelerometer input
    //    float horizontal = Input.acceleration.x; // Assuming vertical input for Z-axis rotation
    //    //float horizontal = Input.GetAxis("Horizontal"); // Assuming vertical input for Z-axis rotation
    //    if (horizontal != 0)
    //    {
    //        float rotationAmount = horizontal* turnSpeed * Time.deltaTime;
    //        transform.Rotate(Vector3.back, rotationAmount);
    //    }

    //    Vector3 finalForward = transform.rotation * UpdateVectorXYBasedOnZRotation(Vector3.) ; // Get final forward after Z-axis rotation

    //    // Project the final forward direction onto the XZ plane to eliminate Y-axis movement
    //    finalForward = Vector3.ProjectOnPlane(finalForward, Vector3.up);

    //    // Apply normalized thrust force in the corrected forward direction
    //    float currentThrustForce = throttleSlider.value * maxThrustForce;
    //    Vector3 thrustForce = finalForward * currentThrustForce;
    //    rb.AddForce(thrustForce);

    //    // Limit maximum speed
    //    rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);

    //    Debug.Log("Applied force: " + thrustForce);
    //    Debug.Log("Current velocity: " + rb.velocity);
    //}
    //public static Vector3 UpdateVectorXYBasedOnZRotation(Vector3 originalVector, float zRotation)
    //{
    //    // Convert rotation angle to radians
    //    float radians = Mathf.Deg2Rad * zRotation;

    //    // Calculate sine and cosine of the rotation angle
    //    float sinTheta = Mathf.Sin(radians);
    //    float cosTheta = Mathf.Cos(radians);

    //    // Extract original X and Y values
    //    float originalX = originalVector.x;
    //    float originalY = originalVector.y;

    //    // Update X and Y based on rotation
    //    float newX = originalX * cosTheta - originalY * sinTheta;
    //    float newY = originalX * sinTheta + originalY * cosTheta;

    //    // Return the updated vector
    //    return new Vector3(newX, originalVector.z, newY); // Preserve original Z value
    //}

    // Public variables for flexibility and potential editor changes
    // Public variables for flexibility and potential editor changes
    // Public variables for flexibility and potential editor changes
    // Public variables for flexibility and potential editor changes
    public Transform aircraftTransform; // Reference to the aircraft's transform component
    public Rigidbody2D aircraftRigidbody; // Use Rigidbody2D in 2D
    public Slider throttleSlider;       // Reference to the throttle UI slider

    public float maxThrustForce = 10f;  // Maximum thrust force
    public float moveSpeed = 5f;        // Base speed multiplier
    public float rotationSpeed = 5f;   // Adjusted rotation speed (slower)

    void Start()
    {
        // Get the required components (assuming they are attached to this object)
        aircraftTransform = GetComponent<Transform>();
        aircraftRigidbody = GetComponent<Rigidbody2D>(); // Use Rigidbody2D
    }

    void Update()
    {
        // Get user input (ignore vertical input for thrust)
        float horizontalInput = Input.GetAxis("Horizontal");
        float throttleInput = throttleSlider.value; // Get throttle value (0 - 1)

        // Combine rotation and movement into a single step
        MoveAndRotateAircraft(horizontalInput, throttleInput);
    }

    // Rotates the aircraft model and applies thrust simultaneously
    void MoveAndRotateAircraft(float rotationInput, float throttleInput)
    {
        // Calculate desired rotation angle based on input
        float desiredRotation = rotationInput * rotationSpeed * Time.deltaTime;

        // Rotate the aircraft and apply thrust based on facing direction
        aircraftTransform.Rotate(0, 0, desiredRotation);
        Vector2 thrustDirection = aircraftTransform.up * throttleInput;
        aircraftRigidbody.AddForce(thrustDirection * maxThrustForce * moveSpeed);
    }
}
