using CodeMonkey;
using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadarRotation : MonoBehaviour
{
    [SerializeField] private Transform pfRadarPing;
    [SerializeField] private GameObject infoPopUpPrefab;
    [SerializeField] private LayerMask radarLayerMask;
    [SerializeField] private Transform system;
    [SerializeField] private Sprite sweepSprite;
    private Transform sweepTransform;
    private float rotationSpeed;
    private float radarDistance;
    private List<Collider2D> colliderList;

    private void Awake()
    {
        sweepTransform = transform.Find("Sweep");
        rotationSpeed = 180f;
        radarDistance = 150f;
        colliderList = new List<Collider2D>();
    }

    private void Update()
    {
        transform.position=system.position;
        transform.rotation=Quaternion.Euler(0,0,-system.rotation.eulerAngles.z);
        // Adjust your previous/current rotation calculations if needed
        float previousRotation = (sweepTransform.eulerAngles.z % 180);
        sweepTransform.eulerAngles -= new Vector3(0, 0, rotationSpeed * Time.deltaTime);
        float currentRotation = (sweepTransform.eulerAngles.z % 180);

        // Ensure currentRotation is always within 0-360 for smoother clamping
        if (currentRotation < 0)
        {
            currentRotation += 360;
        }

        // Clamp and reverse rotation
        float targetRotation = Mathf.Clamp(currentRotation, 0, 180);
        if (targetRotation >= 180 || targetRotation <= 0)
        {
            rotationSpeed *= -1;
        }
        sweepTransform.eulerAngles = new Vector3(0, 0, targetRotation);

        // ... rest of your code ...

        // Assign the sprite (assuming sweepTransform has a SpriteRenderer)
        if (sweepTransform.GetComponent<SpriteRenderer>() != null)
        {
            sweepTransform.GetComponent<SpriteRenderer>().sprite = sweepSprite;
        }
        if (previousRotation < 0 && currentRotation >= 0)
        {
            // Half rotation
            colliderList.Clear();
        }

        RaycastHit2D[] raycastHit2DArray = Physics2D.RaycastAll(transform.position, UtilsClass.GetVectorFromAngle(sweepTransform.eulerAngles.z), radarDistance, radarLayerMask);
        foreach (RaycastHit2D raycastHit2D in raycastHit2DArray)
        {
            if (raycastHit2D.collider != null)
            {
                RadarPing radarPing = Instantiate(pfRadarPing, raycastHit2D.point, Quaternion.identity).GetComponent<RadarPing>();
                radarPing.SetDisappearTimer(360f / rotationSpeed * 1f);
                //radarPing.transform.SetParent(transform);
                // Hit something
                if (!colliderList.Contains(raycastHit2D.collider))
                {
                    // Hit this one for the first time
                    colliderList.Add(raycastHit2D.collider);
                    //CMDebug.TextPopup("Ping!", raycastHit2D.point);
                    //RadarPing radarPing = Instantiate(pfRadarPing, raycastHit2D.point, Quaternion.identity).GetComponent<RadarPing>();
                    //radarPing.transform.SetParent(transform);
                    if (raycastHit2D.collider.gameObject.GetComponent<ItemHandler>() != null)
                    {
                        // Hit an Item
                        radarPing.SetColor(new Color(0, 1, 0));
                    }
                    if (raycastHit2D.collider.gameObject.GetComponent<CharacterWaypointsHandler>() != null)
                    {
                        // Hit an Enemy
                        radarPing.SetColor(new Color(1, 0, 0));
                    }
                    //radarPing.SetDisappearTimer(360f / rotationSpeed * 1f);
                    //colliderList.Add(raycastHit2D.collider);

                }
            }
        }

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
    }
}
