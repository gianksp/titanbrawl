using UnityEngine;

public class BodyColliderManager : MonoBehaviour
{

    public Collider headCollider;
    public Collider neckCollider;
    public Collider chestCollider;
    public Collider abdomenCollider;

    private TitanController controller;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = gameObject.GetComponent<TitanController>();
    }

    // Update is called once per frame
    void Update()
    {

        if (controller.HasGuardUp()) {
            chestCollider.enabled = false;
            headCollider.enabled = false;
            neckCollider.enabled = true;
            abdomenCollider.enabled = true;
        } else if (controller.HasGuardHighUp()) {
            chestCollider.enabled = false;
            headCollider.enabled = false;
            neckCollider.enabled = false;
            abdomenCollider.enabled = true;
        } else if (controller.HasGuardLowUp()) {
            chestCollider.enabled = false;
            headCollider.enabled = true;
            neckCollider.enabled = true;
            abdomenCollider.enabled = false;
        } else {
            chestCollider.enabled = true;
            headCollider.enabled = true;
            neckCollider.enabled = true;
            abdomenCollider.enabled = true;
        }
    }
}
