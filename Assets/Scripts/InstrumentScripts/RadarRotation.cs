using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RadarRotation : MonoBehaviour
{

    [SerializeField] private Transform system;
    private Transform sweepTransform;
    [SerializeField]private float rotationSpeed;
    private float radarDistance;
    private List<Collider2D> colliderList;
    [SerializeField]Camera radarcam;
  
    private void Update()
    {

        transform.position = system.position;
        transform.rotation = system.rotation;



    }
    public void WaypointScreen()
    {
        Debug.Log("Radar Culling masek " + radarcam.cullingMask);   
        if (radarcam.cullingMask != 256)
        {
            
            //radarcam.transform.position = new Vector3(radarcam.transform.position.x, radarcam.transform.position.y+10, radarcam.transform.position.z);
            radarcam.cullingMask =256;
        }
        else
            radarcam.cullingMask = 8;
        
    }
    
}
