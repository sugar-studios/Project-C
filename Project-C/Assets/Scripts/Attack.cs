using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Attack
{
    public string name;
    public string id;
    public string description;
    public string moveLabel;
    public int priority;
    public List<Hit> hits;
    public string hitboxType;
    public string attackType;
    public bool chargeable;
    public float projectileDir;
    public bool maintainMomentum;
}

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
    public int damage;
    public float knockback;
    public bool isSetKnockback;
    public float knockbackScaling;
    public float speed;
    public float duration;
    public float dropOffRate;
    public float maxChargeTime;
}
