using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUD_Text : MonoBehaviour
{
    [SerializeField]TextMeshProUGUI text;
    public float fadeInDuration = 0.5f; // Duration to fade in
    public float fadeOutDuration = 2.0f;
    public float moveDistance = 50.0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SplashText(string newText)
    {
        text.text = "Splashed: "+ newText;
        StartCoroutine(FadeAndMoveCoroutine());
    }
    //lean move and fade out 
    IEnumerator FadeAndMoveCoroutine()
    {
        text.gameObject.SetActive(true); // Show the text

        // Fade in using LeanTween
        LeanTween.alphaCanvas(text.GetComponent<CanvasGroup>(), 1f, fadeInDuration)
                 .setEase(LeanTweenType.easeOutSine);

        // Move up using LeanTween
        LeanTween.moveLocalY(text.gameObject, text.transform.localPosition.y + moveDistance, fadeInDuration)
                 .setEase(LeanTweenType.easeOutSine);

        yield return new WaitForSeconds(fadeInDuration); // Wait for fade in to complete

        // Fade out
        LeanTween.alphaCanvas(text.GetComponent<CanvasGroup>(), 0f, fadeOutDuration)
                 .setEase(LeanTweenType.easeOutSine)
                 .setOnComplete(() => text.gameObject.SetActive(false)); // Hide when fade-out is done

    }
}