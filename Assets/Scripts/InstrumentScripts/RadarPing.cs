
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
        if(rotation!=null)
        {
            transform.rotation = rotation.rotation;
        }
        if(pingHighlight)
        {
            pingHighlight.transform.rotation = gameObject.transform.rotation;

        }
    }
    

}
