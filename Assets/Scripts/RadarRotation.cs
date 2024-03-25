using CodeMonkey;
using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Compilation;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UI;

public class RadarRotation : MonoBehaviour
{
    [SerializeField] private Transform pfRadarPing;
    [SerializeField] private GameObject infoPopUpPrefab;
    [SerializeField] private LayerMask radarLayerMask;
    [SerializeField] private Transform system;
    private Transform sweepTransform;
    [SerializeField]private float rotationSpeed;
    private float radarDistance;
    private List<Collider2D> colliderList;
    public GameObject sweep;
    public GameObject Rtrail;
    public GameObject ltrail;
    private void Awake()
    {
       sweepTransform = sweep.transform;
       //StartCoroutine(SweepRotationCoroutine());
       //StartCoroutine(RadarSweep());
        radarDistance = 150f;
        colliderList = new List<Collider2D>();
    }

    private void Update()
    {
        transform.position = system.position;
        transform.rotation = system.rotation;



    }

    private IEnumerator SweepRotationCoroutine()
    {
        while (true)  // Run indefinitely
        {
            transform.position = system.position;
            transform.rotation = system.rotation;
            sweep.transform.localPosition = new Vector3(Mathf.PingPong(Time.time * rotationSpeed, 300f) - 150f, 0, 0);
           

            yield return null; // Wait until the next frame
        }
    }
    private IEnumerator RadarSweep()
    {
       while(true) 
       {
            Vector2 raycastOrigin = sweepTransform.position;
            Vector2 raycastDirection = UtilsClass.GetVectorFromAngle(sweepTransform.eulerAngles.z);

            // Perform the raycast
            RaycastHit2D[] detecD = Physics2D.RaycastAll(raycastOrigin, raycastDirection, radarDistance, radarLayerMask);
            foreach (RaycastHit2D raycastHit2D in detecD)
            {
                if (raycastHit2D.collider == null) continue;
                RadarPing radarPing = Instantiate(pfRadarPing, raycastHit2D.point, Quaternion.identity).GetComponent<RadarPing>();

            }
            //if (raycastHit2D.collider != null)
            //{
            //    Debug.Log("Hit: " + raycastHit2D.collider.name);
            //    RadarPing radarPing = Instantiate(pfRadarPing, raycastHit2D.point, Quaternion.identity).GetComponent<RadarPing>();
            //}

            //RaycastHit2D[] raycastHit2DArray = Physics2D.RaycastAll(transform.localPosition, UtilsClass.GetVectorFromAngle(sweepTransform.eulerAngles.z), radarDistance, radarLayerMask);

            //// Use a HashSet for efficient checking
            //var detectedColliders = new HashSet<Collider2D>();

            //foreach (RaycastHit2D raycastHit2D in raycastHit2DArray)
            //{
            //    if (raycastHit2D.collider == null) continue;

            //    if (!detectedColliders.Contains(raycastHit2D.collider))
            //    {
            //        detectedColliders.Add(raycastHit2D.collider);

            //        RadarPing radarPing = Instantiate(pfRadarPing, raycastHit2D.point, Quaternion.identity).GetComponent<RadarPing>();

            //    }
            //}
            if (Input.GetKeyDown(KeyCode.T))
            {
                rotationSpeed += 20;
                Debug.Log("rotationSpeed: " + rotationSpeed);
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                rotationSpeed -= 20;
                Debug.Log("rotationSpeed: " + rotationSpeed);
            }

            yield return null;
       }
    }
}
