using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

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
    [SerializeField]private Dictionary<GameObject, float> lastPingedTimes = new Dictionary<GameObject, float>();
  
    private void Awake()
    {
        spriteRenderer =spriteExtreme.GetComponent<SpriteRenderer>();
        StartCoroutine(SweepRotationCoroutine());
        StartCoroutine(CheckForStaleObjects());
        //StartCoroutine(RadarSweep());
        radarDistance = 150f;
        //colliderList = new List<Collider2D>();
    }
    private IEnumerator SweepRotationCoroutine()
    {
        while (true)  // Run indefinitely
        {
            float position = Mathf.PingPong(Time.time * rotationSpeed, 300f) - 150f;

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

            transform.localPosition = new Vector3(position, 0, 0);

            yield return null; // Wait until the next frame
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision");
        RadarPing radarPing = Instantiate(pfRadarPing, collision.gameObject.transform.position, Quaternion.identity).GetComponent<RadarPing>();

        if (!RadarObjects.Contains(collision.gameObject)) // Check for duplicates
        {
            
            RadarObjects.Add(collision.gameObject);
            lastPingedTimes[collision.gameObject] = Time.time; // Record ping time
        }
    }

    private IEnumerator CheckForStaleObjects()
    {
        while (true)
        {
            foreach (var pair in lastPingedTimes.ToList()) // Use ToList to iterate over a copy
            {
                if (Time.time - pair.Value > 5f) // Adjust the threshold as needed
                {
                    RadarObjects.Remove(pair.Key);
                    lastPingedTimes.Remove(pair.Key);
                }
            }
            yield return new WaitForSeconds(1f); // Check every second
        }
    }
    public void CycleAndLockOnTarget()
    {
        // 1. Check if there are targets
        if (RadarObjects.Count == 0)
        {
            Debug.Log("No targets detected!");
            return;
        }

        // 2. Manage Target Index
        int targetIndex = 0; // Start at the first target
        targetIndex = (targetIndex + 1) % RadarObjects.Count; // Cycle through targets

        // 3. Move Radar Cursor
        GameObject targetObject = RadarObjects[targetIndex];
        //add a buffer of 10 to the x axis to make the cursor appear on the edge of the object
        StartCoroutine(updatecursor(targetObject));
        // 4. Move Sweep to Target Position
        
        // You'll need logic here to adjust the sweep position for smooth transition,
        // potentially using coroutines or tweening libraries like LeanTween.
    }
    IEnumerator updatecursor(GameObject target)
    {
        while (target)
        {
            yield return new WaitForSeconds(0.1f);

            //add a buffer of 10 to the x axis to make the cursor appear on the edge of the object
            RadarCursor.transform.position = new Vector3(target.transform.position.x + 1, target.transform.position.y + 20, target.transform.position.z);
        }
        
    }
}