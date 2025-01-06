using TMPro; // For TextMeshPro
using System.Collections; // Required for IEnumerator
using UnityEngine;


public class FadeIn : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float fadeDuration = 2f; // Time for the fade-in effect

    private CanvasGroup canvasGroup;

    void Start()
    {
        // Get or add a CanvasGroup component for the fade effect
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0; // Start fully transparent
        StartCoroutine(FadeInEffect());
    }

    private System.Collections.IEnumerator FadeInEffect()
    {
        float elapsedTime = 0;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration); // Gradually increase alpha
            yield return null;
        }

        canvasGroup.alpha = 1; // Ensure fully visible at the end
    }
}
