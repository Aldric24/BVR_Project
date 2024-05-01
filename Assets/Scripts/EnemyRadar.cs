using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyRadar : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float radarDistance = 150f;
    [SerializeField] private LayerMask radarLayerMask;
    [SerializeField] private GameObject spriteExtreme;

    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] spriteExtremeA;

    public List<GameObject> RadarObjects;
    private Dictionary<GameObject, float> lastPingedTimes;
    public GameObject LockedTarget = null;
    private float position;
    private bool isAtLeftExtreme, isAtRightExtreme;
    private bool wasAtLeftExtreme, wasAtRightExtreme;

    private void Awake()
    {
        //spriteRenderer = spriteExtreme.GetComponent<SpriteRenderer>();
        StartCoroutine(SweepRotationCoroutine());
        StartCoroutine(CheckForStaleObjects());
        RadarObjects = new List<GameObject>();
        lastPingedTimes = new Dictionary<GameObject, float>();
    }

    private IEnumerator SweepRotationCoroutine()
    {
        while (true)  // Run indefinitely
        {
            position = Mathf.PingPong(Time.time * rotationSpeed, 300f) - 150f;


            transform.localPosition = new Vector3(position, 0, 0);
            
            yield return null;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {

        if (!RadarObjects.Contains(collision.gameObject) )
        {
            RadarObjects.Add(collision.gameObject);
            lastPingedTimes[collision.gameObject] = Time.time;
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
            if(RadarObjects.Count > 0)
            {
                foreach (var pair in lastPingedTimes.ToList()) // Use ToList to avoid modifying while iterating
                {
                    // Remove objects not detected in a full cycle


                    //// Remove objects not detected in a full cycle 
                    if (Time.time - pair.Value > 4f) // Adjust threshold as needed
                    {


                        RadarObjects.Remove(pair.Key);
                        lastPingedTimes.Remove(pair.Key);

                    }


                }
            }
            
            yield return new WaitForSeconds(1f); // Check every second
        }
    }

}

