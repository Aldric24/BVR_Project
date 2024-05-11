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

    [SerializeField] private AudioSource missileWarningSource;
    [SerializeField] private AudioSource radarPingSource;
    private void Awake()
    {
        StartCoroutine(CheckForStaleRWRObjects());
        StartCoroutine(CheckforENemyLock());
    }
    private void Update()
    {
        if(system != null)
        {
            transform.position = system.position;
            transform.rotation = system.rotation;
        }
       
    }
    internal void Popup(GameObject Popup)
    {
        if (!RWRObjects.Contains(Popup)) // Check for duplicates
        {
            if(Popup.tag == "Missile")
            {
                RWRObjects.Add(Popup);
                lastPingedTimes[Popup] = Time.time;
                popup.GetComponent<PopUp>().system = system;
                PopUp missile = Instantiate(Missile, Popup.transform.position, Quaternion.identity).GetComponent<PopUp>();
                missile.system = gameObject.transform;
                RWRpings[Popup] = missile;// Record ping time
                StartCoroutine(PlayMissileWarningSound());
            }
            else
            {
                RWRObjects.Add(Popup);
                lastPingedTimes[Popup] = Time.time;
                PopUp radarPing = Instantiate(popup, Popup.transform.position, Quaternion.identity).GetComponent<PopUp>();
                radarPing.system = gameObject.transform;
                radarPing.gameObject.transform.parent = gameObject.transform.parent;
                RWRpings[Popup] = radarPing;// Record ping time
            }
        }
            
        else
        {
            lastPingedTimes[Popup] = Time.time;
            RWRpings[Popup].transform.position = Popup.transform.position;

        }
        
        //Instantiate(popup, Popup.transform.position, Quaternion.identity);
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
    IEnumerator CheckforENemyLock()
    {
        foreach (var pair in lastPingedTimes.ToList()) // Use ToList to avoid modifying while iterating
        {

            if(pair.Key!=null)
            {
                
                //// Remove objects not detected in a full cycle 
                if (pair.Key.GetComponentInParent<EnemyAI>() != null && pair.Key.GetComponentInParent<EnemyAI>().target == gameObject.transform.parent) // Adjust threshold as needed
                {
                    Debug.Log("Enemy Locked");
                    // Remove associated ping
                    if (RWRpings.ContainsKey(pair.Key))
                    {

                        RWRpings[pair.Key].GetComponentInChildren<SpriteRenderer>().gameObject.SetActive(true); // Destroy the ping
                    }
                    RWRObjects.Remove(pair.Key);
                    lastPingedTimes.Remove(pair.Key);


                }
            }


        }
        yield return new WaitForSeconds(1f); // Check every second
    }
    private IEnumerator PlayMissileWarningSound()
    {
        while (true)
        {
            if (RWRObjects.Any(obj => obj.tag == "AdversaryMissile"))
            {
                if(!missileWarningSource.isPlaying )
                {
                    missileWarningSource.Play();
                }
                if (!radarPingSource.isPlaying)
                {
                    radarPingSource.Play();
                }
                yield return new WaitForSeconds(1f); // Adjust the alert interval as needed
            }
            else
            {
                // Stop the alert when no missiles are detected
                missileWarningSource.Stop();
                radarPingSource.Stop();
                yield break; // Exit the coroutine
            }
        }
    }

}
