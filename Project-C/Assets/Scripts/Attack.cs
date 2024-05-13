using UnityEngine;

[System.Serializable]
public class Attack
{
    public string name;
    public string id;
    public string description;
    public string moveLabel;
    public int priority;
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
    public string hitboxType;
    public string attackType;
}
