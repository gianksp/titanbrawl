using UnityEngine;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Positioning
{
    public float px;
    public float py;
    public float pz;
    public float rx;
    public float ry;
    public float rz;
}

public class RandomStage : MonoBehaviour
{

    public Positioning[] configurations;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake() {
        if (configurations.Length > 0)
        {
            // Select a random sound from the array
            int randomIndex = Random.Range(0, configurations.Length);
            Positioning config = configurations[randomIndex];

            // Play the selected sound
            transform.position = new Vector3(config.px, config.py, config.pz);
            transform.rotation = Quaternion.Euler(new Vector3(config.rx, config.ry, config.rz));
        }
        else
        {
            Debug.LogWarning("No hit sounds assigned to the array.");
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
