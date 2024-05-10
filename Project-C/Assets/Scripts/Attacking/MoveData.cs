using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MoveData
{
    public string name;
    public Vector3 coordinates;
    public string hitboxType;
    public Vector3 rotation;
    public float damage;
    public float startup;
    public float active;
    public float endLag;

    public MoveData(string name, Vector3 coordinates, string hitboxType, Vector3 rotation, float damage, float startup, float active, float endLag)
    {
        this.name = name;
        this.coordinates = coordinates;
        this.hitboxType = hitboxType;
        this.rotation = rotation;
        this.damage = damage;
        this.startup = startup;
        this.active = active;
        this.endLag = endLag;
    }
}

public class MoveSet
{
    public Dictionary<string, MoveData> moves = new Dictionary<string, MoveData>();

    public MoveSet()
    {
        InitializeMoves();
    }

    private void InitializeMoves()
    {
        // Example initialization for one move
        moves.Add("GroundedLeftRightNormal", new MoveData("GroundedLeftRightNormal", new Vector3(1, 0, 0), "Square", Vector3.zero, 10.0f, 0.1f, 0.5f, 0.2f));
        // Add other moves similarly
    }
}
