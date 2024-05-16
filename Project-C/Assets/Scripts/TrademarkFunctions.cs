using System;
using UnityEngine;

public class TrademarkFunctions : MonoBehaviour
{
    public PlayerAttacker PA;
    public PlayerController PC;

    public void default_trademark_neutral_hit()
    {
        Debug.Log("Blast!");
    }
    public void default_trademark_downward_attack()
    {
        Debug.Log("Recover");
    }
    public void default_trademark_forward_attack()
    {
        Debug.Log("Recover");
    }
    public void default_trademark_upward_attack()
    {
        Debug.Log("Charge up");
    }
}
