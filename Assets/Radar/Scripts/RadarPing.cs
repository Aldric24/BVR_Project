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
    private void Awake()
    {
        pingHighlight = transform.Find("TGTselec").gameObject;
    }

    //private void Update() {
    //    disappearTimer += Time.deltaTime;

    //    color.a = Mathf.Lerp(disappearTimerMax, 0f, disappearTimer / disappearTimerMax);
    //    spriteRenderer.color = color;

    //    if (disappearTimer >= disappearTimerMax) {
    //        Destroy(gameObject);
    //    }
    //}

    //public void SetColor(Color color) {
    //    this.color = color;
    //}

    //public void SetDisappearTimer(float disappearTimerMax) {
    //    this.disappearTimerMax = disappearTimerMax;
    //    disappearTimer = 0f;
    //}

}
