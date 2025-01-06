using UnityEngine;

public class CinematicCamera : MonoBehaviour
{
    public Transform boxer1; // Reference to the first boxer's transform
    public Transform boxer2; // Reference to the second boxer's transform
    public float rotationSpeed = 10f; // Speed of circling the boxers
    public float heightOffset = 2f; // Height above the midpoint
    public float zoomDistance = 5f; // Distance from the midpoint
    public float zoomSpeed = 2f; // Speed of zooming in and out

    private Vector3 midpoint;
    private float currentAngle = 0f;

    void Start()
    {
        if (boxer1 == null || boxer2 == null)
        {
            Debug.LogError("Boxers are not assigned!");
            return;
        }

        // Calculate the initial midpoint between the boxers
        midpoint = (boxer1.position + boxer2.position) / 2;
    }

    void Update()
    {
        if (boxer1 == null || boxer2 == null) return;

        // Update the midpoint dynamically in case boxers move
        midpoint = (boxer1.position + boxer2.position) / 2;

        // Increment the angle based on rotation speed
        currentAngle += rotationSpeed * Time.deltaTime;

        // Calculate the position for the camera
        float x = Mathf.Sin(currentAngle * Mathf.Deg2Rad) * zoomDistance;
        float z = Mathf.Cos(currentAngle * Mathf.Deg2Rad) * zoomDistance;
        Vector3 cameraPosition = new Vector3(midpoint.x + x, midpoint.y+0.5f + heightOffset, midpoint.z + z);

        // Set the camera's position
        transform.position = cameraPosition;

        // Make the camera look at the midpoint
        transform.LookAt(new Vector3(midpoint.x, midpoint.y+1.2f, midpoint.z));

        // Zoom in or out with user input (optional)
        if (Input.GetKey(KeyCode.W)) zoomDistance -= zoomSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.S)) zoomDistance += zoomSpeed * Time.deltaTime;

        // Clamp the zoom distance to prevent issues
        zoomDistance = Mathf.Clamp(zoomDistance, 1f, 15f);
    }
}
