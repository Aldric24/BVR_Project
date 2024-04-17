using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCam : MonoBehaviour
{
    public List<Camera> cameras; // List to store your cameras
    private int currentCameraIndex = 0; // Index of the currently active camera

    void Start()
    {
        if (cameras == null || cameras.Count == 0)
        {
            Debug.LogError("No cameras assigned to CameraSwitcher!");
            return;
        }

        // Set the first camera as active by default
        cameras[currentCameraIndex].enabled = true;
    }

    public void SwitchCamera()
    {
        // Disable the currently active camera
        cameras[currentCameraIndex].enabled = false;

        // Increment the index, looping back to the first camera if necessary
        currentCameraIndex = (currentCameraIndex + 1) % cameras.Count;

        // Enable the new active camera
        cameras[currentCameraIndex].enabled = true;
    }   
}
