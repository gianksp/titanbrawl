using UnityEngine;
using TMPro; // If using TextMeshPro

public class DistanceLine : MonoBehaviour
{
    public Transform player1; // Reference to Player 1
    public Transform player2; // Reference to Player 2
    public LineRenderer lineRenderer; // Reference to the LineRenderer
    public TextMeshPro distanceText; // Reference to a UI Text element (Optional)

    void Start()
    {
        if (!lineRenderer)
        {
            Debug.LogError("LineRenderer not assigned!");
        }

        if (!player1 || !player2)
        {
            Debug.LogError("Player Transforms not assigned!");
        }
    }

    void Update()
    {
        if (player1 && player2 && lineRenderer)
        {
            // Update the line positions
            lineRenderer.SetPosition(0, player1.position);
            lineRenderer.SetPosition(1, player2.position);

            // Calculate the distance
            float distance = Vector3.Distance(player1.position, player2.position);

            // Update the distance text (Optional)
            if (distanceText)
            {
                distanceText.text = $"{distance:F2}m";
                // Position the text midway between the players
                distanceText.transform.position = (player1.position + player2.position) / 2;
            }
        }
    }
}