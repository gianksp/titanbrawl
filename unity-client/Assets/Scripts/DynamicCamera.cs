using UnityEngine;

public class DynamicCamera : MonoBehaviour
{
        public Transform boxer1; // Reference to the first boxer
    public Transform boxer2; // Reference to the second boxer

    public float minZoom = 5f; // Minimum zoom level
    public float maxZoom = 20f; // Maximum zoom level
    public float zoomSpeed = 10f; // Speed at which the camera zooms
    public float zoomLimiter = 10f; // Limits zoom based on distance scaling

    public float rotationSpeed = 5f; // Speed of the camera rotation

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("Camera component is missing!");
        }
    }

    void LateUpdate()
    {
        if (boxer1 == null || boxer2 == null)
            return;

        AdjustZoom();
        AdjustPositionAndRotation();
    }

    void AdjustZoom()
    {
        // Calculate the distance between the boxers
        float distance = Vector3.Distance(boxer1.position, boxer2.position);

        // Determine the desired zoom level
        float targetZoom = Mathf.Lerp(maxZoom, minZoom, distance / zoomLimiter);

        // Smoothly interpolate the camera's field of view to the target zoom
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetZoom, Time.deltaTime * zoomSpeed);
    }

    void AdjustPositionAndRotation()
    {
        // Calculate the midpoint between the two boxers
        Vector3 midpoint = (boxer1.position + boxer2.position) / 2f;

        // Adjust the camera's position to focus on the midpoint
        transform.position = new Vector3(midpoint.x, midpoint.y, transform.position.z);

        // Calculate the direction vector between the boxers
        Vector3 direction = boxer2.position - boxer1.position;

        // Calculate the new camera rotation to face the midpoint with slight adjustments
        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);

        // Smoothly rotate the camera to the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}