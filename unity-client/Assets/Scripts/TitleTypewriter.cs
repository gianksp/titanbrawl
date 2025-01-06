using TMPro; // For TextMeshPro
using System.Collections; // Required for IEnumerator
using UnityEngine;

public class TitleTypewriterWithCursor : MonoBehaviour
{    public TMP_Text titleText; // The TextMeshPro object
    public float typeSpeed = 0.1f; // Time between each character
    public string cursorCharacter = "_"; // The cursor symbol
    public float cursorBlinkSpeed = 0.5f; // Speed of cursor blinking

    private string fullText = "Titan Brawl"; // Your title text
    private string typedText = ""; // Text that has been typed so far
    private bool showCursor = true; // Toggles cursor visibility

    void Start()
    {
        StartCoroutine(TypeText());
        StartCoroutine(CursorBlink());
    }

    IEnumerator TypeText()
    {
        typedText = ""; // Clear the text initially
        titleText.text = ""; // Clear the TMP_Text content

        foreach (char letter in fullText)
        {
            typedText += letter; // Append one character at a time
            titleText.text = typedText + cursorCharacter; // Add the cursor
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    IEnumerator CursorBlink()
    {
        while (true)
        {
            showCursor = !showCursor;

            // Add or replace the cursor with a non-breaking space to keep text stable
            titleText.text = typedText + (showCursor ? cursorCharacter : "\u2007");
            yield return new WaitForSeconds(cursorBlinkSpeed);
        }
    }
}
