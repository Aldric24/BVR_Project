using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class SweepRotation : MonoBehaviour
{
    [SerializeField] private Transform pfRadarPing;
    [SerializeField] private GameObject infoPopUpPrefab;
    [SerializeField] private LayerMask radarLayerMask;
    [SerializeField] private float rotationSpeed;
    private float radarDistance;
    private List<Collider2D> colliderList;
    [SerializeField] private GameObject spriteExtreme;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] spriteExtremeA;
    public GameObject RangeText;
    public GameObject RadarCursor;
    public GameObject Bearing;
    public List<GameObject> RadarObjects;
    [SerializeField] private Dictionary<GameObject, float> lastPingedTimes = new Dictionary<GameObject, float>();
    [SerializeField] private Dictionary<GameObject, RadarPing> radarPings = new Dictionary<GameObject, RadarPing>();
    bool isAtLeftExtreme;
    bool isAtRightExtreme;
    private bool wasAtLeftExtreme;
    private bool wasAtRightExtreme;
    float position;
    [SerializeField] private GameObject LocekdTarget;
    private void Awake()
    {
        spriteRenderer = spriteExtreme.GetComponent<SpriteRenderer>();
        StartCoroutine(SweepRotationCoroutine());
        StartCoroutine(CheckForStaleObjects());
        //StartCoroutine(pingradarcontacts());
        //StartCoroutine(RadarSweep());
        radarDistance = 150f;
        //colliderList = new List<Collider2D>();
    }
    private IEnumerator SweepRotationCoroutine()
    {
        while (true)  // Run indefinitely
        {
             position = Mathf.PingPong(Time.time * rotationSpeed, 300f) - 150f;

            // Sprite Switching Logic
            if (position <= -145f) // Check if near left extreme (adjust threshold as needed)
            {
                spriteRenderer.sprite = spriteExtremeA[0];
                spriteRenderer.flipY = true;
            }
            else if (position >= 145f) // Check if near right extreme
            {
                //flipo the sprite on x axis
                spriteRenderer.flipY = false;

            }

            // Update extreme flags
            isAtLeftExtreme = position <= -145f;
            isAtRightExtreme = position >= 145f;

            // Check for full rotation and trigger actions (if needed)
            if (isAtLeftExtreme && !wasAtLeftExtreme) // Just passed left extreme
            {
                // Full rotation completed (modify as needed)
                Debug.Log("Full Rotation Completed!");
                // You can call CycleAndLockOnTarget() or perform other full-rotation actions here.
                wasAtLeftExtreme = true;
            }
            else if (isAtRightExtreme && !wasAtRightExtreme) // Just passed right extreme
            {
                wasAtRightExtreme = true;
            }

            wasAtLeftExtreme = isAtLeftExtreme; // Update previous state for next frame
            wasAtRightExtreme = isAtRightExtreme;

            transform.localPosition = new Vector3(position, 0, 0);

            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision");
        if(collision.gameObject.tag=="Player")
        {
            Debug.Log("Player Detected");
            return;
        }
        
        if (!RadarObjects.Contains(collision.gameObject)) // Check for duplicates
        {

            RadarObjects.Add(collision.gameObject);
            lastPingedTimes[collision.gameObject] = Time.time;
            RadarPing radarPing = Instantiate(pfRadarPing, collision.gameObject.transform.position, Quaternion.identity).GetComponent<RadarPing>();

            radarPings[collision.gameObject] = radarPing;// Record ping time
        }
        else
        {
            lastPingedTimes[collision.gameObject] = Time.time;

        }
        

        
    }

    private IEnumerator CheckForStaleObjects()
    {
        while (true)
        {
            foreach (var pair in lastPingedTimes.ToList()) // Use ToList to avoid modifying while iterating
            {
                // Remove objects not detected in a full cycle


                //// Remove objects not detected in a full cycle 
                if (Time.time - pair.Value > 4f) // Adjust threshold as needed
                {

                    // Remove associated ping
                    if (radarPings.ContainsKey(pair.Key))
                    {
                        Destroy(radarPings[pair.Key].gameObject); // Destroy the ping
                        radarPings.Remove(pair.Key);
                    }
                    RadarObjects.Remove(pair.Key);
                    lastPingedTimes.Remove(pair.Key);
                    if (pair.Key == LocekdTarget)
                    {
                        LocekdTarget = null;
                    }

                }
               

            }
            yield return new WaitForSeconds(1f); // Check every second
        }
    }
    int targetIndex = 0;
    public void CycleTarget()
    {   

        // 1. Check if there are targets
        if (RadarObjects.Count == 0)
        {
            Debug.Log("No targets detected!");
            return;
        }
        
        if (radarPings.ContainsKey(RadarObjects[targetIndex])) // Adjust for zero-based indexing
        {

            radarPings[RadarObjects[targetIndex]].pingHighlight.SetActive(false);
            radarPings[RadarObjects[targetIndex]].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }
        // 2. Manage Target Index
        targetIndex = (targetIndex + 1) % RadarObjects.Count; // Cycle through targets

        // 3. Disable old highlight (if any)
        

        // 4. Enable new highlight
        if (radarPings.ContainsKey(RadarObjects[targetIndex]))
        {
            RangeText.GetComponent<Text>().text ="Range: " + Vector3.Distance(transform.position, RadarObjects[targetIndex].transform.position).ToString("F2") + "m";
            Bearing.GetComponent<Text>().text = "Bearing: " + (transform.position.x - RadarObjects[targetIndex].transform.position.x).ToString("F2") + "m";
            radarPings[RadarObjects[targetIndex]].pingHighlight.SetActive(true);
        }
    }
    public void LockOnTarget()
    {
        // 1. Check if there are targets
        if (RadarObjects.Count == 0)
        {
            Debug.Log("No targets detected!");
            return;
        }

        // 2. Enable highlight
        if (radarPings.ContainsKey(RadarObjects[targetIndex]))
        {
            LocekdTarget = RadarObjects[targetIndex];
            GameObject tgt = radarPings[RadarObjects[targetIndex]].pingHighlight;
            if(tgt.activeSelf)
            {
                tgt.SetActive(false);
                radarPings[RadarObjects[targetIndex]].GetComponent<SpriteRenderer>().color = new Color(1, 0, 1, 1);
            }
            

        }
    }
    public void deselect()
    {  
        if (radarPings.ContainsKey(RadarObjects[targetIndex]))
        {
            LocekdTarget=null;
            radarPings[RadarObjects[targetIndex]].pingHighlight.SetActive(false);
            radarPings[RadarObjects[targetIndex]].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }
    }
}