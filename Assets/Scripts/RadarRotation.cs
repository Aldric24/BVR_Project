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

    
}
