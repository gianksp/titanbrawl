using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Action
{
    public string name;
    public float energy;
    public float damagePerHit;
    public float minRange;
    public float maxRange;
    public float duration;
    public string description;

    public int totalAttacks;
}

[System.Serializable]

public class ActionList
{
    public List<Action> actions;

    public Action GetAction(string name) {
        return actions.Find(a => a.name.Equals(name, System.StringComparison.OrdinalIgnoreCase));
    }

    public bool IsActionInRange(string name, float distance) {
        Action action = GetAction(name);
        bool inRange = distance >= action.minRange && distance <= action.maxRange;
        // Debug.Log($"Distance {distance} in range? {inRange} for {name}");
        return inRange;
    }
}