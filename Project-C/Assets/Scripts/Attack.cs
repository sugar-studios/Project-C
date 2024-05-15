using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Hit
{
    public Vector3 size;
    public float scale;
    public Vector3 rotation;
    public Vector3 position;
    public float startup;
    public float active;
    public float endlag;
    public float damage;
    public float knockback;
    public bool isSetKnockback;
    public float knockbackScaling;
    public float speed;        // Added for projectiles
    public float duration;     // Added for projectiles
    public float dropOffRate;  // Added for projectile drop-off
    public float maxChargeTime; // Added for chargeable projectiles
}

[System.Serializable]
public class Attack
{
    public string name;
    public string id;
    public string description;
    public string moveLabel;
    public int priority;
    public List<Hit> hits; // List of hits for multi-hit attacks
    public string hitboxType;
    public string attackType;
    public bool chargeable;    // Added for chargeable projectiles
    public float projectileDir; // Added for projectile direction
    public bool maintainMomentum; // Added to maintain momentum during attack
}
