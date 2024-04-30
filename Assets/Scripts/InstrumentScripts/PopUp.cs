using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PopUp : MonoBehaviour
{
    public Transform system;
    [SerializeField] private GameObject popUp;
    // Update is called once per frame
    private void Start()
    {
        
    }
    void Update()
    {
       transform.rotation=system.rotation;  

    }
}
