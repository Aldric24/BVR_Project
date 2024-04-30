using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RWR : MonoBehaviour
{

    [SerializeField] private Transform system;
    private Transform sweepTransform;
    [SerializeField] private float rotationSpeed;
    private float radarDistance;
    [SerializeField] private GameObject popup;
    [SerializeField] private GameObject Missile;
    [SerializeField] Dictionary<GameObject, float> lastPingedTimes = new Dictionary<GameObject, float>();
    [SerializeField] List<GameObject> RWRObjects = new List<GameObject>();
    [SerializeField] Dictionary<GameObject, PopUp> RWRpings = new Dictionary<GameObject, PopUp>();
    private void Awake()
    {
        StartCoroutine(CheckForStaleRWRObjects());
    }
    private void Update()
    {
        transform.position = system.position;
        transform.rotation = system.rotation;
    }
    internal void Popup(GameObject gameObject)
    {
        if (!RWRObjects.Contains(gameObject)) // Check for duplicates
        {
            if(gameObject.name == "AIM120D")
            {
                RWRObjects.Add(gameObject);
                lastPingedTimes[gameObject] = Time.time;
                popup.GetComponent<PopUp>().system = system;
                PopUp missile = Instantiate(Missile, gameObject.transform.position, Quaternion.identity).GetComponent<PopUp>();

                RWRpings[gameObject] = missile;// Record ping time
            }
            else
            {
                RWRObjects.Add(gameObject);
                lastPingedTimes[gameObject] = Time.time;
                popup.GetComponent<PopUp>().system = system;
                PopUp radarPing = Instantiate(popup, gameObject.transform.position, Quaternion.identity).GetComponent<PopUp>();

                RWRpings[gameObject] = radarPing;// Record ping time
            }
        }
            
        else
        {
            lastPingedTimes[gameObject] = Time.time;

        }
        
        //Instantiate(popup, gameObject.transform.position, Quaternion.identity);
    }
    private IEnumerator CheckForStaleRWRObjects()
    {
        while (true)
        {
            foreach (var pair in lastPingedTimes.ToList()) // Use ToList to avoid modifying while iterating
            {
              
                //// Remove objects not detected in a full cycle 
                if (Time.time - pair.Value > 4f) // Adjust threshold as needed
                {

                    // Remove associated ping
                    if (RWRpings.ContainsKey(pair.Key))
                    {
                       
                        Destroy(RWRpings[pair.Key].gameObject); // Destroy the ping
                        RWRpings.Remove(pair.Key);
                    }
                    RWRObjects.Remove(pair.Key);
                    lastPingedTimes.Remove(pair.Key);
                   

                }


            }
            yield return new WaitForSeconds(1f); // Check every second
        }
    }
    

}
