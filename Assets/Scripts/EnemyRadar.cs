using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyRadar : MonoBehaviour
{
    float position;
    [SerializeField] private float rotationSpeed;
    

    [SerializeField] private GameObject spriteExtreme;



    private void Awake()
    {
        
        StartCoroutine(SweepRotationCoroutine());
       
      
    }
    private IEnumerator SweepRotationCoroutine()
    {
        while (true)  // Run indefinitely
        {
            position = Mathf.PingPong(Time.time * rotationSpeed, 300f) - 150f;

           
            transform.localPosition = new Vector3(position, 0, 0);

            yield return null;
        }
    }
}
