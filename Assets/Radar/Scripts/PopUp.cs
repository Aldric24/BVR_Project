using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PopUp : MonoBehaviour
{
    [SerializeField] private Transform system;
    [SerializeField] private GameObject popUp;
    // Update is called once per frame

    void Update()
    {
       transform.rotation=system.rotation;  

    }
}
