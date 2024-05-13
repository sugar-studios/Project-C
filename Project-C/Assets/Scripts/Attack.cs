using System.Collections;
using UnityEngine;

public class Attack
{
    public enum HitboxType { Square, Capsule }

    public HitboxType Type;
    public float Damage;
    public float Knockback;
    public float KnockbackScaling;
    public float Startup;
    public float Active;
    public float Endlag;
    public string ID;
    public int Priority;
    public GameObject HitboxPrefab;
    public Vector3 HitboxSize;
    public Vector3 Rotation;

    public Attack(HitboxType type, float damage, float knockback, float knockbackScaling, float startup, float active, float endlag, string id, int priority, GameObject prefab, Vector3 size, Vector3 rotation)
    {
        Type = type;
        Damage = damage;
        Knockback = knockback;
        KnockbackScaling = knockbackScaling;
        Startup = startup;
        Active = active;
        Endlag = endlag;
        ID = id;
        Priority = priority;
        HitboxPrefab = prefab;
        HitboxSize = size;
        Rotation = rotation;
    }
}
