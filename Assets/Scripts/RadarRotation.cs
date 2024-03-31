using CodeMonkey;
using CodeMonkey.Utils;
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
  
    private void Update()
    {
        transform.position = system.position;
        transform.rotation = system.rotation;



    }

    
}
