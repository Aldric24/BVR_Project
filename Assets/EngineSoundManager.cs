using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EngineSoundManager : MonoBehaviour
{
    public AudioSource idleEngineSound;
    public AudioSource[] buildUpEngineSounds;
    public AudioSource afterburnerSound;

    public float minPitch = 0.5f;
    public float maxPitch = 2.0f;
    public float pitchChangeSpeed = 0.1f; // Adjust this for smoother transitions

    [SerializeField] Slider Slider; // Reference to your aircraft control script (if not already present)

    private float targetPitch;
    private int currentBuildUpIndex = 0;

    private void Update()
    {
        // Get throttle input (assuming a value between 0 and 1)
        float throttleInput = Slider.value; // Adjust based on your input system
        float enginePower = throttleInput;

        // Calculate target pitch based on throttle input
        targetPitch = minPitch + (maxPitch - minPitch) * throttleInput;

        // Gradually adjust pitch (smoother transition)
        idleEngineSound.pitch = Mathf.Lerp(idleEngineSound.pitch, targetPitch, pitchChangeSpeed * Time.deltaTime);
        idleEngineSound.volume = (throttleInput == Slider.minValue) ? 1f : 0f;  // Full volume at min, 0 otherwise
        afterburnerSound.volume = (throttleInput == Slider.maxValue) ? 1f : 0f; // Full volume at max, 0 otherwise
        // Crossfade between build-up sounds
        CrossfadeToBuildUpSound(enginePower);

        // Afterburner 
        //  (This part likely remains the same, using 'enginePower' >= 1.0f as the trigger)
    }

    void CrossfadeToBuildUpSound(float enginePower)
    {
        int targetIndex = Mathf.FloorToInt((buildUpEngineSounds.Length - 1) * enginePower);

        // Smooth crossfading (instead of abrupt changes)
        for (int i = 0; i < buildUpEngineSounds.Length; i++)
        {
            if (i == targetIndex)
            {
                buildUpEngineSounds[i].volume += pitchChangeSpeed * Time.deltaTime;
                buildUpEngineSounds[i].volume = Mathf.Clamp01(buildUpEngineSounds[i].volume); // Ensure it doesn't go above 1
            }
            else
            {
                buildUpEngineSounds[i].volume -= pitchChangeSpeed * Time.deltaTime;
                buildUpEngineSounds[i].volume = Mathf.Clamp01(buildUpEngineSounds[i].volume); // Ensure it doesn't go below 0
            }
        }
    }
}
