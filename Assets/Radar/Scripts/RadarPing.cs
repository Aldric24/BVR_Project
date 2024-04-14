/* 
    ------------------- Code Monkey -------------------
    
    Thank you for downloading the Code Monkey Utilities
    I hope you find them useful in your projects
    If you have any questions use the contact form
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarPing : MonoBehaviour {

    //private SpriteRenderer spriteRenderer;
    //private float disappearTimer;
    //private float disappearTimerMax;
    //private Color color;
    public GameObject pingHighlight;
    public Transform rotation;
    private void Awake()
    {
        pingHighlight = transform.Find("TGTselec").gameObject;
    }
    private void Update()
    {
        transform.rotation=rotation.rotation;
        if(pingHighlight)
        {
            pingHighlight.transform.rotation = rotation.rotation;

        }
    }
    

}
