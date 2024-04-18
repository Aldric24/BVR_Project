using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
    // Start is called before the first frame update
    
    [SerializeField]private Transform player;
    void Start()
    {
       player = FindObjectOfType<NewControl>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<TMPro.TextMeshProUGUI>().text = player.eulerAngles.z.ToString("F0") + "°";
    }
}
