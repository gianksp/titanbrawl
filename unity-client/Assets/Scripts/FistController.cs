using UnityEngine;

public class FistController : MonoBehaviour
{
    public TitanController controller;
    public GameObject hitEffectPrefab;
    public GameObject blockEffectPrefab;
    public AudioSource audioSource;
    public AudioClip[] hitSounds; 
    public AudioClip[] blockSounds; 


    private float lastTriggerTime = 0f; // Keeps track of the last trigger time
    private float triggerCooldown = 0.15f; // Cooldown time in seconds (100 ms)


    void Start() {
        controller = transform.root.gameObject.GetComponent<TitanController>();
    }

    void OnTriggerEnter(Collider other) {
        // Check if enough time has passed since the last trigger
        if (Time.time - lastTriggerTime >= triggerCooldown)
        {
            lastTriggerTime = Time.time; // Update the last trigger time
            
            // Your trigger logic here
            Debug.Log($"Triggered by: {other.gameObject.name} at {Time.time}");
            processCollision(other);
        }
    }

    void processCollision(Collider other) {

        Vector3 collisionPoint = GetComponent<Collider>().ClosestPoint(other.transform.position);


        if ((gameObject.tag.Contains("BlueFist") && other.tag.Contains("RedFist")) ||
                    (gameObject.tag.Contains("RedFist") && other.tag.Contains("BlueFist"))) {
                // if (controller.IsExecutingTag("guard")) {
                //        Debug.Log($"Player {controller.GetName()} has blocked successfully");
                //         TitanController opponentController = other.gameObject.GetComponent<FistController>().controller;
                        
                //         opponentController.ResetLatestTrigger();
                 
                //     // opponentController.ResetLatestTrigger();
                    
                //     // Vector3 collisionPoint = GetComponent<Collider>().ClosestPoint(other.transform.position);
                //     // controller.opponentController.OnHit(collisionPoint, controller.GetCurrentAnimationDamage(), other.gameObject.name);
                //     // PlayRandomSound();
                //     // Instantiate(effectPrefab, collisionPoint, Quaternion.identity);
                //     RegisterBlock(collisionPoint);
                // }
                RegisterBlock(collisionPoint);
                if (controller.IsGuarding()) {
                    controller.IncreaseEnergyRegen();
                }
            // Fists intercept, whoever is on_guard blocks
            // if (controller.HasGuardUp()) {
            //     controller.GetAnimator().SetTrigger("parry_left");
            // } else {
            //     TitanController opponentController = other.gameObject.GetComponent<TitanController>();
            //     opponentController.ResetLatestTrigger();
            // }
        } else if((gameObject.tag.Contains("BlueFist") && other.tag.Contains("RedPlayer")) ||
           (gameObject.tag.Contains("RedFist") && other.tag.Contains("BluePlayer"))) {
      
            RegisterContact(collisionPoint);
            controller.opponentController.OnHit(collisionPoint, controller.GetCurrentAnimationDamage(), other.gameObject.name);

        //     if (controller.IsGuarding()) {
        //         // controller.RefundEnergy();
        //     } else {
        //         // controller.opponentController.RefundEnergy();
        //     }
        }
    }

        private void RegisterContact(Vector3 collisionPoint) {
                        PlayRandomSound(hitSounds);
                    Instantiate(hitEffectPrefab, collisionPoint, Quaternion.identity);
        }

        private void RegisterBlock(Vector3 collisionPoint) {
                                    PlayRandomSound(blockSounds);
                    Instantiate(blockEffectPrefab, collisionPoint, Quaternion.identity);
        }
        private void PlayRandomSound(AudioClip[] baseSounds)
    {
        if (baseSounds.Length > 0)
        {
            // Select a random sound from the array
            int randomIndex = Random.Range(0, baseSounds.Length);
            AudioClip selectedClip = baseSounds[randomIndex];

            // Play the selected sound
            audioSource.PlayOneShot(selectedClip);
        }
        else
        {
            Debug.LogWarning("No hit sounds assigned to the array.");
        }
    }
}
