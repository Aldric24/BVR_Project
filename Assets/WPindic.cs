using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WPindic : MonoBehaviour
{
    [SerializeField] private Transform system;
   

    private void Update()
    {
        transform.position = system.position;
        transform.rotation = system.rotation;



    }
}
