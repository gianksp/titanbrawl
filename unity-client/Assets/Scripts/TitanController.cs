using UnityEngine;
using MoonSharp.Interpreter;
using System.Collections;
using System.Collections.Generic;

public class TitanController : MonoBehaviour {

    private int totalStrikes = 0;

    public int GetTotalStrikes() {
        return totalStrikes;
    }

    public void RegisterStrike(int total) {
        totalStrikes += total;
    }

    private int strikesLanded = 0;

    public int GetStrikesLanded() {
        return strikesLanded;
    }

    public void RegisterStrikeLanded() {
        strikesLanded += 1;
    }

    private int criticalStrikes = 0;

    public int GetCriticalStrikes() {
        return criticalStrikes;
    }

    public void RegisterCriticalStrikeLanded() {
        criticalStrikes += 1;
        RegisterStrikeLanded();
    }

    private Animator animator;
    
    private bool active = false;

    private Script luaScript;
    private DynValue updateFunction;

    private float health = 100;
    private float energy = 100f;
    public float minDistance = 0.88f;

    private float energyGenerated = 0f;
    

    public ActionList actionList;
    public TitanController opponentController;

    private string state;
    private bool ready;

    private string name;
    private string mode;
    private string trigger;

    private float energyRegen = 1f;

    private float energySpent = 0f;

private float lastExecutionTime = 0f; // Tracks the last time the method was executed
    public float cooldown = 0.3f; // Cooldown duration in seconds

    /// <summary>
    /// 
    /// </summary>
    void Start() {

        // Load costs matrix
        TextAsset jsonText = Resources.Load<TextAsset>("values");
        actionList = JsonUtility.FromJson<ActionList>($"{{\"actions\":{jsonText.text}}}");
        Debug.Log("Costs loaded successfully!");

        animator = GetComponent<Animator>();
        luaScript = new Script();

        // Register the C# function as a global Lua function
        luaScript.Globals["run"] = (System.Action<string, float>)TriggerAction;
        luaScript.Globals["init"] = (System.Action<string, string>)InitTitan;
        luaScript.Globals["set_guard"] = (System.Action<int>)SetGuard;
        luaScript.Options.DebugPrint = s => Debug.Log($"[Lua]: {s}");

        // Energy generator
        InvokeRepeating("IncreaseEnergy", 0f, 0.1f);
        InvokeRepeating("UpdateScript",0f, 0.2f);
    }

    void SetGuard(int guardType) {
        animator.SetInteger("guard", guardType);
    }

    public float GetEnergySpent() {
        return this.energySpent;
    }

    void InitTitan(string name, string mode) {

        if (this.name != null) {
            return;
        }

        this.name = name;
        this.mode = "init_"+mode;
    }

    public void ActivatePassive() {
        switch (this.mode) {
            case "init_speed":
                animator.SetTrigger("init_speed");
                break;

            case "init_energy":
                animator.SetTrigger("init_energy");
                break;

            case "init_resillience":
                animator.SetTrigger("init_resillience");
                break;
            case "init_power":
                animator.SetTrigger("init_power");
                break;
            default:
                Debug.Log("Value is none");
                break;
        }
    }

    public string GetMode() {
        return this.mode;
    }

    public string GetName() {
        return this.name;
    }

    public float GetEnergyGenerated() {
        return this.energyGenerated;
    }

    void OnAnimatorMove()
    {
        // Apply root motion movement
        Vector3 newPosition = transform.position + animator.deltaPosition;

        // Enforce minimum distance
        if (gameObject.tag == "RedPlayer")
        {
            // Ensure Red Player cannot move closer than minDistance to the Blue Player
            if (newPosition.x + minDistance > opponentController.gameObject.transform.position.x)
            {
                newPosition.x = opponentController.gameObject.transform.position.x - minDistance;
            }
        }
        else if (gameObject.tag == "BluePlayer")
        {
            // Ensure Blue Player cannot move closer than minDistance to the Red Player
            if (newPosition.x - minDistance < opponentController.gameObject.transform.position.x)
            {
                newPosition.x = opponentController.gameObject.transform.position.x + minDistance;
            }
        }

        // Clamp to screen
        newPosition.x = Mathf.Clamp(newPosition.x, -2f, 2f);
        newPosition.z = 0f;

        // Apply the corrected position
        transform.position = newPosition;

        // Debugging the minimum distance enforcement
        float currentDistance = GetDistanceToOpponent();
        // Debug.Log($"Current Distance Between Players: {currentDistance}");
    }

    float GetDistanceToOpponent() {
        return Mathf.Abs(transform.position.x - opponentController.gameObject.transform.position.x);
    }
    public bool IsReady() {
        return ready;
    }

    public float GetEnergyRegen() {
        return this.energyRegen;
    }

    public void IncreaseEnergy() {
        if ((energy+energyRegen/10f) <= 100f) {
            float targetEnergy = energyRegen/10f;
            this.energyGenerated += targetEnergy;
            energy = Mathf.Clamp(energy+energyRegen/10f, 0f, 100f);
        }
    }

    public void IncreaseEnergyRegen() {
        StartCoroutine(ShortlyIncreaseEnergyRegenByOne());
    }
    private IEnumerator ShortlyIncreaseEnergyRegenByOne() {
        this.energyRegen += 1f;
        yield return new WaitForSeconds(5f);
        this.energyRegen -=1f;
    }


    void DecreaseEnergy(float cost) {
        Debug.Log($"{GetName()} adding energy costs + {cost}");
        this.energySpent = this.energySpent + cost;
        energy = Mathf.Clamp(energy-cost, 0f, 100f);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    bool HasEnergy(float amount) {
        return energy - amount >= 0;
    }

    void DecreaseHP(float amount) {
        health = Mathf.Clamp(health-amount, 0f, 100f);
    }

    bool HasHP(float amount) {
        return health - amount >= 0;
    }

    bool IsDead() {
        return health == 0;
    }

    public Animator GetAnimator() {
        return animator;
    }

    public string GetCurrentAnimationName() {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        string animationName = GetAnimationName(stateInfo.shortNameHash);
        return animationName;
    }

    public Table GetPosition() {
        Table luaPosition = new Table(luaScript);
        luaPosition["x"] = transform.position.x;
        luaPosition["y"] = transform.position.y;
        luaPosition["z"] = transform.position.z;
        return luaPosition;
    }

    public Table GetOpponentPosition() {
        Table luaPosition = new Table(luaScript);
        luaPosition["x"] = opponentController.gameObject.transform.position.x;
        luaPosition["y"] = opponentController.gameObject.transform.position.y;
        luaPosition["z"] = opponentController.gameObject.transform.position.z;
        return luaPosition;
    }

    void UpdateScript() {
        if (IsPoweredUp() && active) {
            // Update lua instance

                luaScript.Call(
                    updateFunction, 
                    GetCurrentAnimationName(), 
                    GetEnergy(), 
                    GetHealth(),
                    opponentController.GetCurrentAnimationName(),
                    opponentController.GetEnergy(),
                    opponentController.GetHealth(),
                    Vector3.Distance(transform.position, opponentController.gameObject.transform.position)
                );
   
        }
    }

    void Update() {
        if (IsPoweredUp()) {
            // Debug.Log(energy);
            
            transform.LookAt(opponentController.gameObject.transform);

            if (opponentController.IsDead()) {
                DeclareWinner();
                return;
            }

            // // Update lua instance
            // if (active) {
            //     luaScript.Call(
            //         updateFunction, 
            //         GetCurrentAnimationName(), 
            //         GetEnergy(), 
            //         GetHealth(),
            //         GetPosition(),
            //         opponentController.GetCurrentAnimationName(),
            //         opponentController.GetEnergy(),
            //         opponentController.GetHealth(),
            //         GetOpponentPosition()
            //     );
            // }
        }
    }

    public void DeclareWinner() {
        animator.SetTrigger("win");
        PowerDown();
    }

    public float GetCurrentAnimationDamage() {
        Action current = actionList.GetAction(GetCurrentAnimationName());
        return current != null ? current.damagePerHit : 0;
    }

    public bool HasGuardUp() {
        return GetCurrentAnimationName() == "on_guard";
    }

        public bool HasGuardHighUp() {
        return GetCurrentAnimationName() == "on_guard_high";
    }

        public bool HasGuardLowUp() {
        return GetCurrentAnimationName() == "on_guard_low";
    }

    public bool IsGuarding() {
        return HasGuardHighUp() || HasGuardLowUp() || HasGuardUp();
    }

    public string GetLatestTrigger() {
        return trigger;
    }

    public void ResetLatestTrigger() {
        string latestTrigger = GetLatestTrigger();
        animator.ResetTrigger(latestTrigger);
        animator.SetTrigger("on_guard");
        // Debug.Log($"Animator has reset {latestTrigger} for {gameObject.tag}");
    }

    public bool IsExecutingTag(string action)
    {
        // Get the AnimatorStateInfo for the current animation state
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsTag(action);
    }

    public void TriggerAction(string action, float speed) {
        // Debug.Log("Trigger??");

        this.trigger = action;
        Action actionFound = actionList.GetAction(action);

        if (actionFound == null) {
            return;
        }

        float cost = actionFound.energy*speed;
        float currentDistance = GetDistanceToOpponent();
        if (HasEnergy(cost) && IsGuarding() && actionList.IsActionInRange(action, currentDistance)) {
            // transform.LookAt(opponentController.gameObject.transform);
            animator.speed = speed;
            animator.SetTrigger(action);
            DecreaseEnergy(cost);
            RegisterStrike(actionList.GetAction(action).totalAttacks);
            StartCoroutine(ResetAfterAnimation());
            // StartCoroutine(Cooldown());
        } else {
            // Debug.Log("Titan is currently performing an action or does not have enough energy!");
        }
    }

    private IEnumerator ResetAfterAnimation() {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float waitTime = stateInfo.length/animator.speed;
        yield return new WaitForSeconds(waitTime);
        ResetSpeed();
    }
    /// <summary>
    /// Cooldown implementation on any animation in order for the user to trigger an attack
    /// any animation will incur in a 2x cooldown time.
    /// </summary>
    /// <returns></returns>
    // private IEnumerator Cooldown() {
    //     canAttack = false;
    //     AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
    //     float waitTime = stateInfo.length;
    //     // Debug.Log(waitTime);
    //     yield return new WaitForSeconds(waitTime*2f);
    //     canAttack = true;
    // }

    private void ResetSpeed() {
        animator.speed = 1f;
    }

    public void OnHit(Vector3 impactPoint, float damage, string target) {
        // Debug.Log($"{gameObject.tag} has been hit at {impactPoint.x} {impactPoint.y}, {impactPoint.z} with speed {damage}");
        // animator.SetTrigger("OnHitCross");


//  Calculate if left or right
            Vector3 localPoint = transform.InverseTransformPoint(impactPoint);
            // Debug.Log($"Local point {localPoint.x} {localPoint.y} {localPoint.z}");
            bool isRight =localPoint.x > 0;


        if (Time.time - lastExecutionTime >= cooldown)
        {



        // Debug.Log($"{gameObject.tag} received damage {damage} on {target}");
            DecreaseHP(damage);
            // Die
            if (IsDead()) {
                animator.SetTrigger("knockdown");
                PowerDown();
            } else {
                ResetSpeed();
                if (target.Contains("Neck")) {
                    animator.SetTrigger("hit_move_backwards");
                    opponentController.RegisterCriticalStrikeLanded();
                } else if (target.Contains("Head")) {
                    animator.SetTrigger("hit");
                    // Debug.Log($"Impact point at {impactPoint}");
                    opponentController.RegisterStrikeLanded();
                } else if (target.Contains("Spine2")) {
                    animator.SetTrigger("hit_body");
                    opponentController.RegisterCriticalStrikeLanded();
                } else if (target.Contains("Spine3")) {
                    animator.SetTrigger(isRight? "hit_upper_body_right":"hit_upper_body_left");
                    opponentController.RegisterStrikeLanded();
                }
                // if (impactPoint.y > 1.2f) {
                //     // Hit in face
                    
                //     // animator.ResetTrigger("Jab");
                //     // animator.ResetTrigger("Hook");
                //     // animator.SetTrigger("OnHitCross");
                // } else {
                //     // Hit in body
                //     // animator.SetTrigger("OnHitBody");
                // }
                // StartCoroutine(Cooldown());
            }

            lastExecutionTime = Time.time; // Update the last execution time
        } else {
            // Debug.Log($"{gameObject.tag} is onHit coolwon for {lastExecutionTime}");
        }

    }

   // Convert the shortNameHash to a readable animation name
    string GetAnimationName(int hash)
    {
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (Animator.StringToHash(clip.name) == hash)
            {
                return clip.name;
            }
        }
        return "Unknown Animation";
    }

    public float GetEnergy() {
        return energy;
    }

    public float GetHealth() {
        return health;
    }

    // void OnTriggerEnter(Collider other) {

    //     // if (gameObject.tag.Contains("Player") && other.tag.Contains("Fist")) {
    //     //     // This object is being hit by enemy fists
    //     //     Debug.Log($"{gameObject.tag} is being hit by {other.tag}");
    //     // }
    //     // if (!other.CompareTag(gameObject.tag))
    //     // {
    //     //     // Debug.Log($"I am {gameObject.tag} OnHit by {other.tag}");
    //     //     // animator.SetTrigger("OnHitCross");
    //     //     if (HasHP(10f)) {
    //     //         Vector3 collisionPoint = other.ClosestPoint(transform.position);
    //     //         DecreaseHP(10f);
    //     //         // Die
    //     //         if (IsDead()) {
    //     //             animator.SetTrigger("OnKnockedDown");
    //     //             PowerDown();
    //     //         } else {
    //     //             animator.SetTrigger("OnHitCross");
    //     //         }
    //     //     }
    //     //     // Debug.Log($"Collision point: {collisionPoint}");
    //     // }
    //     // else
    //     // {

    //     // }
    // }

    public void LoadSourceCode(string sourceCode) {
        if (string.IsNullOrWhiteSpace(sourceCode)) {
            Debug.Log("Script is empty!");
            return;
        }

        try {
            // Execute the Lua script
            luaScript.DoString(sourceCode);
            Debug.Log("Script executed successfully!");

            // Init functions
            updateFunction = luaScript.Globals.Get("update");

            ready=true;
        }
        catch (ScriptRuntimeException e) {
            Debug.LogError($"Lua Error: {e.Message}");
        }
    }

    public void PowerUp() {
        this.active = true;
        this.health = 100f;
        this.energy = 100f;
    }

    public void PowerDown() {
        this.active = false;
        ResetLatestTrigger();
    }

    public bool IsPoweredUp() {
        return this.active;
    }

    // private Dictionary<string, string> GetState() {
    //     return
    // }
}
